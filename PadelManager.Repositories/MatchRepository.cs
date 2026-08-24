using Microsoft.EntityFrameworkCore;
using PadelManager.Interfaces;
using PadelManager.Models;

namespace PadelManager.Repositories;

public class MatchRepository : IMatchRepository {
    private readonly PadelManagerDbContext _context;

    public MatchRepository(PadelManagerDbContext context) {
        _context = context;
    }

    public async Task<List<Match>> GetForSiteAndDateAsync(int siteId, DateOnly date) {
        // Bornes explicites plutôt que DateHeure.Date == ... (non-sargable, empêche l'usage
        // d'un index sur la colonne).
        var debut = date.ToDateTime(TimeOnly.MinValue);
        var fin = date.AddDays(1).ToDateTime(TimeOnly.MinValue);

        return await _context.Matches
            .Where(m => m.SiteId == siteId && m.DateHeure >= debut && m.DateHeure < fin)
            .ToListAsync();
    }

    public async Task<bool> ExisteAsync(int terrainId, DateTime dateHeure) {
        return await _context.Matches.AnyAsync(m => m.TerrainId == terrainId && m.DateHeure == dateHeure);
    }

    public async Task<Match> AddAsync(Match match) {
        _context.Matches.Add(match);

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateException) {
            // Filet de sécurité DB (UQ_MATCH_terrain_creneau) : conflit concurrent réel malgré
            // la revérification déjà faite par le service (EF-bk-019, même principe qu'ENF-010).
            throw new CreneauIndisponibleException();
        }

        return match;
    }

    public async Task<Match?> GetByIdAsync(int id) {
        return await _context.Matches.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Match>> GetPublicsIncompletsAsync(DateTime maintenant) {
        return await _context.Matches
            .Include(m => m.Site)
            .Include(m => m.Terrain)
            .Include(m => m.Participations)
            .Where(m => m.Visibilite == "PUBLIC" && m.Statut == "INCOMPLET" && m.DateHeure > maintenant)
            .ToListAsync();
    }

    public async Task<Participation> InscrireEtPayerAsync(int matchId, string membreMatricule, Dette? detteAReporter) {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try {
            var match = await VerrouillerMatchAsync(matchId);

            var participationsExistantes = await _context.Participations
                .Where(p => p.MatchId == matchId)
                .ToListAsync();

            if (participationsExistantes.Any(p => p.MembreMatricule == membreMatricule))
                throw new DejaInscritException();

            if (participationsExistantes.Count >= 4)
                throw new MatchCompletException();

            var maintenant = DateTime.Now;
            var participation = new Participation {
                MatchId = matchId,
                MembreMatricule = membreMatricule,
                DateInscription = maintenant,
                Paiement = new Paiement {
                    MontantParticipation = 15.00m,
                    MontantDetteReportee = detteAReporter?.Montant ?? 0.00m,
                    DatePaiement = maintenant
                }
            };
            _context.Participations.Add(participation);

            RegulerDetteEtStatut(match, detteAReporter, matchId, participationsExistantes.Count + 1, maintenant);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return participation;
        } catch {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Participation?> GetParticipationByIdAsync(int id) {
        return await _context.Participations
            .Include(p => p.Paiement)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Participation> PayerParticipationAsync(Participation participation, Dette? detteAReporter) {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try {
            // Même verrou que InscrireEtPayerAsync (sur le MATCH de cette participation) : si
            // plusieurs joueurs du même match privé paient leur part en attente en même temps,
            // il faut compter les participations payées sous un verrou commun pour ne pas rater
            // le passage à COMPLET (ou le déclencher deux fois).
            var match = await VerrouillerMatchAsync(participation.MatchId);

            var nombrePayeesAvant = await _context.Participations
                .CountAsync(p => p.MatchId == participation.MatchId && p.Paiement != null);

            var maintenant = DateTime.Now;
            participation.Paiement = new Paiement {
                MontantParticipation = 15.00m,
                MontantDetteReportee = detteAReporter?.Montant ?? 0.00m,
                DatePaiement = maintenant
            };

            try {
                RegulerDetteEtStatut(match, detteAReporter, participation.MatchId, nombrePayeesAvant + 1, maintenant);
                await _context.SaveChangesAsync();
            } catch (DbUpdateException) {
                // Filet de sécurité DB (UQ_PAIEMENT_participationId) : déjà payée entre-temps.
                throw new ParticipationDejaPayeeException();
            }

            await transaction.CommitAsync();
            return participation;
        } catch {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // Verrouille la ligne MATCH pour toute la durée de la transaction : sérialise les écritures
    // concurrentes de participations sur ce même match, empêchant de dépasser 4 participations
    // payées même en cas de double clic / requêtes simultanées (ENF-010, R-STR-002) — même
    // principe que le filet UQ_MATCH_terrain_creneau pour la création, mais ici il faut un vrai
    // verrou puisqu'aucune contrainte d'unicité ne borne le nombre de lignes PARTICIPATION (ou de
    // PAIEMENT) pour un même matchId.
    private async Task<Match> VerrouillerMatchAsync(int matchId) {
        var match = await _context.Matches
            .FromSqlInterpolated($"SELECT * FROM dbo.[MATCH] WITH (UPDLOCK, HOLDLOCK) WHERE id = {matchId}")
            .SingleOrDefaultAsync();

        if (match == null)
            throw new InvalidOperationException("Match introuvable.");

        return match;
    }

    // EF-bk-018 (règlement automatique d'une dette) + bascule à COMPLET si c'est la 4e
    // participation désormais payée (EF-bk-007, évènementiel, pas de recalcul).
    private static void RegulerDetteEtStatut(Match match, Dette? detteAReporter, int matchId, int nombrePayeesApres, DateTime maintenant) {
        if (detteAReporter != null) {
            detteAReporter.Soldee = true;
            detteAReporter.MatchReglementId = matchId;
            detteAReporter.DateReglement = maintenant;
        }

        if (nombrePayeesApres == 4)
            match.Statut = "COMPLET";
    }
}

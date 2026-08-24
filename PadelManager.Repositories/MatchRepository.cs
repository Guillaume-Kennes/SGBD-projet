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
            // Verrouille la ligne MATCH pour toute la durée de la transaction : sérialise les
            // inscriptions concurrentes sur ce même match, empêchant de dépasser 4 participations
            // même en cas de double clic / requêtes simultanées (ENF-010, R-STR-002) — même
            // principe que le filet UQ_MATCH_terrain_creneau pour la création, mais ici il faut
            // un vrai verrou puisqu'aucune contrainte d'unicité ne borne le nombre de lignes
            // PARTICIPATION pour un même matchId.
            var match = await _context.Matches
                .FromSqlInterpolated($"SELECT * FROM dbo.[MATCH] WITH (UPDLOCK, HOLDLOCK) WHERE id = {matchId}")
                .SingleOrDefaultAsync();

            if (match == null)
                throw new InvalidOperationException("Match introuvable.");

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

            // EF-bk-018 : report et règlement automatique de la dette (déjà trackée par ce même
            // DbContext, pas besoin de la recharger).
            if (detteAReporter != null) {
                detteAReporter.Soldee = true;
                detteAReporter.MatchReglementId = matchId;
                detteAReporter.DateReglement = maintenant;
            }

            // 4e participation validée -> statut COMPLET (EF-bk-007, évènementiel, pas de recalcul).
            if (participationsExistantes.Count + 1 == 4)
                match.Statut = "COMPLET";

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return participation;
        } catch {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

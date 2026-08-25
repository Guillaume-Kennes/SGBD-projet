using PadelManager.Interfaces;
using PadelManager.Models;

namespace PadelManager.Services;

public class JobService : IJobService {
    private const decimal MontantParticipation = 15.00m;
    private const int NombreParticipationsRequises = 4;
    private const int JoursPenalite = 7;

    private readonly IJobRepository _jobRepository;

    public JobService(IJobRepository jobRepository) {
        _jobRepository = jobRepository;
    }

    public async Task ExecuterBasculeAsync(DateOnly demain) {
        var matchs = await _jobRepository.GetMatchsPrivesDeLaDateAsync(demain);
        var maintenant = DateTime.Now;
        var aujourdHui = DateOnly.FromDateTime(DateTime.Today);

        foreach (var match in matchs) {
            var payes = match.Participations.Count(p => p.Paiement != null);
            if (payes >= NombreParticipationsRequises)
                continue; // déjà complet, pas concerné par la bascule

            var nonPayees = match.Participations.Where(p => p.Paiement == null).ToList();

            // R-CALC-004 : appliquée au moment même de la bascule, à partir du jour d'exécution du
            // job (aujourd'hui), pas de la date du match — indépendante d'une éventuelle dette
            // déjà constituée pour ce même organisateur.
            var penalite = new Penalite {
                MembreMatricule = match.OrganisateurMatricule,
                MatchOrigineId = match.Id,
                DateApplication = maintenant,
                DelaiJusquAu = aujourdHui.AddDays(JoursPenalite)
            };

            await _jobRepository.BasculerAsync(match, nonPayees, penalite);
        }
    }

    public async Task ExecuterClotureAsync(DateOnly hier) {
        var matchs = await _jobRepository.GetMatchsDeLaDateAsync(hier);
        var maintenant = DateTime.Now;

        foreach (var match in matchs) {
            // Idempotence (ENF-011) : un match déjà scellé TERMINE par une exécution précédente de
            // cette même nuit a nécessairement déjà eu sa dette constituée le cas échéant — on ne
            // le retraite jamais.
            if (match.Statut == "TERMINE")
                continue;

            var payes = match.Participations.Count(p => p.Paiement != null);
            if (payes < NombreParticipationsRequises) {
                var dette = new Dette {
                    MembreMatricule = match.OrganisateurMatricule,
                    MatchOrigineId = match.Id,
                    Montant = MontantParticipation * (NombreParticipationsRequises - payes),
                    Soldee = false,
                    DateCreation = maintenant
                };

                await _jobRepository.CreerDetteAsync(dette);
            }

            await _jobRepository.ScellerTermineAsync(match);
        }
    }
}

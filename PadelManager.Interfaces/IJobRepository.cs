using PadelManager.Models;

namespace PadelManager.Interfaces;

// Utilisé exclusivement par le job quotidien (padel_job, ENF-004/ENF-011) : bascule des matchs
// privés de demain (EF-bk-009/010) et clôture des matchs d'hier (EF-bk-008, R-VAL-004). Jamais
// utilisé par l'API (couche Repositories distincte de MatchRepository, qui reste côté padel_api).
public interface IJobRepository {
    // Matchs PRIVE dont la date tombe le jour donné, Participations+Paiement inclus pour compter
    // les places payées (bascule, EF-bk-009).
    Task<List<Match>> GetMatchsPrivesDeLaDateAsync(DateOnly date);

    // Tous les matchs (toute visibilité, tout statut) dont la date tombe le jour donné,
    // Participations+Paiement inclus (clôture, EF-bk-008 + scellage TERMINE).
    Task<List<Match>> GetMatchsDeLaDateAsync(DateOnly date);

    // Bascule un match privé incomplet vers public en une seule opération atomique : libère les
    // places impayées (suppression des PARTICIPATION correspondantes), passe la visibilité à
    // PUBLIC, et applique la pénalité à l'organisateur (EF-bk-009/010).
    Task BasculerAsync(Match match, List<Participation> participationsNonPayees, Penalite penalite);

    // Constitue la dette de l'organisateur pour un match resté incomplet à l'heure du match
    // (EF-bk-008).
    Task CreerDetteAsync(Dette dette);

    // Scelle définitivement le statut TERMINE d'un match d'hier (R-VAL-004).
    Task ScellerTermineAsync(Match match);
}

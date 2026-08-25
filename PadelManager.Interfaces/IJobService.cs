namespace PadelManager.Interfaces;

// Job quotidien (EF-bk-008/009/010), exécuté exclusivement par la planification automatique à
// minuit (ENF-011 : aucun déclenchement manuel, ni endpoint, ni commande). Les deux méthodes sont
// idempotentes par construction : une bascule ne retrouve plus le match une fois qu'il est passé
// PUBLIC, et une clôture déjà scellée TERMINE est ignorée sur une nouvelle exécution.
public interface IJobService {
    // Bascule les matchs privés de la date donnée (le lendemain du jour d'exécution, R-VAL-004)
    // restés incomplets : libère les places impayées, passe le match en public, pénalise
    // l'organisateur (EF-bk-009/010). Un match déjà à 4 participations payées, ou déjà public,
    // n'est pas concerné.
    Task ExecuterBasculeAsync(DateOnly demain);

    // Clôture les matchs de la date donnée (la veille du jour d'exécution, R-VAL-004) : constitue
    // la dette de l'organisateur pour tout match encore incomplet à l'heure du match, quel que
    // soit son mode de création (EF-bk-008), puis scelle définitivement TERMINE tous les matchs de
    // cette date, sans exception.
    Task ExecuterClotureAsync(DateOnly hier);
}

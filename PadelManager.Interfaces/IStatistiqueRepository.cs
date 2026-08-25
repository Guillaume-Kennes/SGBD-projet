using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface IStatistiqueRepository {
    // Paiements effectués, optionnellement filtrés par site (EF-bk-015/016, R-CALC-005) —
    // rattachement via PARTICIPATION.matchId -> MATCH.siteId (le site où le paiement a eu lieu,
    // pas le site du match d'origine d'une dette reportée). Participation/Match inclus pour le
    // calcul par le Service (regroupement + somme/distinct en mémoire, pas d'agrégation SQL).
    // Réutilisé pour les membres actifs (EF-bk-016) : seule une participation PAYÉE représente un
    // membre ayant réellement joué, et ce filtre rend la stat indépendante du moment où le job de
    // bascule supprime les places restées impayées.
    Task<List<Paiement>> GetPaiementsAsync(int? siteId);
}

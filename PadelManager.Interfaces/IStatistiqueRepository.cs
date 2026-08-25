using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface IStatistiqueRepository {
    // Paiements effectués, optionnellement filtrés par site (EF-bk-015, R-CALC-005) — rattachement
    // via PARTICIPATION.matchId -> MATCH.siteId (le site où le paiement a eu lieu, pas le site du
    // match d'origine d'une dette reportée). Participation/Match/Site inclus pour le calcul par le
    // Service (regroupement + somme en mémoire, pas d'agrégation SQL).
    Task<List<Paiement>> GetPaiementsAsync(int? siteId);
}

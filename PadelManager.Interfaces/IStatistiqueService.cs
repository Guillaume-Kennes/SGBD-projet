using PadelManager.Models.Dtos;

namespace PadelManager.Interfaces;

public interface IStatistiqueService {
    // Chiffre d'affaires (EF-bk-015, R-CALC-005) : un élément par site concerné (siteId fourni ->
    // ce seul site ; omis -> tous les sites, y compris à 0€ si aucun paiement), montant = somme de
    // PAIEMENT.montantTotal. Aucun filtre de période (non requis par le CDC) ; la portée
    // Global/Site elle-même n'est pas vérifiée ici (comme les autres écrans admin de ce projet).
    Task<List<ChiffreAffairesDto>> ObtenirChiffreAffairesAsync(int? siteId);

    // Statistiques classiques (EF-bk-016) : un élément par site concerné (siteId fourni -> ce seul
    // site ; omis -> tous les sites). Même absence de vérification de portée que les autres écrans
    // admin de ce projet.
    Task<List<StatistiquesDto>> ObtenirStatistiquesAsync(int? siteId);
}

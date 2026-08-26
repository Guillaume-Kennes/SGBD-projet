using PadelManager.Interfaces;

namespace PadelManager.Services;

// Résolution partagée "un site précis, ou tous les sites" — utilisée par StatistiqueService
// (chiffre d'affaires, statistiques) et MatchService (récapitulatif terrains) — factorisée ici
// pour éviter de dupliquer le même bloc dans chaque service.
public static class SiteRepositoryExtensions {
    public static async Task<List<Site>> ObtenirSitesConcernesAsync(this ISiteRepository siteRepository, int? siteId) {
        if (siteId.HasValue) {
            var site = await siteRepository.GetByIdAsync(siteId.Value);
            return site != null ? new List<Site> { site } : new List<Site>();
        }

        return await siteRepository.GetAllAsync();
    }
}

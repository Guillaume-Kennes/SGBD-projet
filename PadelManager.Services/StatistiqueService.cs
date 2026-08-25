using PadelManager.Interfaces;
using PadelManager.Models.Dtos;

namespace PadelManager.Services;

public class StatistiqueService : IStatistiqueService {
    private readonly ISiteRepository _siteRepository;
    private readonly IStatistiqueRepository _statistiqueRepository;

    public StatistiqueService(ISiteRepository siteRepository, IStatistiqueRepository statistiqueRepository) {
        _siteRepository = siteRepository;
        _statistiqueRepository = statistiqueRepository;
    }

    public async Task<List<ChiffreAffairesDto>> ObtenirChiffreAffairesAsync(int? siteId) {
        List<Site> sites;
        if (siteId.HasValue) {
            var site = await _siteRepository.GetByIdAsync(siteId.Value);
            sites = site != null ? new List<Site> { site } : new List<Site>();
        } else {
            sites = await _siteRepository.GetAllAsync();
        }

        var paiements = await _statistiqueRepository.GetPaiementsAsync(siteId);

        // Un site sans aucun paiement doit tout de même apparaître, à 0€ (plutôt que d'être
        // silencieusement absent, ce qui pourrait laisser croire à une anomalie côté admin).
        var montantsParSite = paiements
            .GroupBy(p => p.Participation.Match.SiteId)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.MontantTotal ?? 0));

        return sites
            .Select(s => new ChiffreAffairesDto {
                SiteId = s.Id,
                NomSite = s.Nom,
                Montant = montantsParSite.GetValueOrDefault(s.Id, 0m)
            })
            .OrderBy(d => d.SiteId)
            .ToList();
    }
}

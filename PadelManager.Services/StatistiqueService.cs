using PadelManager.Interfaces;
using PadelManager.Models.Dtos;

namespace PadelManager.Services;

public class StatistiqueService : IStatistiqueService {
    private readonly ISiteRepository _siteRepository;
    private readonly ITerrainRepository _terrainRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IDisponibiliteRepository _disponibiliteRepository;
    private readonly IStatistiqueRepository _statistiqueRepository;

    public StatistiqueService(
        ISiteRepository siteRepository,
        ITerrainRepository terrainRepository,
        IMatchRepository matchRepository,
        IDisponibiliteRepository disponibiliteRepository,
        IStatistiqueRepository statistiqueRepository) {
        _siteRepository = siteRepository;
        _terrainRepository = terrainRepository;
        _matchRepository = matchRepository;
        _disponibiliteRepository = disponibiliteRepository;
        _statistiqueRepository = statistiqueRepository;
    }

    public async Task<List<ChiffreAffairesDto>> ObtenirChiffreAffairesAsync(int? siteId) {
        var sites = await ObtenirSitesConcernesAsync(siteId);

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

    public async Task<List<StatistiquesDto>> ObtenirStatistiquesAsync(int? siteId) {
        var sites = await ObtenirSitesConcernesAsync(siteId);

        var matchs = await _matchRepository.GetTousLesMatchsAsync(siteId);
        var participations = await _statistiqueRepository.GetParticipationsAsync(siteId);

        var resultat = new List<StatistiquesDto>();
        foreach (var site in sites) {
            var matchsDuSite = matchs.Where(m => m.SiteId == site.Id).ToList();
            var nbPublics = matchsDuSite.Count(m => m.Visibilite == "PUBLIC");
            var nbPrives = matchsDuSite.Count(m => m.Visibilite == "PRIVE");

            // Approximation volontairement simple (CDC : "set raisonnable et classique, pas très
            // poussé"), sur l'ensemble de la période disponible en base — pas de fenêtre
            // temporelle à affiner.
            var nbCreneaux = await _disponibiliteRepository.CountBySiteAsync(site.Id);
            var nbTerrains = (await _terrainRepository.GetBySiteIdAsync(site.Id)).Count;
            var capacite = nbCreneaux * nbTerrains;
            var tauxOccupation = capacite > 0 ? (decimal)(nbPublics + nbPrives) / capacite : 0m;

            // Membres actifs : peu importe payée ou non (R-ACC-006 ne bloque que la création, pas
            // la présence dans une statistique de fréquentation).
            var membresActifs = participations
                .Where(p => p.Match.SiteId == site.Id)
                .Select(p => p.MembreMatricule)
                .Distinct()
                .Count();

            resultat.Add(new StatistiquesDto {
                SiteId = site.Id,
                NomSite = site.Nom,
                NombreMatchsPublics = nbPublics,
                NombreMatchsPrives = nbPrives,
                TauxOccupation = tauxOccupation,
                MembresActifs = membresActifs
            });
        }

        return resultat.OrderBy(r => r.SiteId).ToList();
    }

    private async Task<List<Site>> ObtenirSitesConcernesAsync(int? siteId) {
        if (siteId.HasValue) {
            var site = await _siteRepository.GetByIdAsync(siteId.Value);
            return site != null ? new List<Site> { site } : new List<Site>();
        }

        return await _siteRepository.GetAllAsync();
    }
}

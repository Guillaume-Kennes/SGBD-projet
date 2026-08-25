using PadelManager.Interfaces;
using PadelManager.Models.Dtos;

namespace PadelManager.Services;

public class StatistiqueService : IStatistiqueService {
    // Fenêtre glissante récente pour le taux d'occupation (EF-bk-016) : sur l'ensemble du
    // calendrier DISPONIBILITE (généré 2 ans à l'avance), le taux serait écrasé par la faible
    // proportion de créneaux déjà pris, sans rapport avec l'usage réel récent. 60 jours, contrairement
    // aux matchs publics/privés et aux membres actifs, qui restent sur l'ensemble des données.
    private const int JoursFenetreOccupation = 60;

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
        var paiements = await _statistiqueRepository.GetPaiementsAsync(siteId);

        var dateFin = DateOnly.FromDateTime(DateTime.Today);
        var dateDebut = dateFin.AddDays(-JoursFenetreOccupation);

        var resultat = new List<StatistiquesDto>();
        foreach (var site in sites) {
            var matchsDuSite = matchs.Where(m => m.SiteId == site.Id).ToList();
            var nbPublics = matchsDuSite.Count(m => m.Visibilite == "PUBLIC");
            var nbPrives = matchsDuSite.Count(m => m.Visibilite == "PRIVE");

            // Taux d'occupation : uniquement sur la fenêtre récente (matchs comme créneaux), pas
            // sur les comptes publics/privés ci-dessus qui restent sur l'ensemble des données.
            var matchsRecents = matchsDuSite.Count(m => {
                var date = DateOnly.FromDateTime(m.DateHeure);
                return date >= dateDebut && date <= dateFin;
            });
            var creneauxRecents = (await _disponibiliteRepository.GetBySiteAndPeriodeAsync(site.Id, dateDebut, dateFin)).Count;
            var nbTerrains = (await _terrainRepository.GetBySiteIdAsync(site.Id)).Count;
            var capacite = creneauxRecents * nbTerrains;
            var tauxOccupation = capacite > 0 ? (decimal)matchsRecents / capacite : 0m;

            // Membres actifs : uniquement une participation PAYÉE (jointure PAIEMENT) — une
            // participation impayée ne représente pas un membre ayant réellement joué, et ça rend
            // la stat indépendante du passage ou non du job de bascule sur les places impayées.
            var membresActifs = paiements
                .Where(p => p.Participation.Match.SiteId == site.Id)
                .Select(p => p.Participation.MembreMatricule)
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

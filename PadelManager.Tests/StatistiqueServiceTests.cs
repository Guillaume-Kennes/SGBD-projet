using Moq;
using PadelManager.Interfaces;
using PadelManager.Services;
using Xunit;

namespace PadelManager.Tests;

public class StatistiqueServiceTests {
    private readonly Mock<ISiteRepository> _siteRepoMock;
    private readonly Mock<ITerrainRepository> _terrainRepoMock;
    private readonly Mock<IMatchRepository> _matchRepoMock;
    private readonly Mock<IDisponibiliteRepository> _disponibiliteRepoMock;
    private readonly Mock<IStatistiqueRepository> _statistiqueRepoMock;
    private readonly StatistiqueService _service;

    public StatistiqueServiceTests() {
        _siteRepoMock = new Mock<ISiteRepository>();
        _terrainRepoMock = new Mock<ITerrainRepository>();
        _matchRepoMock = new Mock<IMatchRepository>();
        _disponibiliteRepoMock = new Mock<IDisponibiliteRepository>();
        _statistiqueRepoMock = new Mock<IStatistiqueRepository>();
        _service = new StatistiqueService(
            _siteRepoMock.Object, _terrainRepoMock.Object, _matchRepoMock.Object,
            _disponibiliteRepoMock.Object, _statistiqueRepoMock.Object);
    }

    private static Paiement PaiementPourSite(int matchId, int siteId, decimal montantParticipation, decimal montantDetteReportee) {
        var match = new Match { Id = matchId, SiteId = siteId, TerrainId = 11, DateHeure = DateTime.Now, Visibilite = "PUBLIC", OrganisateurMatricule = "G001", Statut = "TERMINE" };
        var participation = new Participation { MatchId = matchId, MembreMatricule = "G001", DateInscription = DateTime.Now, Match = match };
        return new Paiement {
            Participation = participation, MontantParticipation = montantParticipation, MontantDetteReportee = montantDetteReportee,
            MontantTotal = montantParticipation + montantDetteReportee, DatePaiement = DateTime.Now
        };
    }

    // --- ObtenirChiffreAffairesAsync (EF-bk-015) ---

    [Fact]
    public async Task ObtenirChiffreAffairesAsync_AvecSiteId_RetourneUnSeulSite() {
        _siteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Site { Id = 1, Nom = "Site 1" });
        _statistiqueRepoMock.Setup(r => r.GetPaiementsAsync(1)).ReturnsAsync(new List<Paiement> {
            PaiementPourSite(1, 1, 15.00m, 0.00m),
            PaiementPourSite(2, 1, 15.00m, 30.00m) // dette reportée incluse (R-CALC-005)
        });

        var resultat = await _service.ObtenirChiffreAffairesAsync(1);

        Assert.Single(resultat);
        Assert.Equal(1, resultat[0].SiteId);
        Assert.Equal("Site 1", resultat[0].NomSite);
        Assert.Equal(60.00m, resultat[0].Montant);
    }

    [Fact]
    public async Task ObtenirChiffreAffairesAsync_SiteInconnu_RetourneListeVide() {
        _siteRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Site?)null);
        _statistiqueRepoMock.Setup(r => r.GetPaiementsAsync(99)).ReturnsAsync(new List<Paiement>());

        var resultat = await _service.ObtenirChiffreAffairesAsync(99);

        Assert.Empty(resultat);
    }

    [Fact]
    public async Task ObtenirChiffreAffairesAsync_SansSiteId_RetourneTousLesSitesYComprisA0() {
        _siteRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Site> {
            new() { Id = 1, Nom = "Site 1" },
            new() { Id = 2, Nom = "Site 2" } // aucun paiement -> doit apparaître à 0€
        });
        _statistiqueRepoMock.Setup(r => r.GetPaiementsAsync(null)).ReturnsAsync(new List<Paiement> {
            PaiementPourSite(1, 1, 15.00m, 0.00m)
        });

        var resultat = await _service.ObtenirChiffreAffairesAsync(null);

        Assert.Equal(2, resultat.Count);
        Assert.Equal(15.00m, resultat.Single(d => d.SiteId == 1).Montant);
        Assert.Equal(0.00m, resultat.Single(d => d.SiteId == 2).Montant);
    }

    // --- ObtenirStatistiquesAsync (EF-bk-016) ---

    // Toujours dans la fenêtre des 60 jours (par défaut DateTime.Now), sauf précision contraire.
    private static Match MatchPourSite(int matchId, int siteId, string visibilite, DateTime? dateHeure = null) {
        return new Match {
            Id = matchId, SiteId = siteId, TerrainId = 11, DateHeure = dateHeure ?? DateTime.Now,
            Visibilite = visibilite, OrganisateurMatricule = "G001", Statut = "INCOMPLET"
        };
    }

    private static List<Disponibilite> DisponibilitesPourSite(int siteId, int nombre) {
        return Enumerable.Range(0, nombre)
            .Select(i => new Disponibilite {
                SiteId = siteId, Date = DateOnly.FromDateTime(DateTime.Today),
                HeureDebut = new TimeOnly(8, 0), HeureFin = new TimeOnly(9, 30)
            })
            .ToList();
    }

    // Variante de PaiementPourSite avec matricule paramétrable, pour les tests de membres actifs.
    private static Paiement PaiementPourSiteEtMembre(int matchId, int siteId, string membre) {
        var match = new Match { Id = matchId, SiteId = siteId, TerrainId = 11, DateHeure = DateTime.Now, Visibilite = "PUBLIC", OrganisateurMatricule = "G001", Statut = "INCOMPLET" };
        var participation = new Participation { MatchId = matchId, MembreMatricule = membre, DateInscription = DateTime.Now, Match = match };
        return new Paiement {
            Participation = participation, MontantParticipation = 15.00m, MontantDetteReportee = 0.00m,
            MontantTotal = 15.00m, DatePaiement = DateTime.Now
        };
    }

    [Fact]
    public async Task ObtenirStatistiquesAsync_ComptePublicsEtPrivesSeparement() {
        _siteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Site { Id = 1, Nom = "Site 1" });
        _matchRepoMock.Setup(r => r.GetTousLesMatchsAsync(1)).ReturnsAsync(new List<Match> {
            MatchPourSite(1, 1, "PUBLIC"),
            MatchPourSite(2, 1, "PUBLIC"),
            MatchPourSite(3, 1, "PRIVE")
        });
        _disponibiliteRepoMock.Setup(r => r.GetBySiteAndPeriodeAsync(1, It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).ReturnsAsync(DisponibilitesPourSite(1, 100));
        _terrainRepoMock.Setup(r => r.GetBySiteIdAsync(1)).ReturnsAsync(new List<Terrain> { new() { Id = 11, SiteId = 1, Numero = 1 } });
        _statistiqueRepoMock.Setup(r => r.GetPaiementsAsync(1)).ReturnsAsync(new List<Paiement>());

        var resultat = await _service.ObtenirStatistiquesAsync(1);

        Assert.Single(resultat);
        Assert.Equal(2, resultat[0].NombreMatchsPublics);
        Assert.Equal(1, resultat[0].NombreMatchsPrives);
    }

    [Fact]
    public async Task ObtenirStatistiquesAsync_CalculeLeTauxDOccupation() {
        // 3 matchs récents / (10 créneaux x 2 terrains = 20) = 0.15
        _siteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Site { Id = 1, Nom = "Site 1" });
        _matchRepoMock.Setup(r => r.GetTousLesMatchsAsync(1)).ReturnsAsync(new List<Match> {
            MatchPourSite(1, 1, "PUBLIC"),
            MatchPourSite(2, 1, "PUBLIC"),
            MatchPourSite(3, 1, "PRIVE")
        });
        _disponibiliteRepoMock.Setup(r => r.GetBySiteAndPeriodeAsync(1, It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).ReturnsAsync(DisponibilitesPourSite(1, 10));
        _terrainRepoMock.Setup(r => r.GetBySiteIdAsync(1)).ReturnsAsync(new List<Terrain> {
            new() { Id = 11, SiteId = 1, Numero = 1 }, new() { Id = 12, SiteId = 1, Numero = 2 }
        });
        _statistiqueRepoMock.Setup(r => r.GetPaiementsAsync(1)).ReturnsAsync(new List<Paiement>());

        var resultat = await _service.ObtenirStatistiquesAsync(1);

        Assert.Equal(0.15m, resultat[0].TauxOccupation);
    }

    [Fact]
    public async Task ObtenirStatistiquesAsync_ExclutLesMatchsHorsFenetreDuTauxMaisPasDesComptesPublicsPrives() {
        // Un match vieux de 100 jours (hors fenêtre de 60j) compte pour publics/privés, mais pas
        // pour le taux d'occupation (seul le match récent doit y contribuer).
        _siteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Site { Id = 1, Nom = "Site 1" });
        _matchRepoMock.Setup(r => r.GetTousLesMatchsAsync(1)).ReturnsAsync(new List<Match> {
            MatchPourSite(1, 1, "PUBLIC", DateTime.Today.AddDays(-100)), // hors fenêtre
            MatchPourSite(2, 1, "PUBLIC") // récent
        });
        _disponibiliteRepoMock.Setup(r => r.GetBySiteAndPeriodeAsync(1, It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).ReturnsAsync(DisponibilitesPourSite(1, 10));
        _terrainRepoMock.Setup(r => r.GetBySiteIdAsync(1)).ReturnsAsync(new List<Terrain> { new() { Id = 11, SiteId = 1, Numero = 1 } });
        _statistiqueRepoMock.Setup(r => r.GetPaiementsAsync(1)).ReturnsAsync(new List<Paiement>());

        var resultat = await _service.ObtenirStatistiquesAsync(1);

        Assert.Equal(2, resultat[0].NombreMatchsPublics); // les deux comptent
        Assert.Equal(0.10m, resultat[0].TauxOccupation); // seul le récent (1 / 10)
    }

    [Fact]
    public async Task ObtenirStatistiquesAsync_AucunCreneauOuTerrain_TauxAZeroSansException() {
        _siteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Site { Id = 1, Nom = "Site 1" });
        _matchRepoMock.Setup(r => r.GetTousLesMatchsAsync(1)).ReturnsAsync(new List<Match>());
        _disponibiliteRepoMock.Setup(r => r.GetBySiteAndPeriodeAsync(1, It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).ReturnsAsync(new List<Disponibilite>());
        _terrainRepoMock.Setup(r => r.GetBySiteIdAsync(1)).ReturnsAsync(new List<Terrain>());
        _statistiqueRepoMock.Setup(r => r.GetPaiementsAsync(1)).ReturnsAsync(new List<Paiement>());

        var resultat = await _service.ObtenirStatistiquesAsync(1);

        Assert.Equal(0m, resultat[0].TauxOccupation);
    }

    [Fact]
    public async Task ObtenirStatistiquesAsync_CompteLesMembresActifsDistinctsUniquementSiPaye() {
        // Seuls les paiements (jointure PARTICIPATION -> PAIEMENT) comptent — une participation
        // impayée n'apparaît jamais dans GetPaiementsAsync, donc ne peut pas gonfler ce compte.
        _siteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Site { Id = 1, Nom = "Site 1" });
        _matchRepoMock.Setup(r => r.GetTousLesMatchsAsync(1)).ReturnsAsync(new List<Match>());
        _disponibiliteRepoMock.Setup(r => r.GetBySiteAndPeriodeAsync(1, It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).ReturnsAsync(DisponibilitesPourSite(1, 10));
        _terrainRepoMock.Setup(r => r.GetBySiteIdAsync(1)).ReturnsAsync(new List<Terrain> { new() { Id = 11, SiteId = 1, Numero = 1 } });
        _statistiqueRepoMock.Setup(r => r.GetPaiementsAsync(1)).ReturnsAsync(new List<Paiement> {
            PaiementPourSiteEtMembre(1, 1, "G001"),
            PaiementPourSiteEtMembre(1, 1, "G001"), // même membre, même match compté une fois -> distinct
            PaiementPourSiteEtMembre(2, 1, "L001")
            // G002 aurait une participation impayée : absente de GetPaiementsAsync, donc pas comptée.
        });

        var resultat = await _service.ObtenirStatistiquesAsync(1);

        Assert.Equal(2, resultat[0].MembresActifs);
    }

    [Fact]
    public async Task ObtenirStatistiquesAsync_SansSiteId_RetourneTousLesSites() {
        _siteRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Site> {
            new() { Id = 1, Nom = "Site 1" },
            new() { Id = 2, Nom = "Site 2" }
        });
        _matchRepoMock.Setup(r => r.GetTousLesMatchsAsync(null)).ReturnsAsync(new List<Match>());
        _disponibiliteRepoMock.Setup(r => r.GetBySiteAndPeriodeAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).ReturnsAsync(DisponibilitesPourSite(1, 10));
        _terrainRepoMock.Setup(r => r.GetBySiteIdAsync(It.IsAny<int>())).ReturnsAsync(new List<Terrain> { new() { Id = 11, SiteId = 1, Numero = 1 } });
        _statistiqueRepoMock.Setup(r => r.GetPaiementsAsync(null)).ReturnsAsync(new List<Paiement>());

        var resultat = await _service.ObtenirStatistiquesAsync(null);

        Assert.Equal(2, resultat.Count);
    }
}

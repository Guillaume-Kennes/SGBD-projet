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

    private static Participation ParticipationPourSite(int matchId, int siteId, string membre) {
        var match = new Match { Id = matchId, SiteId = siteId, TerrainId = 11, DateHeure = DateTime.Now, Visibilite = "PUBLIC", OrganisateurMatricule = "G001", Statut = "INCOMPLET" };
        return new Participation { MatchId = matchId, MembreMatricule = membre, DateInscription = DateTime.Now, Match = match };
    }

    [Fact]
    public async Task ObtenirStatistiquesAsync_ComptePublicsEtPrivesSeparement() {
        _siteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Site { Id = 1, Nom = "Site 1" });
        _matchRepoMock.Setup(r => r.GetTousLesMatchsAsync(1)).ReturnsAsync(new List<Match> {
            new() { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now, Visibilite = "PUBLIC", OrganisateurMatricule = "G001", Statut = "INCOMPLET" },
            new() { Id = 2, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now, Visibilite = "PUBLIC", OrganisateurMatricule = "G001", Statut = "INCOMPLET" },
            new() { Id = 3, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now, Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" }
        });
        _disponibiliteRepoMock.Setup(r => r.CountBySiteAsync(1)).ReturnsAsync(100);
        _terrainRepoMock.Setup(r => r.GetBySiteIdAsync(1)).ReturnsAsync(new List<Terrain> { new() { Id = 11, SiteId = 1, Numero = 1 } });
        _statistiqueRepoMock.Setup(r => r.GetParticipationsAsync(1)).ReturnsAsync(new List<Participation>());

        var resultat = await _service.ObtenirStatistiquesAsync(1);

        Assert.Single(resultat);
        Assert.Equal(2, resultat[0].NombreMatchsPublics);
        Assert.Equal(1, resultat[0].NombreMatchsPrives);
    }

    [Fact]
    public async Task ObtenirStatistiquesAsync_CalculeLeTauxDOccupation() {
        // 3 matchs / (10 créneaux x 2 terrains = 20) = 0.15
        _siteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Site { Id = 1, Nom = "Site 1" });
        _matchRepoMock.Setup(r => r.GetTousLesMatchsAsync(1)).ReturnsAsync(new List<Match> {
            new() { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now, Visibilite = "PUBLIC", OrganisateurMatricule = "G001", Statut = "INCOMPLET" },
            new() { Id = 2, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now, Visibilite = "PUBLIC", OrganisateurMatricule = "G001", Statut = "INCOMPLET" },
            new() { Id = 3, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now, Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" }
        });
        _disponibiliteRepoMock.Setup(r => r.CountBySiteAsync(1)).ReturnsAsync(10);
        _terrainRepoMock.Setup(r => r.GetBySiteIdAsync(1)).ReturnsAsync(new List<Terrain> {
            new() { Id = 11, SiteId = 1, Numero = 1 }, new() { Id = 12, SiteId = 1, Numero = 2 }
        });
        _statistiqueRepoMock.Setup(r => r.GetParticipationsAsync(1)).ReturnsAsync(new List<Participation>());

        var resultat = await _service.ObtenirStatistiquesAsync(1);

        Assert.Equal(0.15m, resultat[0].TauxOccupation);
    }

    [Fact]
    public async Task ObtenirStatistiquesAsync_AucunCreneauOuTerrain_TauxAZeroSansException() {
        _siteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Site { Id = 1, Nom = "Site 1" });
        _matchRepoMock.Setup(r => r.GetTousLesMatchsAsync(1)).ReturnsAsync(new List<Match>());
        _disponibiliteRepoMock.Setup(r => r.CountBySiteAsync(1)).ReturnsAsync(0);
        _terrainRepoMock.Setup(r => r.GetBySiteIdAsync(1)).ReturnsAsync(new List<Terrain>());
        _statistiqueRepoMock.Setup(r => r.GetParticipationsAsync(1)).ReturnsAsync(new List<Participation>());

        var resultat = await _service.ObtenirStatistiquesAsync(1);

        Assert.Equal(0m, resultat[0].TauxOccupation);
    }

    [Fact]
    public async Task ObtenirStatistiquesAsync_CompteLesMembresActifsDistinctsPeuImportePaye() {
        _siteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Site { Id = 1, Nom = "Site 1" });
        _matchRepoMock.Setup(r => r.GetTousLesMatchsAsync(1)).ReturnsAsync(new List<Match>());
        _disponibiliteRepoMock.Setup(r => r.CountBySiteAsync(1)).ReturnsAsync(10);
        _terrainRepoMock.Setup(r => r.GetBySiteIdAsync(1)).ReturnsAsync(new List<Terrain> { new() { Id = 11, SiteId = 1, Numero = 1 } });
        _statistiqueRepoMock.Setup(r => r.GetParticipationsAsync(1)).ReturnsAsync(new List<Participation> {
            ParticipationPourSite(1, 1, "G001"),
            ParticipationPourSite(1, 1, "G001"), // même membre, même match compté une fois -> distinct
            ParticipationPourSite(2, 1, "L001")
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
        _disponibiliteRepoMock.Setup(r => r.CountBySiteAsync(It.IsAny<int>())).ReturnsAsync(10);
        _terrainRepoMock.Setup(r => r.GetBySiteIdAsync(It.IsAny<int>())).ReturnsAsync(new List<Terrain> { new() { Id = 11, SiteId = 1, Numero = 1 } });
        _statistiqueRepoMock.Setup(r => r.GetParticipationsAsync(null)).ReturnsAsync(new List<Participation>());

        var resultat = await _service.ObtenirStatistiquesAsync(null);

        Assert.Equal(2, resultat.Count);
    }
}

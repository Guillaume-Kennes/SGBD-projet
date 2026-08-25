using Moq;
using PadelManager.Interfaces;
using PadelManager.Services;
using Xunit;

namespace PadelManager.Tests;

public class StatistiqueServiceTests {
    private readonly Mock<ISiteRepository> _siteRepoMock;
    private readonly Mock<IStatistiqueRepository> _statistiqueRepoMock;
    private readonly StatistiqueService _service;

    public StatistiqueServiceTests() {
        _siteRepoMock = new Mock<ISiteRepository>();
        _statistiqueRepoMock = new Mock<IStatistiqueRepository>();
        _service = new StatistiqueService(_siteRepoMock.Object, _statistiqueRepoMock.Object);
    }

    private static Paiement PaiementPourSite(int matchId, int siteId, decimal montantParticipation, decimal montantDetteReportee) {
        var match = new Match { Id = matchId, SiteId = siteId, TerrainId = 11, DateHeure = DateTime.Now, Visibilite = "PUBLIC", OrganisateurMatricule = "G001", Statut = "TERMINE" };
        var participation = new Participation { MatchId = matchId, MembreMatricule = "G001", DateInscription = DateTime.Now, Match = match };
        return new Paiement {
            Participation = participation, MontantParticipation = montantParticipation, MontantDetteReportee = montantDetteReportee,
            MontantTotal = montantParticipation + montantDetteReportee, DatePaiement = DateTime.Now
        };
    }

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
}

using Moq;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;
using PadelManager.Services;
using Xunit;

namespace PadelManager.Tests;

public class JourFermetureServiceTests {
    private readonly Mock<ISiteRepository> _siteRepoMock;
    private readonly Mock<IJourFermetureRepository> _jourFermetureRepoMock;
    private readonly Mock<IDisponibiliteGenerationService> _generationServiceMock;
    private readonly JourFermetureService _service;

    public JourFermetureServiceTests() {
        _siteRepoMock = new Mock<ISiteRepository>();
        _jourFermetureRepoMock = new Mock<IJourFermetureRepository>();
        _generationServiceMock = new Mock<IDisponibiliteGenerationService>();
        _service = new JourFermetureService(_siteRepoMock.Object, _jourFermetureRepoMock.Object, _generationServiceMock.Object);

        _siteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Site { Id = 1, Nom = "Site 1" });
        _jourFermetureRepoMock.Setup(r => r.ExisteAsync(It.IsAny<int?>(), It.IsAny<DateOnly>())).ReturnsAsync(false);
        _jourFermetureRepoMock.Setup(r => r.AddAsync(It.IsAny<JourFermeture>()))
            .ReturnsAsync((JourFermeture j) => { j.Id = 42; return j; });
    }

    [Fact]
    public async Task DeclarerAsync_SiteDonne_EnregistreEtRegenereCeSite() {
        // Act
        var resultat = await _service.DeclarerAsync(new JourFermetureRequestDto { SiteId = 1, Date = new DateOnly(2026, 12, 24) });

        // Assert
        Assert.True(resultat.Succes);
        Assert.NotNull(resultat.Fermeture);
        Assert.Equal(1, resultat.Fermeture!.SiteId);

        _jourFermetureRepoMock.Verify(r => r.AddAsync(It.Is<JourFermeture>(j => j.SiteId == 1 && j.Date == new DateOnly(2026, 12, 24))), Times.Once);
        _generationServiceMock.Verify(g => g.GenererPourSiteEtAnneeAsync(1, 2026), Times.Once);
        _generationServiceMock.Verify(g => g.GenererPourTousLesSitesEtAnneeAsync(It.IsAny<short>()), Times.Never);
    }

    [Fact]
    public async Task DeclarerAsync_FermetureGlobale_EnregistreEtRegenereTousLesSites() {
        // Act
        var resultat = await _service.DeclarerAsync(new JourFermetureRequestDto { SiteId = null, Date = new DateOnly(2026, 12, 25) });

        // Assert
        Assert.True(resultat.Succes);
        Assert.Null(resultat.Fermeture!.SiteId);

        _generationServiceMock.Verify(g => g.GenererPourTousLesSitesEtAnneeAsync(2026), Times.Once);
        _generationServiceMock.Verify(g => g.GenererPourSiteEtAnneeAsync(It.IsAny<int>(), It.IsAny<short>()), Times.Never);
    }

    [Fact]
    public async Task DeclarerAsync_SiteInconnu_RetourneEchecSansEnregistrer() {
        // Arrange
        _siteRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Site?)null);

        // Act
        var resultat = await _service.DeclarerAsync(new JourFermetureRequestDto { SiteId = 99, Date = new DateOnly(2026, 12, 24) });

        // Assert
        Assert.False(resultat.Succes);
        _jourFermetureRepoMock.Verify(r => r.AddAsync(It.IsAny<JourFermeture>()), Times.Never);
        _generationServiceMock.Verify(g => g.GenererPourSiteEtAnneeAsync(It.IsAny<int>(), It.IsAny<short>()), Times.Never);
    }

    [Fact]
    public async Task DeclarerAsync_DejaDeclaree_RetourneEchec() {
        // Arrange
        _jourFermetureRepoMock.Setup(r => r.ExisteAsync(1, new DateOnly(2026, 12, 24))).ReturnsAsync(true);

        // Act
        var resultat = await _service.DeclarerAsync(new JourFermetureRequestDto { SiteId = 1, Date = new DateOnly(2026, 12, 24) });

        // Assert
        Assert.False(resultat.Succes);
        _jourFermetureRepoMock.Verify(r => r.AddAsync(It.IsAny<JourFermeture>()), Times.Never);
    }

    [Fact]
    public async Task SupprimerAsync_FermetureSiteExistante_SupprimeEtRegenereCeSite() {
        // Arrange
        _jourFermetureRepoMock.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(new JourFermeture { Id = 5, SiteId = 1, Date = new DateOnly(2026, 12, 24) });

        // Act
        var resultat = await _service.SupprimerAsync(5);

        // Assert
        Assert.True(resultat);
        _jourFermetureRepoMock.Verify(r => r.DeleteAsync(5), Times.Once);
        _generationServiceMock.Verify(g => g.GenererPourSiteEtAnneeAsync(1, 2026), Times.Once);
    }

    [Fact]
    public async Task SupprimerAsync_FermetureGlobaleExistante_SupprimeEtRegenereTousLesSites() {
        // Arrange
        _jourFermetureRepoMock.Setup(r => r.GetByIdAsync(6))
            .ReturnsAsync(new JourFermeture { Id = 6, SiteId = null, Date = new DateOnly(2026, 12, 25) });

        // Act
        var resultat = await _service.SupprimerAsync(6);

        // Assert
        Assert.True(resultat);
        _generationServiceMock.Verify(g => g.GenererPourTousLesSitesEtAnneeAsync(2026), Times.Once);
    }

    [Fact]
    public async Task SupprimerAsync_Inconnue_RetourneFauxSansRegenerer() {
        // Arrange
        _jourFermetureRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((JourFermeture?)null);

        // Act
        var resultat = await _service.SupprimerAsync(999);

        // Assert
        Assert.False(resultat);
        _jourFermetureRepoMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        _generationServiceMock.Verify(g => g.GenererPourSiteEtAnneeAsync(It.IsAny<int>(), It.IsAny<short>()), Times.Never);
        _generationServiceMock.Verify(g => g.GenererPourTousLesSitesEtAnneeAsync(It.IsAny<short>()), Times.Never);
    }

    [Fact]
    public async Task ObtenirPourSiteEtAnneeAsync_DelegueAuRepositoryEtMappe() {
        // Arrange
        _jourFermetureRepoMock.Setup(r => r.GetForSiteAndAnneeAsync(1, 2026)).ReturnsAsync(new List<JourFermeture> {
            new() { Id = 1, SiteId = 1, Date = new DateOnly(2026, 12, 24) },
            new() { Id = 2, SiteId = null, Date = new DateOnly(2026, 12, 25) }
        });

        // Act
        var resultat = await _service.ObtenirPourSiteEtAnneeAsync(1, 2026);

        // Assert
        Assert.Equal(2, resultat.Count);
        Assert.Contains(resultat, f => f.SiteId == null && f.Date == new DateOnly(2026, 12, 25));
    }
}

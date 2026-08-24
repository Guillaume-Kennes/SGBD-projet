using Microsoft.AspNetCore.Mvc;
using Moq;
using PadelManager.Api.Controllers;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;
using Xunit;

namespace PadelManager.Tests;

public class DisponibiliteControllerTests {
    private readonly Mock<ISiteService> _siteServiceMock;
    private readonly Mock<IDisponibiliteGenerationService> _generationServiceMock;
    private readonly DisponibiliteController _controller;

    public DisponibiliteControllerTests() {
        _siteServiceMock = new Mock<ISiteService>();
        _generationServiceMock = new Mock<IDisponibiliteGenerationService>();
        _controller = new DisponibiliteController(_siteServiceMock.Object, _generationServiceMock.Object);

        _siteServiceMock.Setup(s => s.ObtenirParIdAsync(1)).ReturnsAsync(new SiteDto { Id = 1, Nom = "Site 1" });
    }

    [Fact]
    public async Task Generer_HoraireConfigure_RetourneOkAvecLeCompte() {
        // Arrange
        _generationServiceMock.Setup(s => s.GenererPourSiteEtAnneeAsync(1, 2026)).ReturnsAsync(42);

        // Act
        var resultat = await _controller.Generer(1, 2026);

        // Assert
        Assert.IsType<OkObjectResult>(resultat);
    }

    [Fact]
    public async Task Generer_AucunHoraireConfigure_RetourneNotFound() {
        // Arrange
        _generationServiceMock.Setup(s => s.GenererPourSiteEtAnneeAsync(1, 2026)).ReturnsAsync((int?)null);

        // Act
        var resultat = await _controller.Generer(1, 2026);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultat);
    }

    [Fact]
    public async Task Generer_SiteInconnu_RetourneNotFoundSansAppelerLaGeneration() {
        // Arrange
        _siteServiceMock.Setup(s => s.ObtenirParIdAsync(99)).ReturnsAsync((SiteDto?)null);

        // Act
        var resultat = await _controller.Generer(99, 2026);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultat);
        _generationServiceMock.Verify(g => g.GenererPourSiteEtAnneeAsync(It.IsAny<int>(), It.IsAny<short>()), Times.Never);
    }
}

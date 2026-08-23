using Microsoft.AspNetCore.Mvc;
using Moq;
using PadelManager.Api.Controllers;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;
using Xunit;

namespace PadelManager.Tests;

public class DisponibiliteControllerTests {
    private readonly Mock<ISiteService> _siteServiceMock;
    private readonly Mock<IDisponibiliteService> _disponibiliteServiceMock;
    private readonly Mock<IDisponibiliteGenerationService> _generationServiceMock;
    private readonly DisponibiliteController _controller;

    public DisponibiliteControllerTests() {
        _siteServiceMock = new Mock<ISiteService>();
        _disponibiliteServiceMock = new Mock<IDisponibiliteService>();
        _generationServiceMock = new Mock<IDisponibiliteGenerationService>();
        _controller = new DisponibiliteController(_siteServiceMock.Object, _disponibiliteServiceMock.Object, _generationServiceMock.Object);

        _siteServiceMock.Setup(s => s.ObtenirParIdAsync(1)).ReturnsAsync(new SiteDto { Id = 1, Nom = "Site 1" });
    }

    [Fact]
    public async Task ConsulterPlanning_SiteConnu_RetourneOk() {
        // Arrange
        var from = new DateOnly(2026, 1, 1);
        var to = new DateOnly(2026, 1, 31);
        var disponibilites = new List<DisponibiliteDto> { new() { SiteId = 1, Date = from } };
        _disponibiliteServiceMock.Setup(s => s.ConsulterPlanningAsync(1, from, to)).ReturnsAsync(disponibilites);

        // Act
        var resultat = await _controller.ConsulterPlanning(1, from, to);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(disponibilites, okResult.Value);
    }

    [Fact]
    public async Task ConsulterPlanning_SiteInconnu_RetourneNotFound() {
        // Arrange
        var from = new DateOnly(2026, 1, 1);
        var to = new DateOnly(2026, 1, 31);
        _disponibiliteServiceMock.Setup(s => s.ConsulterPlanningAsync(99, from, to)).ReturnsAsync((List<DisponibiliteDto>?)null);

        // Act
        var resultat = await _controller.ConsulterPlanning(99, from, to);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultat);
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

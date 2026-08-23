using Microsoft.AspNetCore.Mvc;
using Moq;
using PadelManager.Api.Controllers;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;
using Xunit;

namespace PadelManager.Tests;

public class JourFermetureControllerTests {
    private readonly Mock<IJourFermetureService> _serviceMock;
    private readonly JourFermetureController _controller;

    public JourFermetureControllerTests() {
        _serviceMock = new Mock<IJourFermetureService>();
        _controller = new JourFermetureController(_serviceMock.Object);
    }

    [Fact]
    public async Task ObtenirPourSite_RetourneOkAvecLaListe() {
        // Arrange
        var liste = new List<JourFermetureDto> { new() { Id = 1, SiteId = 1, Date = new DateOnly(2026, 12, 24) } };
        _serviceMock.Setup(s => s.ObtenirPourSiteEtAnneeAsync(1, 2026)).ReturnsAsync(liste);

        // Act
        var resultat = await _controller.ObtenirPourSite(1, 2026);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(liste, okResult.Value);
    }

    [Fact]
    public async Task Declarer_RequeteValide_RetourneOk() {
        // Arrange
        var requete = new JourFermetureRequestDto { SiteId = 1, Date = new DateOnly(2026, 12, 24) };
        var dto = new JourFermetureDto { Id = 1, SiteId = 1, Date = requete.Date };
        _serviceMock.Setup(s => s.DeclarerAsync(requete))
            .ReturnsAsync(new DeclarerFermetureResultatDto { Succes = true, Fermeture = dto });

        // Act
        var resultat = await _controller.Declarer(requete);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    public async Task Declarer_RequeteInvalide_RetourneBadRequest() {
        // Arrange
        var requete = new JourFermetureRequestDto { SiteId = 99, Date = new DateOnly(2026, 12, 24) };
        _serviceMock.Setup(s => s.DeclarerAsync(requete))
            .ReturnsAsync(new DeclarerFermetureResultatDto { Succes = false, MessageErreur = "Site introuvable." });

        // Act
        var resultat = await _controller.Declarer(requete);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultat);
    }

    [Fact]
    public async Task Supprimer_FermetureExistante_RetourneNoContent() {
        // Arrange
        _serviceMock.Setup(s => s.SupprimerAsync(5)).ReturnsAsync(true);

        // Act
        var resultat = await _controller.Supprimer(5);

        // Assert
        Assert.IsType<NoContentResult>(resultat);
    }

    [Fact]
    public async Task Supprimer_FermetureInexistante_RetourneNotFound() {
        // Arrange
        _serviceMock.Setup(s => s.SupprimerAsync(999)).ReturnsAsync(false);

        // Act
        var resultat = await _controller.Supprimer(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultat);
    }
}

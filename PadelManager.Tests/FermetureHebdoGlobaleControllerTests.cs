using Microsoft.AspNetCore.Mvc;
using Moq;
using PadelManager.Api.Controllers;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;
using Xunit;

namespace PadelManager.Tests;

public class FermetureHebdoGlobaleControllerTests {
    private readonly Mock<IFermetureHebdoGlobaleService> _serviceMock;
    private readonly FermetureHebdoGlobaleController _controller;

    public FermetureHebdoGlobaleControllerTests() {
        _serviceMock = new Mock<IFermetureHebdoGlobaleService>();
        _controller = new FermetureHebdoGlobaleController(_serviceMock.Object);
    }

    [Fact]
    public async Task Obtenir_FermetureExistante_RetourneOk() {
        // Arrange
        var dto = new FermetureHebdoGlobaleDto { Annee = 2026, JoursFermes = new List<string> { "LUN" } };
        _serviceMock.Setup(s => s.ObtenirAsync(2026)).ReturnsAsync(dto);

        // Act
        var resultat = await _controller.Obtenir(2026);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    public async Task Obtenir_Aucune_RetourneNotFound() {
        // Arrange
        _serviceMock.Setup(s => s.ObtenirAsync(2026)).ReturnsAsync((FermetureHebdoGlobaleDto?)null);

        // Act
        var resultat = await _controller.Obtenir(2026);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultat);
    }

    [Fact]
    public async Task Definir_RequeteValide_RetourneOk() {
        // Arrange
        var requete = new FermetureHebdoGlobaleRequestDto { JoursFermes = new List<string> { "LUN" } };
        var dto = new FermetureHebdoGlobaleDto { Annee = 2026, JoursFermes = requete.JoursFermes };
        _serviceMock.Setup(s => s.DefinirAsync(2026, requete))
            .ReturnsAsync(new DefinirFermetureHebdoGlobaleResultatDto { Succes = true, Fermeture = dto });

        // Act
        var resultat = await _controller.Definir(2026, requete);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    public async Task Definir_RequeteInvalide_RetourneBadRequest() {
        // Arrange
        var requete = new FermetureHebdoGlobaleRequestDto();
        _serviceMock.Setup(s => s.DefinirAsync(2026, requete))
            .ReturnsAsync(new DefinirFermetureHebdoGlobaleResultatDto { Succes = false, MessageErreur = "Veuillez sélectionner au moins un jour fermé." });

        // Act
        var resultat = await _controller.Definir(2026, requete);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultat);
    }

    [Fact]
    public async Task Supprimer_AnneeExistante_RetourneNoContent() {
        // Arrange
        _serviceMock.Setup(s => s.SupprimerAsync(2026)).ReturnsAsync(true);

        // Act
        var resultat = await _controller.Supprimer(2026);

        // Assert
        Assert.IsType<NoContentResult>(resultat);
    }

    [Fact]
    public async Task Supprimer_AnneeInexistante_RetourneNotFound() {
        // Arrange
        _serviceMock.Setup(s => s.SupprimerAsync(2026)).ReturnsAsync(false);

        // Act
        var resultat = await _controller.Supprimer(2026);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultat);
    }
}

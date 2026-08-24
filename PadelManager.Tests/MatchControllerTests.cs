using Microsoft.AspNetCore.Mvc;
using Moq;
using PadelManager.Api.Controllers;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;
using Xunit;

namespace PadelManager.Tests;

public class MatchControllerTests {
    private readonly Mock<IMatchService> _serviceMock;
    private readonly MatchController _controller;

    public MatchControllerTests() {
        _serviceMock = new Mock<IMatchService>();
        _controller = new MatchController(_serviceMock.Object);
    }

    [Fact]
    public async Task ObtenirCreneauxDisponibles_SiteExistant_RetourneOk() {
        // Arrange
        var date = new DateOnly(2026, 1, 5);
        var creneaux = new List<CreneauMatchDto> { new() { TerrainId = 11, NumeroTerrain = 1, HeureDebut = new TimeOnly(9, 0), HeureFin = new TimeOnly(10, 30) } };
        _serviceMock.Setup(s => s.ObtenirCreneauxDisponiblesAsync(1, date)).ReturnsAsync(creneaux);

        // Act
        var resultat = await _controller.ObtenirCreneauxDisponibles(1, date);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(creneaux, okResult.Value);
    }

    [Fact]
    public async Task ObtenirCreneauxDisponibles_SiteInconnu_RetourneNotFound() {
        // Arrange
        var date = new DateOnly(2026, 1, 5);
        _serviceMock.Setup(s => s.ObtenirCreneauxDisponiblesAsync(99, date)).ReturnsAsync((List<CreneauMatchDto>?)null);

        // Act
        var resultat = await _controller.ObtenirCreneauxDisponibles(99, date);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultat);
    }

    [Fact]
    public async Task CreerPrive_RequeteValide_RetourneOk() {
        // Arrange
        var requete = new CreerMatchPriveRequestDto { OrganisateurMatricule = "G0001", SiteId = 1, TerrainId = 11, Date = new DateOnly(2026, 1, 5), HeureDebut = new TimeOnly(9, 0) };
        var dto = new MatchDto { Id = 1, SiteId = 1, TerrainId = 11, OrganisateurMatricule = "G0001", Statut = "INCOMPLET", Visibilite = "PRIVE" };
        _serviceMock.Setup(s => s.CreerMatchPriveAsync(requete)).ReturnsAsync(new CreerMatchResultatDto { Succes = true, Match = dto });

        // Act
        var resultat = await _controller.CreerPrive(requete);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    public async Task CreerPrive_RequeteInvalide_RetourneBadRequest() {
        // Arrange
        var requete = new CreerMatchPriveRequestDto { OrganisateurMatricule = "XXXX", SiteId = 1, TerrainId = 11, Date = new DateOnly(2026, 1, 5), HeureDebut = new TimeOnly(9, 0) };
        _serviceMock.Setup(s => s.CreerMatchPriveAsync(requete)).ReturnsAsync(new CreerMatchResultatDto { Succes = false, MessageErreur = "Organisateur introuvable." });

        // Act
        var resultat = await _controller.CreerPrive(requete);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultat);
    }

    [Fact]
    public async Task CreerPublic_RequeteValide_RetourneOk() {
        // Arrange
        var requete = new CreerMatchPublicRequestDto { OrganisateurMatricule = "G0001", SiteId = 1, TerrainId = 11, Date = new DateOnly(2026, 1, 5), HeureDebut = new TimeOnly(9, 0) };
        var dto = new MatchDto { Id = 1, SiteId = 1, TerrainId = 11, OrganisateurMatricule = "G0001", Statut = "INCOMPLET", Visibilite = "PUBLIC" };
        _serviceMock.Setup(s => s.CreerMatchPublicAsync(requete)).ReturnsAsync(new CreerMatchResultatDto { Succes = true, Match = dto });

        // Act
        var resultat = await _controller.CreerPublic(requete);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    public async Task CreerPublic_RequeteInvalide_RetourneBadRequest() {
        // Arrange
        var requete = new CreerMatchPublicRequestDto { OrganisateurMatricule = "XXXX", SiteId = 1, TerrainId = 11, Date = new DateOnly(2026, 1, 5), HeureDebut = new TimeOnly(9, 0) };
        _serviceMock.Setup(s => s.CreerMatchPublicAsync(requete)).ReturnsAsync(new CreerMatchResultatDto { Succes = false, MessageErreur = "Organisateur introuvable." });

        // Act
        var resultat = await _controller.CreerPublic(requete);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultat);
    }
}

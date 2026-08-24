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

    [Fact]
    public async Task ObtenirPublics_MembreConnu_RetourneOk() {
        // Arrange
        var matchs = new List<MatchPublicDto> { new() { Id = 1, SiteId = 1, NomSite = "Site 1", TerrainId = 11, NumeroTerrain = 1, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), PlacesRestantes = 2 } };
        _serviceMock.Setup(s => s.ObtenirMatchsPublicsAsync("G0001")).ReturnsAsync(matchs);

        // Act
        var resultat = await _controller.ObtenirPublics("G0001");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(matchs, okResult.Value);
    }

    [Fact]
    public async Task ObtenirPublics_MembreInconnu_RetourneNotFound() {
        // Arrange
        _serviceMock.Setup(s => s.ObtenirMatchsPublicsAsync("XXXX")).ReturnsAsync((List<MatchPublicDto>?)null);

        // Act
        var resultat = await _controller.ObtenirPublics("XXXX");

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultat);
    }

    [Fact]
    public async Task Rejoindre_RequeteValide_RetourneOk() {
        // Arrange
        var requete = new RejoindreMatchRequestDto { MembreMatricule = "G0001" };
        var attendu = new InscriptionResultatDto { Succes = true, MontantPaye = 15.00m, DetteReglee = false };
        _serviceMock.Setup(s => s.RejoindreMatchPublicAsync(1, "G0001")).ReturnsAsync(attendu);

        // Act
        var resultat = await _controller.Rejoindre(1, requete);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(attendu, okResult.Value);
    }

    [Fact]
    public async Task Rejoindre_RequeteInvalide_RetourneBadRequest() {
        // Arrange
        var requete = new RejoindreMatchRequestDto { MembreMatricule = "XXXX" };
        _serviceMock.Setup(s => s.RejoindreMatchPublicAsync(1, "XXXX")).ReturnsAsync(new InscriptionResultatDto { Succes = false, MessageErreur = "Membre introuvable." });

        // Act
        var resultat = await _controller.Rejoindre(1, requete);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultat);
    }

    [Fact]
    public async Task ObtenirMontantAPayer_RetourneOk() {
        // Arrange
        var montant = new MontantAPayerDto { MontantParticipation = 15.00m, MontantDette = 45.00m, MontantTotal = 60.00m };
        _serviceMock.Setup(s => s.ObtenirMontantAPayerAsync("G0001")).ReturnsAsync(montant);

        // Act
        var resultat = await _controller.ObtenirMontantAPayer("G0001");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(montant, okResult.Value);
    }

    [Fact]
    public async Task PayerParticipation_RequeteValide_RetourneOk() {
        // Arrange
        var requete = new PayerParticipationRequestDto { MembreMatricule = "L00001" };
        var attendu = new InscriptionResultatDto { Succes = true, MontantPaye = 15.00m, DetteReglee = false };
        _serviceMock.Setup(s => s.PayerParticipationAsync(1, "L00001")).ReturnsAsync(attendu);

        // Act
        var resultat = await _controller.PayerParticipation(1, requete);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(attendu, okResult.Value);
    }

    [Fact]
    public async Task PayerParticipation_RequeteInvalide_RetourneBadRequest() {
        // Arrange
        var requete = new PayerParticipationRequestDto { MembreMatricule = "XXXX" };
        _serviceMock.Setup(s => s.PayerParticipationAsync(1, "XXXX")).ReturnsAsync(new InscriptionResultatDto { Succes = false, MessageErreur = "Participation introuvable." });

        // Act
        var resultat = await _controller.PayerParticipation(1, requete);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultat);
    }
}

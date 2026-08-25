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
        var requete = new CreerMatchPriveRequestDto { OrganisateurMatricule = "G001", SiteId = 1, TerrainId = 11, Date = new DateOnly(2026, 1, 5), HeureDebut = new TimeOnly(9, 0) };
        var dto = new MatchDto { Id = 1, SiteId = 1, TerrainId = 11, OrganisateurMatricule = "G001", Statut = "INCOMPLET", Visibilite = "PRIVE" };
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
        var requete = new CreerMatchPublicRequestDto { OrganisateurMatricule = "G001", SiteId = 1, TerrainId = 11, Date = new DateOnly(2026, 1, 5), HeureDebut = new TimeOnly(9, 0) };
        var dto = new MatchDto { Id = 1, SiteId = 1, TerrainId = 11, OrganisateurMatricule = "G001", Statut = "INCOMPLET", Visibilite = "PUBLIC" };
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
        _serviceMock.Setup(s => s.ObtenirMatchsPublicsAsync("G001")).ReturnsAsync(matchs);

        // Act
        var resultat = await _controller.ObtenirPublics("G001");

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
        var requete = new RejoindreMatchRequestDto { MembreMatricule = "G001" };
        var attendu = new InscriptionResultatDto { Succes = true, MontantPaye = 15.00m, DetteReglee = false };
        _serviceMock.Setup(s => s.RejoindreMatchPublicAsync(1, "G001")).ReturnsAsync(attendu);

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
        _serviceMock.Setup(s => s.ObtenirMontantAPayerAsync("G001")).ReturnsAsync(montant);

        // Act
        var resultat = await _controller.ObtenirMontantAPayer("G001");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(montant, okResult.Value);
    }

    [Fact]
    public async Task PayerParticipation_RequeteValide_RetourneOk() {
        // Arrange
        var requete = new PayerParticipationRequestDto { MembreMatricule = "L001" };
        var attendu = new InscriptionResultatDto { Succes = true, MontantPaye = 15.00m, DetteReglee = false };
        _serviceMock.Setup(s => s.PayerParticipationAsync(1, "L001")).ReturnsAsync(attendu);

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

    [Fact]
    public async Task ObtenirParticipationsEnAttente_MembreConnu_RetourneOk() {
        // Arrange
        var participations = new List<ParticipationEnAttenteDto> { new() { ParticipationId = 10, MatchId = 1, SiteId = 1, NomSite = "Site 1", TerrainId = 11, NumeroTerrain = 3, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), OrganisateurMatricule = "S001" } };
        _serviceMock.Setup(s => s.ObtenirParticipationsEnAttenteAsync("G001")).ReturnsAsync(participations);

        // Act
        var resultat = await _controller.ObtenirParticipationsEnAttente("G001");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(participations, okResult.Value);
    }

    [Fact]
    public async Task ObtenirParticipationsEnAttente_MembreInconnu_RetourneNotFound() {
        // Arrange
        _serviceMock.Setup(s => s.ObtenirParticipationsEnAttenteAsync("XXXX")).ReturnsAsync((List<ParticipationEnAttenteDto>?)null);

        // Act
        var resultat = await _controller.ObtenirParticipationsEnAttente("XXXX");

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultat);
    }

    [Fact]
    public async Task ObtenirReservations_MembreConnu_RetourneOk() {
        // Arrange
        var reservations = new List<ReservationDto> { new() { Id = 1, SiteId = 1, NomSite = "Site 1", TerrainId = 11, NumeroTerrain = 2, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PRIVE", Statut = "INCOMPLET", EstOrganisateur = true } };
        _serviceMock.Setup(s => s.ObtenirReservationsAsync("G001")).ReturnsAsync(reservations);

        // Act
        var resultat = await _controller.ObtenirReservations("G001");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(reservations, okResult.Value);
    }

    [Fact]
    public async Task ObtenirReservations_MembreInconnu_RetourneNotFound() {
        // Arrange
        _serviceMock.Setup(s => s.ObtenirReservationsAsync("XXXX")).ReturnsAsync((List<ReservationDto>?)null);

        // Act
        var resultat = await _controller.ObtenirReservations("XXXX");

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultat);
    }

    [Fact]
    public async Task ObtenirDetail_MatchConsultable_RetourneOk() {
        // Arrange
        var detail = new MatchDetailDto {
            Id = 1, SiteId = 1, NomSite = "Site 1", TerrainId = 11, NumeroTerrain = 2,
            DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PRIVE", Statut = "INCOMPLET",
            OrganisateurMatricule = "G001", Joueurs = new List<JoueurDetailDto> { new() { MembreMatricule = "G001", Paye = true } }
        };
        _serviceMock.Setup(s => s.ObtenirDetailAsync(1, "G001")).ReturnsAsync(detail);

        // Act
        var resultat = await _controller.ObtenirDetail(1, "G001");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(detail, okResult.Value);
    }

    [Fact]
    public async Task ObtenirDetail_IntrouvableOuNonAutorise_RetourneNotFound() {
        // Arrange
        _serviceMock.Setup(s => s.ObtenirDetailAsync(1, "XXXX")).ReturnsAsync((MatchDetailDto?)null);

        // Act
        var resultat = await _controller.ObtenirDetail(1, "XXXX");

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultat);
    }
}

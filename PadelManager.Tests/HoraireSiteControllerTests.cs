using Microsoft.AspNetCore.Mvc;
using Moq;
using PadelManager.Api.Controllers;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;
using Xunit;

namespace PadelManager.Tests;

public class HoraireSiteControllerTests {
    private readonly Mock<IHoraireSiteService> _horaireServiceMock;
    private readonly Mock<IAdminPorteeService> _adminPorteeServiceMock;
    private readonly HoraireSiteController _controller;

    public HoraireSiteControllerTests() {
        _horaireServiceMock = new Mock<IHoraireSiteService>();
        _adminPorteeServiceMock = new Mock<IAdminPorteeService>();
        _adminPorteeServiceMock.Setup(s => s.VerifierPorteeSiteAsync(It.IsAny<string>(), It.IsAny<int?>()))
            .ReturnsAsync(new PorteeAdminResultatDto { Autorise = true });
        _controller = new HoraireSiteController(_horaireServiceMock.Object, _adminPorteeServiceMock.Object);
    }

    // Obtenir (GET) n'est volontairement pas soumis au contrôle de portée admin : cette route est
    // aussi consommée par l'application Membre (CreerMatchForm/CreerMatchPublicForm) pour savoir
    // à l'avance quels jours un site est ouvert — n'importe quel membre doit pouvoir la lire.

    [Fact]
    public async Task Obtenir_HoraireExistant_RetourneOk() {
        // Arrange
        var dto = new HoraireSiteDto { SiteId = 1, Annee = 2026, JoursOuverture = new List<string> { "LUN" } };
        _horaireServiceMock.Setup(s => s.ObtenirHoraireAsync(1, 2026)).ReturnsAsync(dto);

        // Act
        var resultat = await _controller.Obtenir(1, 2026);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    public async Task Obtenir_AucunHoraire_RetourneNotFound() {
        // Arrange
        _horaireServiceMock.Setup(s => s.ObtenirHoraireAsync(1, 2026)).ReturnsAsync((HoraireSiteDto?)null);

        // Act
        var resultat = await _controller.Obtenir(1, 2026);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultat);
    }

    [Fact]
    public async Task Definir_RequeteValide_RetourneOk() {
        // Arrange
        var requete = new HoraireSiteRequestDto {
            AdminMatricule = "G001",
            JoursOuverture = new List<string> { "LUN" },
            HeureDebutReservation = new TimeOnly(9, 0),
            HeureFinReservation = new TimeOnly(21, 0)
        };
        var dto = new HoraireSiteDto { SiteId = 1, Annee = 2026, JoursOuverture = requete.JoursOuverture };
        _horaireServiceMock.Setup(s => s.DefinirHoraireAsync(1, 2026, requete))
            .ReturnsAsync(new DefinirHoraireResultatDto { Succes = true, Horaire = dto });

        // Act
        var resultat = await _controller.Definir(1, 2026, requete);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    public async Task Definir_RequeteInvalide_RetourneBadRequest() {
        // Arrange
        var requete = new HoraireSiteRequestDto { AdminMatricule = "G001" };
        _horaireServiceMock.Setup(s => s.DefinirHoraireAsync(1, 2026, requete))
            .ReturnsAsync(new DefinirHoraireResultatDto { Succes = false, MessageErreur = "Veuillez sélectionner au moins un jour d'ouverture." });

        // Act
        var resultat = await _controller.Definir(1, 2026, requete);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultat);
    }

    [Fact]
    public async Task Definir_PorteeRefusee_RetourneForbidden() {
        // Arrange : un admin de site tente d'écrire sur un autre site.
        var requete = new HoraireSiteRequestDto { AdminMatricule = "S002" };
        _adminPorteeServiceMock.Setup(s => s.VerifierPorteeSiteAsync("S002", 1))
            .ReturnsAsync(new PorteeAdminResultatDto { Autorise = false, MessageErreur = "Cet administrateur n'est pas autorisé pour ce site." });

        // Act
        var resultat = await _controller.Definir(1, 2026, requete);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(resultat);
        Assert.Equal(403, objectResult.StatusCode);
        _horaireServiceMock.Verify(s => s.DefinirHoraireAsync(It.IsAny<int>(), It.IsAny<short>(), It.IsAny<HoraireSiteRequestDto>()), Times.Never);
    }
}

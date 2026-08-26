using Microsoft.AspNetCore.Mvc;
using Moq;
using PadelManager.Api.Controllers;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;
using Xunit;

namespace PadelManager.Tests;

public class JourFermetureControllerTests {
    private readonly Mock<IJourFermetureService> _serviceMock;
    private readonly Mock<IAdminPorteeService> _adminPorteeServiceMock;
    private readonly JourFermetureController _controller;

    public JourFermetureControllerTests() {
        _serviceMock = new Mock<IJourFermetureService>();
        _adminPorteeServiceMock = new Mock<IAdminPorteeService>();
        _adminPorteeServiceMock.Setup(s => s.VerifierPorteeSiteAsync(It.IsAny<string>(), It.IsAny<int?>()))
            .ReturnsAsync(new PorteeAdminResultatDto { Autorise = true });
        _controller = new JourFermetureController(_serviceMock.Object, _adminPorteeServiceMock.Object);
    }

    [Fact]
    public async Task ObtenirPourSite_RetourneOkAvecLaListe() {
        // Arrange
        var liste = new List<JourFermetureDto> { new() { Id = 1, SiteId = 1, Date = new DateOnly(2026, 12, 24) } };
        _serviceMock.Setup(s => s.ObtenirPourSiteEtAnneeAsync(1, 2026)).ReturnsAsync(liste);

        // Act
        var resultat = await _controller.ObtenirPourSite(1, 2026, "G001");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(liste, okResult.Value);
    }

    [Fact]
    public async Task ObtenirPourSite_PorteeRefusee_RetourneForbidden() {
        // Arrange
        _adminPorteeServiceMock.Setup(s => s.VerifierPorteeSiteAsync("S002", 1))
            .ReturnsAsync(new PorteeAdminResultatDto { Autorise = false, MessageErreur = "Cet administrateur n'est pas autorisé pour ce site." });

        // Act
        var resultat = await _controller.ObtenirPourSite(1, 2026, "S002");

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(resultat);
        Assert.Equal(403, objectResult.StatusCode);
    }

    [Fact]
    public async Task Declarer_RequeteValide_RetourneOk() {
        // Arrange
        var requete = new JourFermetureRequestDto { AdminMatricule = "G001", SiteId = 1, Date = new DateOnly(2026, 12, 24) };
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
        var requete = new JourFermetureRequestDto { AdminMatricule = "G001", SiteId = 99, Date = new DateOnly(2026, 12, 24) };
        _serviceMock.Setup(s => s.DeclarerAsync(requete))
            .ReturnsAsync(new DeclarerFermetureResultatDto { Succes = false, MessageErreur = "Site introuvable." });

        // Act
        var resultat = await _controller.Declarer(requete);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultat);
    }

    [Fact]
    public async Task Declarer_AdminSiteCibleUnAutreSite_RetourneForbidden() {
        // Arrange
        var requete = new JourFermetureRequestDto { AdminMatricule = "S002", SiteId = 1, Date = new DateOnly(2026, 12, 24) };
        _adminPorteeServiceMock.Setup(s => s.VerifierPorteeSiteAsync("S002", 1))
            .ReturnsAsync(new PorteeAdminResultatDto { Autorise = false, MessageErreur = "Cet administrateur n'est pas autorisé pour ce site." });

        // Act
        var resultat = await _controller.Declarer(requete);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(resultat);
        Assert.Equal(403, objectResult.StatusCode);
        _serviceMock.Verify(s => s.DeclarerAsync(It.IsAny<JourFermetureRequestDto>()), Times.Never);
    }

    [Fact]
    public async Task Declarer_AdminSiteCibleFermetureGlobale_RetourneForbidden() {
        // Arrange : siteId == null (fermeture globale) réservé à l'admin global, jamais un admin de site.
        var requete = new JourFermetureRequestDto { AdminMatricule = "S001", SiteId = null, Date = new DateOnly(2026, 12, 24) };
        _adminPorteeServiceMock.Setup(s => s.VerifierPorteeSiteAsync("S001", null))
            .ReturnsAsync(new PorteeAdminResultatDto { Autorise = false, MessageErreur = "Cet administrateur n'est pas autorisé pour ce site." });

        // Act
        var resultat = await _controller.Declarer(requete);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(resultat);
        Assert.Equal(403, objectResult.StatusCode);
    }

    [Fact]
    public async Task Supprimer_FermetureExistante_RetourneNoContent() {
        // Arrange
        _serviceMock.Setup(s => s.ObtenirParIdAsync(5)).ReturnsAsync(new JourFermetureDto { Id = 5, SiteId = 1, Date = new DateOnly(2026, 12, 24) });
        _serviceMock.Setup(s => s.SupprimerAsync(5)).ReturnsAsync(true);

        // Act
        var resultat = await _controller.Supprimer(5, "G001");

        // Assert
        Assert.IsType<NoContentResult>(resultat);
    }

    [Fact]
    public async Task Supprimer_FermetureInexistante_RetourneNotFound() {
        // Arrange
        _serviceMock.Setup(s => s.ObtenirParIdAsync(999)).ReturnsAsync((JourFermetureDto?)null);

        // Act
        var resultat = await _controller.Supprimer(999, "G001");

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultat);
        _serviceMock.Verify(s => s.SupprimerAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Supprimer_AdminSiteCibleUneFermetureDunAutreSite_RetourneForbidden() {
        // Arrange
        _serviceMock.Setup(s => s.ObtenirParIdAsync(5)).ReturnsAsync(new JourFermetureDto { Id = 5, SiteId = 2, Date = new DateOnly(2026, 12, 24) });
        _adminPorteeServiceMock.Setup(s => s.VerifierPorteeSiteAsync("S001", 2))
            .ReturnsAsync(new PorteeAdminResultatDto { Autorise = false, MessageErreur = "Cet administrateur n'est pas autorisé pour ce site." });

        // Act
        var resultat = await _controller.Supprimer(5, "S001");

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(resultat);
        Assert.Equal(403, objectResult.StatusCode);
        _serviceMock.Verify(s => s.SupprimerAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Supprimer_AdminSiteCibleUneFermetureGlobale_RetourneForbidden() {
        // Arrange : seul un admin global peut annuler une fermeture globale (siteId NULL).
        _serviceMock.Setup(s => s.ObtenirParIdAsync(6)).ReturnsAsync(new JourFermetureDto { Id = 6, SiteId = null, Date = new DateOnly(2026, 12, 25) });
        _adminPorteeServiceMock.Setup(s => s.VerifierPorteeSiteAsync("S001", null))
            .ReturnsAsync(new PorteeAdminResultatDto { Autorise = false, MessageErreur = "Cet administrateur n'est pas autorisé pour ce site." });

        // Act
        var resultat = await _controller.Supprimer(6, "S001");

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(resultat);
        Assert.Equal(403, objectResult.StatusCode);
    }
}

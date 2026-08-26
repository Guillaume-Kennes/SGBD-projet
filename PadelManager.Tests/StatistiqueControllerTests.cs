using Microsoft.AspNetCore.Mvc;
using Moq;
using PadelManager.Api.Controllers;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;
using Xunit;

namespace PadelManager.Tests;

public class StatistiqueControllerTests {
    private readonly Mock<IStatistiqueService> _serviceMock;
    private readonly Mock<IAdminPorteeService> _adminPorteeServiceMock;
    private readonly StatistiqueController _controller;

    public StatistiqueControllerTests() {
        _serviceMock = new Mock<IStatistiqueService>();
        _adminPorteeServiceMock = new Mock<IAdminPorteeService>();
        _adminPorteeServiceMock.Setup(s => s.VerifierPorteeSiteAsync(It.IsAny<string>(), It.IsAny<int?>()))
            .ReturnsAsync(new PorteeAdminResultatDto { Autorise = true });
        _controller = new StatistiqueController(_serviceMock.Object, _adminPorteeServiceMock.Object);
    }

    [Fact]
    public async Task ObtenirChiffreAffaires_RetourneOk() {
        // Arrange
        var chiffreAffaires = new List<ChiffreAffairesDto> { new() { SiteId = 1, NomSite = "Site 1", Montant = 60.00m } };
        _serviceMock.Setup(s => s.ObtenirChiffreAffairesAsync(1)).ReturnsAsync(chiffreAffaires);

        // Act
        var resultat = await _controller.ObtenirChiffreAffaires(1, "G001");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(chiffreAffaires, okResult.Value);
    }

    [Fact]
    public async Task ObtenirChiffreAffaires_SansSiteId_RetourneOk() {
        // Arrange
        var chiffreAffaires = new List<ChiffreAffairesDto> {
            new() { SiteId = 1, NomSite = "Site 1", Montant = 60.00m },
            new() { SiteId = 2, NomSite = "Site 2", Montant = 0.00m }
        };
        _serviceMock.Setup(s => s.ObtenirChiffreAffairesAsync(null)).ReturnsAsync(chiffreAffaires);

        // Act
        var resultat = await _controller.ObtenirChiffreAffaires(null, "G001");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(chiffreAffaires, okResult.Value);
    }

    [Fact]
    public async Task ObtenirChiffreAffaires_PorteeRefusee_RetourneForbidden() {
        // Arrange
        _adminPorteeServiceMock.Setup(s => s.VerifierPorteeSiteAsync("S002", 1))
            .ReturnsAsync(new PorteeAdminResultatDto { Autorise = false, MessageErreur = "Cet administrateur n'est pas autorisé pour ce site." });

        // Act
        var resultat = await _controller.ObtenirChiffreAffaires(1, "S002");

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(resultat);
        Assert.Equal(403, objectResult.StatusCode);
        _serviceMock.Verify(s => s.ObtenirChiffreAffairesAsync(It.IsAny<int?>()), Times.Never);
    }

    [Fact]
    public async Task ObtenirStatistiques_RetourneOk() {
        // Arrange
        var statistiques = new List<StatistiquesDto> {
            new() { SiteId = 1, NomSite = "Site 1", NombreMatchsPublics = 3, NombreMatchsPrives = 2, TauxOccupation = 0.15m, MembresActifs = 4 }
        };
        _serviceMock.Setup(s => s.ObtenirStatistiquesAsync(1)).ReturnsAsync(statistiques);

        // Act
        var resultat = await _controller.ObtenirStatistiques(1, "G001");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(statistiques, okResult.Value);
    }

    [Fact]
    public async Task ObtenirStatistiques_PorteeRefusee_RetourneForbidden() {
        // Arrange
        _adminPorteeServiceMock.Setup(s => s.VerifierPorteeSiteAsync("S002", null))
            .ReturnsAsync(new PorteeAdminResultatDto { Autorise = false, MessageErreur = "Cet administrateur n'est pas autorisé pour ce site." });

        // Act : un admin de site tentant "tous les sites" (siteId omis) doit être rejeté.
        var resultat = await _controller.ObtenirStatistiques(null, "S002");

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(resultat);
        Assert.Equal(403, objectResult.StatusCode);
        _serviceMock.Verify(s => s.ObtenirStatistiquesAsync(It.IsAny<int?>()), Times.Never);
    }
}

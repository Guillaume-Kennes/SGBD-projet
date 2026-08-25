using Microsoft.AspNetCore.Mvc;
using Moq;
using PadelManager.Api.Controllers;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;
using Xunit;

namespace PadelManager.Tests;

public class StatistiqueControllerTests {
    private readonly Mock<IStatistiqueService> _serviceMock;
    private readonly StatistiqueController _controller;

    public StatistiqueControllerTests() {
        _serviceMock = new Mock<IStatistiqueService>();
        _controller = new StatistiqueController(_serviceMock.Object);
    }

    [Fact]
    public async Task ObtenirChiffreAffaires_RetourneOk() {
        // Arrange
        var chiffreAffaires = new List<ChiffreAffairesDto> { new() { SiteId = 1, NomSite = "Site 1", Montant = 60.00m } };
        _serviceMock.Setup(s => s.ObtenirChiffreAffairesAsync(1)).ReturnsAsync(chiffreAffaires);

        // Act
        var resultat = await _controller.ObtenirChiffreAffaires(1);

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
        var resultat = await _controller.ObtenirChiffreAffaires(null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(chiffreAffaires, okResult.Value);
    }
}

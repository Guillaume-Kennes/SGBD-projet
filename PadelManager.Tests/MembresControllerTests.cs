using Microsoft.AspNetCore.Mvc;
using Moq;
using PadelManager.Api.Controllers;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;
using Xunit;

namespace PadelManager.Tests;

public class MembresControllerTests {
    private readonly Mock<IMembreService> _serviceMock;
    private readonly MembresController _controller;

    public MembresControllerTests() {
        _serviceMock = new Mock<IMembreService>();
        _controller = new MembresController(_serviceMock.Object);
    }

    [Fact]
    public async Task ObtenirMembres_AvecSiteId_RetourneOk() {
        // Arrange
        var membres = new List<MembreAdminDto> { new() { Matricule = "S001", TypeMembre = "SITE", SiteId = 1, DetteActive = false, PenaliteActive = false } };
        _serviceMock.Setup(s => s.ObtenirMembresAsync(1)).ReturnsAsync(membres);

        // Act
        var resultat = await _controller.ObtenirMembres(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(membres, okResult.Value);
    }

    [Fact]
    public async Task ObtenirMembres_SansSiteId_RetourneOk() {
        // Arrange
        var membres = new List<MembreAdminDto> {
            new() { Matricule = "G001", TypeMembre = "GLOBAL", SiteId = null, DetteActive = true, PenaliteActive = false },
            new() { Matricule = "S001", TypeMembre = "SITE", SiteId = 1, DetteActive = false, PenaliteActive = true }
        };
        _serviceMock.Setup(s => s.ObtenirMembresAsync(null)).ReturnsAsync(membres);

        // Act
        var resultat = await _controller.ObtenirMembres(null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(membres, okResult.Value);
    }
}

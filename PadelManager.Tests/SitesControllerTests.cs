using Microsoft.AspNetCore.Mvc;
using Moq;
using PadelManager.Api.Controllers;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;
using Xunit;

namespace PadelManager.Tests;

public class SitesControllerTests {
    private readonly Mock<ISiteService> _siteServiceMock;
    private readonly SitesController _controller;

    public SitesControllerTests() {
        _siteServiceMock = new Mock<ISiteService>();
        _controller = new SitesController(_siteServiceMock.Object);
    }

    [Fact]
    public async Task ObtenirTous_RetourneOkAvecLaListe() {
        // Arrange
        var sites = new List<SiteDto> { new() { Id = 1, Nom = "Site 1" } };
        _siteServiceMock.Setup(s => s.ObtenirTousAsync()).ReturnsAsync(sites);

        // Act
        var resultat = await _controller.ObtenirTous();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(sites, okResult.Value);
    }

    [Fact]
    public async Task ObtenirParId_SiteExistant_RetourneOk() {
        // Arrange
        var site = new SiteDto { Id = 1, Nom = "Site 1" };
        _siteServiceMock.Setup(s => s.ObtenirParIdAsync(1)).ReturnsAsync(site);

        // Act
        var resultat = await _controller.ObtenirParId(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(site, okResult.Value);
    }

    [Fact]
    public async Task ObtenirParId_SiteInexistant_RetourneNotFound() {
        // Arrange
        _siteServiceMock.Setup(s => s.ObtenirParIdAsync(99)).ReturnsAsync((SiteDto?)null);

        // Act
        var resultat = await _controller.ObtenirParId(99);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultat);
    }
}

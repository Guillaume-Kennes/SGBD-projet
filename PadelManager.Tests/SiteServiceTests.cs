using Moq;
using PadelManager.Interfaces;
using PadelManager.Services;
using Xunit;

namespace PadelManager.Tests;

public class SiteServiceTests {
    private readonly Mock<ISiteRepository> _siteRepoMock;
    private readonly SiteService _service;

    public SiteServiceTests() {
        _siteRepoMock = new Mock<ISiteRepository>();
        _service = new SiteService(_siteRepoMock.Object);
    }

    [Fact]
    public async Task ObtenirParIdAsync_SiteExistant_RetourneLeDto() {
        // Arrange
        _siteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Site { Id = 1, Nom = "Site 1" });

        // Act
        var resultat = await _service.ObtenirParIdAsync(1);

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal(1, resultat!.Id);
        Assert.Equal("Site 1", resultat.Nom);
    }

    [Fact]
    public async Task ObtenirParIdAsync_SiteInexistant_RetourneNull() {
        // Arrange
        _siteRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Site?)null);

        // Act
        var resultat = await _service.ObtenirParIdAsync(99);

        // Assert
        Assert.Null(resultat);
    }

    [Fact]
    public async Task ObtenirTousAsync_RetourneTousLesSitesMappes() {
        // Arrange
        _siteRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Site> {
            new() { Id = 1, Nom = "Site 1" },
            new() { Id = 2, Nom = "Site 2" }
        });

        // Act
        var resultat = await _service.ObtenirTousAsync();

        // Assert
        Assert.Equal(2, resultat.Count);
        Assert.Equal("Site 1", resultat[0].Nom);
        Assert.Equal("Site 2", resultat[1].Nom);
    }

    [Fact]
    public async Task ObtenirTousAsync_AucunSite_RetourneListeVide() {
        // Arrange
        _siteRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Site>());

        // Act
        var resultat = await _service.ObtenirTousAsync();

        // Assert
        Assert.Empty(resultat);
    }
}

using Moq;
using PadelManager.Interfaces;
using PadelManager.Services;
using Xunit;

namespace PadelManager.Tests;

public class DisponibiliteServiceTests {
    private readonly Mock<ISiteRepository> _siteRepoMock;
    private readonly Mock<IDisponibiliteRepository> _disponibiliteRepoMock;
    private readonly DisponibiliteService _service;

    public DisponibiliteServiceTests() {
        _siteRepoMock = new Mock<ISiteRepository>();
        _disponibiliteRepoMock = new Mock<IDisponibiliteRepository>();
        _service = new DisponibiliteService(_siteRepoMock.Object, _disponibiliteRepoMock.Object);
    }

    [Fact]
    public async Task ConsulterPlanningAsync_SiteInconnu_RetourneNull() {
        // Arrange
        _siteRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Site?)null);

        // Act
        var resultat = await _service.ConsulterPlanningAsync(99, new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 31));

        // Assert
        Assert.Null(resultat);
    }

    [Fact]
    public async Task ConsulterPlanningAsync_SiteConnu_RetourneLesCreneauxMappes() {
        // Arrange
        _siteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Site { Id = 1, Nom = "Site 1" });
        _disponibiliteRepoMock.Setup(r => r.GetBySiteAndPeriodeAsync(1, new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 31)))
            .ReturnsAsync(new List<Disponibilite> {
                new() { SiteId = 1, Date = new DateOnly(2026, 1, 5), HeureDebut = new TimeOnly(9, 0), HeureFin = new TimeOnly(10, 30) }
            });

        // Act
        var resultat = await _service.ConsulterPlanningAsync(1, new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 31));

        // Assert
        Assert.NotNull(resultat);
        Assert.Single(resultat!);
        Assert.Equal(new DateOnly(2026, 1, 5), resultat[0].Date);
    }

    [Fact]
    public async Task ConsulterPlanningAsync_PeriodeInversee_RetourneListeVide() {
        // Arrange
        _siteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Site { Id = 1, Nom = "Site 1" });

        // Act
        var resultat = await _service.ConsulterPlanningAsync(1, new DateOnly(2026, 1, 31), new DateOnly(2026, 1, 1));

        // Assert
        Assert.NotNull(resultat);
        Assert.Empty(resultat!);
        _disponibiliteRepoMock.Verify(r => r.GetBySiteAndPeriodeAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()), Times.Never);
    }
}

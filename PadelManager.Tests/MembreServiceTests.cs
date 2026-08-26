using Moq;
using PadelManager.Interfaces;
using PadelManager.Services;
using Xunit;

namespace PadelManager.Tests;

public class MembreServiceTests {
    private readonly Mock<IMembreRepository> _membreRepoMock;
    private readonly Mock<IDetteRepository> _detteRepoMock;
    private readonly Mock<IPenaliteRepository> _penaliteRepoMock;
    private readonly MembreService _service;

    private static readonly DateOnly Aujourdhui = DateOnly.FromDateTime(DateTime.Today);

    public MembreServiceTests() {
        _membreRepoMock = new Mock<IMembreRepository>();
        _detteRepoMock = new Mock<IDetteRepository>();
        _penaliteRepoMock = new Mock<IPenaliteRepository>();
        _service = new MembreService(_membreRepoMock.Object, _detteRepoMock.Object, _penaliteRepoMock.Object);

        _detteRepoMock.Setup(r => r.ExisteDetteNonSoldeeAsync(It.IsAny<string>())).ReturnsAsync(false);
        _penaliteRepoMock.Setup(r => r.GetPlusRecenteAsync(It.IsAny<string>())).ReturnsAsync((Penalite?)null);
    }

    [Fact]
    public async Task ObtenirMembresAsync_TransmetLeFiltreSiteAuRepository() {
        _membreRepoMock.Setup(r => r.GetTousAsync(1)).ReturnsAsync(new List<Membre>());

        await _service.ObtenirMembresAsync(1);

        _membreRepoMock.Verify(r => r.GetTousAsync(1), Times.Once);
    }

    [Fact]
    public async Task ObtenirMembresAsync_ExposeMatriculeTypeEtSite_TriesParMatricule() {
        _membreRepoMock.Setup(r => r.GetTousAsync(null)).ReturnsAsync(new List<Membre> {
            new() { Matricule = "S002", TypeMembre = "SITE", SiteId = 1 },
            new() { Matricule = "G001", TypeMembre = "GLOBAL", SiteId = null }
        });

        var resultat = await _service.ObtenirMembresAsync(null);

        Assert.Equal(2, resultat.Count);
        Assert.Equal("G001", resultat[0].Matricule); // trié
        Assert.Equal("GLOBAL", resultat[0].TypeMembre);
        Assert.Null(resultat[0].SiteId);
        Assert.Equal("S002", resultat[1].Matricule);
        Assert.Equal(1, resultat[1].SiteId);
    }

    [Fact]
    public async Task ObtenirMembresAsync_DetteNonSoldee_MarqueDetteActive() {
        _membreRepoMock.Setup(r => r.GetTousAsync(null)).ReturnsAsync(new List<Membre> { new() { Matricule = "G001", TypeMembre = "GLOBAL" } });
        _detteRepoMock.Setup(r => r.ExisteDetteNonSoldeeAsync("G001")).ReturnsAsync(true);

        var resultat = await _service.ObtenirMembresAsync(null);

        Assert.True(resultat[0].DetteActive);
        Assert.False(resultat[0].PenaliteActive);
    }

    [Fact]
    public async Task ObtenirMembresAsync_PenaliteActive_MarquePenaliteActive() {
        _membreRepoMock.Setup(r => r.GetTousAsync(null)).ReturnsAsync(new List<Membre> { new() { Matricule = "G001", TypeMembre = "GLOBAL" } });
        _penaliteRepoMock.Setup(r => r.GetPlusRecenteAsync("G001")).ReturnsAsync(new Penalite { MembreMatricule = "G001", MatchOrigineId = 1, DelaiJusquAu = Aujourdhui.AddDays(7) });

        var resultat = await _service.ObtenirMembresAsync(null);

        Assert.True(resultat[0].PenaliteActive);
    }

    [Fact]
    public async Task ObtenirMembresAsync_PenaliteExpiree_NePasMarquerActive() {
        _membreRepoMock.Setup(r => r.GetTousAsync(null)).ReturnsAsync(new List<Membre> { new() { Matricule = "G001", TypeMembre = "GLOBAL" } });
        _penaliteRepoMock.Setup(r => r.GetPlusRecenteAsync("G001")).ReturnsAsync(new Penalite { MembreMatricule = "G001", MatchOrigineId = 1, DelaiJusquAu = Aujourdhui.AddDays(-1) });

        var resultat = await _service.ObtenirMembresAsync(null);

        Assert.False(resultat[0].PenaliteActive);
    }
}

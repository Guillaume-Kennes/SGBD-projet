using Moq;
using PadelManager.Interfaces;
using PadelManager.Services;
using Xunit;

namespace PadelManager.Tests;

public class AdminPorteeServiceTests {
    private readonly Mock<IAdministrateurRepository> _administrateurRepoMock;
    private readonly AdminPorteeService _service;

    public AdminPorteeServiceTests() {
        _administrateurRepoMock = new Mock<IAdministrateurRepository>();
        _service = new AdminPorteeService(_administrateurRepoMock.Object);
    }

    // --- VerifierPorteeSiteAsync ---

    [Fact]
    public async Task VerifierPorteeSiteAsync_AdminInconnu_Refuse() {
        _administrateurRepoMock.Setup(r => r.GetByMatriculeAsync("XXXX")).ReturnsAsync((Administrateur?)null);

        var resultat = await _service.VerifierPorteeSiteAsync("XXXX", 1);

        Assert.False(resultat.Autorise);
        Assert.NotNull(resultat.MessageErreur);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(null)]
    public async Task VerifierPorteeSiteAsync_AdminGlobal_AutoriseQuelQueSoitLeSite(int? siteIdDemande) {
        _administrateurRepoMock.Setup(r => r.GetByMatriculeAsync("G001"))
            .ReturnsAsync(new Administrateur { Matricule = "G001", Type = "GLOBAL", SiteId = null });

        var resultat = await _service.VerifierPorteeSiteAsync("G001", siteIdDemande);

        Assert.True(resultat.Autorise);
    }

    [Fact]
    public async Task VerifierPorteeSiteAsync_AdminSite_AutorisePourSonPropreSite() {
        _administrateurRepoMock.Setup(r => r.GetByMatriculeAsync("S001"))
            .ReturnsAsync(new Administrateur { Matricule = "S001", Type = "SITE", SiteId = 1 });

        var resultat = await _service.VerifierPorteeSiteAsync("S001", 1);

        Assert.True(resultat.Autorise);
    }

    [Fact]
    public async Task VerifierPorteeSiteAsync_AdminSite_RefusePourUnAutreSite() {
        _administrateurRepoMock.Setup(r => r.GetByMatriculeAsync("S001"))
            .ReturnsAsync(new Administrateur { Matricule = "S001", Type = "SITE", SiteId = 1 });

        var resultat = await _service.VerifierPorteeSiteAsync("S001", 2);

        Assert.False(resultat.Autorise);
        Assert.NotNull(resultat.MessageErreur);
    }

    [Fact]
    public async Task VerifierPorteeSiteAsync_AdminSite_RefuseSiteIdNull() {
        // "Tous les sites" n'a pas de sens pour un admin de site : rejeté, pas ignoré silencieusement.
        _administrateurRepoMock.Setup(r => r.GetByMatriculeAsync("S001"))
            .ReturnsAsync(new Administrateur { Matricule = "S001", Type = "SITE", SiteId = 1 });

        var resultat = await _service.VerifierPorteeSiteAsync("S001", null);

        Assert.False(resultat.Autorise);
    }

    // --- VerifierAdminGlobalAsync ---

    [Fact]
    public async Task VerifierAdminGlobalAsync_AdminGlobal_Autorise() {
        _administrateurRepoMock.Setup(r => r.GetByMatriculeAsync("G001"))
            .ReturnsAsync(new Administrateur { Matricule = "G001", Type = "GLOBAL", SiteId = null });

        var resultat = await _service.VerifierAdminGlobalAsync("G001");

        Assert.True(resultat.Autorise);
    }

    [Fact]
    public async Task VerifierAdminGlobalAsync_AdminSite_Refuse() {
        _administrateurRepoMock.Setup(r => r.GetByMatriculeAsync("S001"))
            .ReturnsAsync(new Administrateur { Matricule = "S001", Type = "SITE", SiteId = 1 });

        var resultat = await _service.VerifierAdminGlobalAsync("S001");

        Assert.False(resultat.Autorise);
        Assert.NotNull(resultat.MessageErreur);
    }

    [Fact]
    public async Task VerifierAdminGlobalAsync_AdminInconnu_Refuse() {
        _administrateurRepoMock.Setup(r => r.GetByMatriculeAsync("XXXX")).ReturnsAsync((Administrateur?)null);

        var resultat = await _service.VerifierAdminGlobalAsync("XXXX");

        Assert.False(resultat.Autorise);
    }
}

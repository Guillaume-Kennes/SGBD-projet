using Moq;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;
using PadelManager.Services;
using Xunit;

namespace PadelManager.Tests;

public class HoraireSiteServiceTests {
    private readonly Mock<ISiteRepository> _siteRepoMock;
    private readonly Mock<IHoraireSiteRepository> _horaireRepoMock;
    private readonly Mock<IFermetureHebdoGlobaleRepository> _fermetureRepoMock;
    private readonly Mock<IDisponibiliteGenerationService> _generationServiceMock;
    private readonly HoraireSiteService _service;

    public HoraireSiteServiceTests() {
        _siteRepoMock = new Mock<ISiteRepository>();
        _horaireRepoMock = new Mock<IHoraireSiteRepository>();
        _fermetureRepoMock = new Mock<IFermetureHebdoGlobaleRepository>();
        _generationServiceMock = new Mock<IDisponibiliteGenerationService>();
        _service = new HoraireSiteService(
            _siteRepoMock.Object, _horaireRepoMock.Object, _fermetureRepoMock.Object, _generationServiceMock.Object);

        _siteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Site { Id = 1, Nom = "Site 1" });
        _fermetureRepoMock.Setup(r => r.GetByAnneeAsync(It.IsAny<short>())).ReturnsAsync((FermetureHebdoGlobale?)null);
    }

    private static HoraireSiteRequestDto RequeteValide() => new() {
        JoursOuverture = new List<string> { "VEN", "LUN", "MER" },
        HeureDebutReservation = new TimeOnly(9, 0),
        HeureFinReservation = new TimeOnly(21, 0)
    };

    [Fact]
    public async Task DefinirHoraireAsync_RequeteValide_EnregistreEtDeclencheLaGeneration() {
        // Act
        var resultat = await _service.DefinirHoraireAsync(1, 2026, RequeteValide());

        // Assert
        Assert.True(resultat.Succes);
        Assert.NotNull(resultat.Horaire);
        // Les jours sont ré-ordonnés dans l'ordre canonique LUN..DIM
        Assert.Equal(new[] { "LUN", "MER", "VEN" }, resultat.Horaire!.JoursOuverture);

        _horaireRepoMock.Verify(r => r.UpsertAsync(It.Is<HoraireSite>(h =>
            h.SiteId == 1 && h.Annee == 2026 && h.JoursOuverture == "LUN,MER,VEN")), Times.Once);
        _generationServiceMock.Verify(g => g.GenererPourSiteEtAnneeAsync(1, 2026), Times.Once);
    }

    [Fact]
    public async Task DefinirHoraireAsync_SiteInconnu_RetourneEchecSansEnregistrer() {
        // Arrange
        _siteRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Site?)null);

        // Act
        var resultat = await _service.DefinirHoraireAsync(99, 2026, RequeteValide());

        // Assert
        Assert.False(resultat.Succes);
        Assert.Null(resultat.Horaire);
        _horaireRepoMock.Verify(r => r.UpsertAsync(It.IsAny<HoraireSite>()), Times.Never);
        _generationServiceMock.Verify(g => g.GenererPourSiteEtAnneeAsync(It.IsAny<int>(), It.IsAny<short>()), Times.Never);
    }

    [Fact]
    public async Task DefinirHoraireAsync_AucunJourOuverture_RetourneEchec() {
        var requete = RequeteValide();
        requete.JoursOuverture = new List<string>();

        var resultat = await _service.DefinirHoraireAsync(1, 2026, requete);

        Assert.False(resultat.Succes);
        Assert.NotNull(resultat.MessageErreur);
    }

    [Fact]
    public async Task DefinirHoraireAsync_JourDuplique_RetourneEchec() {
        var requete = RequeteValide();
        requete.JoursOuverture = new List<string> { "LUN", "LUN" };

        var resultat = await _service.DefinirHoraireAsync(1, 2026, requete);

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task DefinirHoraireAsync_CodeJourInvalide_RetourneEchec() {
        var requete = RequeteValide();
        requete.JoursOuverture = new List<string> { "LUNDI" };

        var resultat = await _service.DefinirHoraireAsync(1, 2026, requete);

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task DefinirHoraireAsync_HeureDebutApresHeureFin_RetourneEchec() {
        var requete = RequeteValide();
        requete.HeureDebutReservation = new TimeOnly(21, 0);
        requete.HeureFinReservation = new TimeOnly(9, 0);

        var resultat = await _service.DefinirHoraireAsync(1, 2026, requete);

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task DefinirHoraireAsync_AnneeHorsBornes_RetourneEchec() {
        var resultat = await _service.DefinirHoraireAsync(1, 1999, RequeteValide());

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task DefinirHoraireAsync_ConflitAvecFermetureHebdoGlobale_RetourneEchec() {
        // Arrange : LUN est fermé partout en 2026, mais fait partie des jours demandés
        _fermetureRepoMock.Setup(r => r.GetByAnneeAsync(2026))
            .ReturnsAsync(new FermetureHebdoGlobale { Annee = 2026, JoursFermes = "LUN" });

        // Act
        var resultat = await _service.DefinirHoraireAsync(1, 2026, RequeteValide());

        // Assert
        Assert.False(resultat.Succes);
        Assert.Contains("LUN", resultat.MessageErreur);
        _horaireRepoMock.Verify(r => r.UpsertAsync(It.IsAny<HoraireSite>()), Times.Never);
    }

    [Fact]
    public async Task ObtenirHoraireAsync_HoraireExistant_RetourneLeDto() {
        // Arrange
        _horaireRepoMock.Setup(r => r.GetBySiteAndAnneeAsync(1, 2026)).ReturnsAsync(new HoraireSite {
            SiteId = 1,
            Annee = 2026,
            JoursOuverture = "LUN,MER",
            HeureDebutReservation = new TimeOnly(9, 0),
            HeureFinReservation = new TimeOnly(21, 0)
        });

        // Act
        var resultat = await _service.ObtenirHoraireAsync(1, 2026);

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal(new List<string> { "LUN", "MER" }, resultat!.JoursOuverture);
    }

    [Fact]
    public async Task ObtenirHoraireAsync_AucunHoraire_RetourneNull() {
        _horaireRepoMock.Setup(r => r.GetBySiteAndAnneeAsync(1, 2026)).ReturnsAsync((HoraireSite?)null);

        var resultat = await _service.ObtenirHoraireAsync(1, 2026);

        Assert.Null(resultat);
    }

    // Un site entièrement vidé de ses jours d'ouverture par FermetureHebdoGlobaleService (R-STR-006
    // asymétrique) stocke JoursOuverture = "" ; "".Split(',') renvoie [""] en C#, pas [] : le DTO
    // doit tout de même exposer une liste vide, pas une liste contenant une chaîne vide.
    [Fact]
    public async Task ObtenirHoraireAsync_HoraireVide_RetourneListeVide() {
        _horaireRepoMock.Setup(r => r.GetBySiteAndAnneeAsync(1, 2026)).ReturnsAsync(new HoraireSite {
            SiteId = 1,
            Annee = 2026,
            JoursOuverture = "",
            HeureDebutReservation = new TimeOnly(9, 0),
            HeureFinReservation = new TimeOnly(21, 0)
        });

        var resultat = await _service.ObtenirHoraireAsync(1, 2026);

        Assert.NotNull(resultat);
        Assert.Empty(resultat!.JoursOuverture);
    }
}

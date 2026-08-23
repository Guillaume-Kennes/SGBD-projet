using Moq;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;
using PadelManager.Services;
using Xunit;

namespace PadelManager.Tests;

public class FermetureHebdoGlobaleServiceTests {
    private readonly Mock<IHoraireSiteRepository> _horaireRepoMock;
    private readonly Mock<IFermetureHebdoGlobaleRepository> _fermetureRepoMock;
    private readonly Mock<IDisponibiliteGenerationService> _generationServiceMock;
    private readonly FermetureHebdoGlobaleService _service;

    public FermetureHebdoGlobaleServiceTests() {
        _horaireRepoMock = new Mock<IHoraireSiteRepository>();
        _fermetureRepoMock = new Mock<IFermetureHebdoGlobaleRepository>();
        _generationServiceMock = new Mock<IDisponibiliteGenerationService>();
        _service = new FermetureHebdoGlobaleService(_horaireRepoMock.Object, _fermetureRepoMock.Object, _generationServiceMock.Object);

        _horaireRepoMock.Setup(r => r.GetAllForAnneeAsync(It.IsAny<short>())).ReturnsAsync(new List<HoraireSite>());
    }

    private static FermetureHebdoGlobaleRequestDto RequeteValide() => new() {
        JoursFermes = new List<string> { "MER", "LUN" }
    };

    [Fact]
    public async Task DefinirAsync_RequeteValide_Enregistre() {
        // Act
        var resultat = await _service.DefinirAsync(2026, RequeteValide());

        // Assert
        Assert.True(resultat.Succes);
        Assert.NotNull(resultat.Fermeture);
        // Ordre canonique LUN..DIM
        Assert.Equal(new[] { "LUN", "MER" }, resultat.Fermeture!.JoursFermes);

        _fermetureRepoMock.Verify(r => r.UpsertAsync(It.Is<FermetureHebdoGlobale>(f =>
            f.Annee == 2026 && f.JoursFermes == "LUN,MER")), Times.Once);
    }

    [Fact]
    public async Task DefinirAsync_AucunSiteConcerne_NeRegenereAucunSite() {
        // Act (aucun HORAIRE_SITE configuré pour l'année, cf. setup par défaut)
        await _service.DefinirAsync(2026, RequeteValide());

        // Assert : aucun site à régénérer -> pas de coût inutile en base
        _generationServiceMock.Verify(g => g.GenererPourSiteEtAnneeAsync(It.IsAny<int>(), It.IsAny<short>()), Times.Never);
        _generationServiceMock.Verify(g => g.GenererPourTousLesSitesEtAnneeAsync(It.IsAny<short>()), Times.Never);
    }

    [Fact]
    public async Task DefinirAsync_AucunJourFerme_RetourneEchec() {
        // Act
        var resultat = await _service.DefinirAsync(2026, new FermetureHebdoGlobaleRequestDto { JoursFermes = new List<string>() });

        // Assert
        Assert.False(resultat.Succes);
        _fermetureRepoMock.Verify(r => r.UpsertAsync(It.IsAny<FermetureHebdoGlobale>()), Times.Never);
    }

    [Fact]
    public async Task DefinirAsync_JourDuplique_RetourneEchec() {
        var requete = new FermetureHebdoGlobaleRequestDto { JoursFermes = new List<string> { "LUN", "LUN" } };

        var resultat = await _service.DefinirAsync(2026, requete);

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task DefinirAsync_CodeJourInvalide_RetourneEchec() {
        var requete = new FermetureHebdoGlobaleRequestDto { JoursFermes = new List<string> { "LUNDI" } };

        var resultat = await _service.DefinirAsync(2026, requete);

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task DefinirAsync_AnneeHorsBornes_RetourneEchec() {
        var resultat = await _service.DefinirAsync(1999, RequeteValide());

        Assert.False(resultat.Succes);
    }

    // Nouveau sens de R-STR-006 (issue #6) : la fermeture hebdomadaire globale prime sur le
    // paramétrage local (CDC) -> l'écriture est acceptée, et le jour désormais fermé
    // globalement est automatiquement retiré du joursOuverture du site concerné.
    [Fact]
    public async Task DefinirAsync_ConflitAvecHoraireSiteExistant_AccepteEtRetireLeJourDeLHoraireDuSite() {
        // Arrange : le site 1 ouvre LUN et MAR ; la requête ferme globalement LUN et MER
        _horaireRepoMock.Setup(r => r.GetAllForAnneeAsync(2026)).ReturnsAsync(new List<HoraireSite> {
            new() { SiteId = 1, Annee = 2026, JoursOuverture = "LUN,MAR", HeureDebutReservation = new TimeOnly(9, 0), HeureFinReservation = new TimeOnly(21, 0) }
        });

        // Act
        var resultat = await _service.DefinirAsync(2026, RequeteValide());

        // Assert
        Assert.True(resultat.Succes);
        _fermetureRepoMock.Verify(r => r.UpsertAsync(It.Is<FermetureHebdoGlobale>(f => f.Annee == 2026 && f.JoursFermes == "LUN,MER")), Times.Once);
        // LUN retiré (désormais fermé globalement), MAR conservé (jamais concerné)
        _horaireRepoMock.Verify(r => r.UpsertAsync(It.Is<HoraireSite>(h => h.SiteId == 1 && h.JoursOuverture == "MAR")), Times.Once);
    }

    // Seul le site réellement modifié doit être régénéré, pas tous les sites du système
    // (sinon un simple paramétrage de fermeture hebdo globale régénère inutilement l'année
    // complète de sites jamais concernés, ce qui peut être très coûteux en base).
    [Fact]
    public async Task DefinirAsync_ConflitAvecHoraireSiteExistant_NeRegenereQueLeSiteModifie() {
        // Arrange
        _horaireRepoMock.Setup(r => r.GetAllForAnneeAsync(2026)).ReturnsAsync(new List<HoraireSite> {
            new() { SiteId = 1, Annee = 2026, JoursOuverture = "LUN,MAR", HeureDebutReservation = new TimeOnly(9, 0), HeureFinReservation = new TimeOnly(21, 0) }
        });

        // Act
        await _service.DefinirAsync(2026, RequeteValide());

        // Assert
        _generationServiceMock.Verify(g => g.GenererPourSiteEtAnneeAsync(1, 2026), Times.Once);
        _generationServiceMock.Verify(g => g.GenererPourTousLesSitesEtAnneeAsync(It.IsAny<short>()), Times.Never);
    }

    [Fact]
    public async Task DefinirAsync_PlusieursSites_NeCorrigeEtNeRegenereQueLesSitesConcernes() {
        // Arrange : site 1 ouvre LUN (concerné), site 2 ouvre MAR,JEU (pas concerné)
        _horaireRepoMock.Setup(r => r.GetAllForAnneeAsync(2026)).ReturnsAsync(new List<HoraireSite> {
            new() { SiteId = 1, Annee = 2026, JoursOuverture = "LUN", HeureDebutReservation = new TimeOnly(9, 0), HeureFinReservation = new TimeOnly(21, 0) },
            new() { SiteId = 2, Annee = 2026, JoursOuverture = "MAR,JEU", HeureDebutReservation = new TimeOnly(9, 0), HeureFinReservation = new TimeOnly(21, 0) }
        });

        // Act
        var resultat = await _service.DefinirAsync(2026, RequeteValide());

        // Assert
        Assert.True(resultat.Succes);
        _horaireRepoMock.Verify(r => r.UpsertAsync(It.Is<HoraireSite>(h => h.SiteId == 1)), Times.Once);
        _horaireRepoMock.Verify(r => r.UpsertAsync(It.Is<HoraireSite>(h => h.SiteId == 2)), Times.Never);
        _generationServiceMock.Verify(g => g.GenererPourSiteEtAnneeAsync(1, 2026), Times.Once);
        _generationServiceMock.Verify(g => g.GenererPourSiteEtAnneeAsync(2, It.IsAny<short>()), Times.Never);
    }

    [Fact]
    public async Task DefinirAsync_TousLesJoursDuSiteDesormaisFermes_ViteLHoraireDuSite() {
        // Arrange : le site 1 n'ouvrait que LUN, qui est désormais fermé globalement
        _horaireRepoMock.Setup(r => r.GetAllForAnneeAsync(2026)).ReturnsAsync(new List<HoraireSite> {
            new() { SiteId = 1, Annee = 2026, JoursOuverture = "LUN", HeureDebutReservation = new TimeOnly(9, 0), HeureFinReservation = new TimeOnly(21, 0) }
        });

        // Act
        var resultat = await _service.DefinirAsync(2026, RequeteValide());

        // Assert
        Assert.True(resultat.Succes);
        _horaireRepoMock.Verify(r => r.UpsertAsync(It.Is<HoraireSite>(h => h.SiteId == 1 && h.JoursOuverture == "")), Times.Once);
    }

    [Fact]
    public async Task DefinirAsync_SitesSansConflit_RetourneSuccesSansToucherAuxHorairesNiRegenerer() {
        // Arrange : le site n'ouvre que MAR, JEU -> aucun conflit avec LUN,MER
        _horaireRepoMock.Setup(r => r.GetAllForAnneeAsync(2026)).ReturnsAsync(new List<HoraireSite> {
            new() { SiteId = 1, Annee = 2026, JoursOuverture = "MAR,JEU", HeureDebutReservation = new TimeOnly(9, 0), HeureFinReservation = new TimeOnly(21, 0) }
        });

        // Act
        var resultat = await _service.DefinirAsync(2026, RequeteValide());

        // Assert
        Assert.True(resultat.Succes);
        _horaireRepoMock.Verify(r => r.UpsertAsync(It.IsAny<HoraireSite>()), Times.Never);
        _generationServiceMock.Verify(g => g.GenererPourSiteEtAnneeAsync(It.IsAny<int>(), It.IsAny<short>()), Times.Never);
    }

    [Fact]
    public async Task ObtenirAsync_FermetureExistante_RetourneLeDto() {
        // Arrange
        _fermetureRepoMock.Setup(r => r.GetByAnneeAsync(2026)).ReturnsAsync(new FermetureHebdoGlobale { Annee = 2026, JoursFermes = "LUN,MER" });

        // Act
        var resultat = await _service.ObtenirAsync(2026);

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal(new List<string> { "LUN", "MER" }, resultat!.JoursFermes);
    }

    [Fact]
    public async Task ObtenirAsync_Aucune_RetourneNull() {
        _fermetureRepoMock.Setup(r => r.GetByAnneeAsync(2026)).ReturnsAsync((FermetureHebdoGlobale?)null);

        var resultat = await _service.ObtenirAsync(2026);

        Assert.Null(resultat);
    }

    // La suppression ne restaure aucun HORAIRE_SITE (cf. commentaire du service) : aucun site
    // n'a donc de DISPONIBILITE à régénérer.
    [Fact]
    public async Task SupprimerAsync_AnneeExistante_SupprimeSansRegenererAucunSite() {
        // Arrange
        _fermetureRepoMock.Setup(r => r.GetByAnneeAsync(2026)).ReturnsAsync(new FermetureHebdoGlobale { Annee = 2026, JoursFermes = "LUN" });

        // Act
        var resultat = await _service.SupprimerAsync(2026);

        // Assert
        Assert.True(resultat);
        _fermetureRepoMock.Verify(r => r.DeleteAsync(2026), Times.Once);
        _generationServiceMock.Verify(g => g.GenererPourSiteEtAnneeAsync(It.IsAny<int>(), It.IsAny<short>()), Times.Never);
        _generationServiceMock.Verify(g => g.GenererPourTousLesSitesEtAnneeAsync(It.IsAny<short>()), Times.Never);
    }

    [Fact]
    public async Task SupprimerAsync_AnneeInexistante_RetourneFaux() {
        // Arrange
        _fermetureRepoMock.Setup(r => r.GetByAnneeAsync(2026)).ReturnsAsync((FermetureHebdoGlobale?)null);

        // Act
        var resultat = await _service.SupprimerAsync(2026);

        // Assert
        Assert.False(resultat);
        _fermetureRepoMock.Verify(r => r.DeleteAsync(It.IsAny<short>()), Times.Never);
    }
}

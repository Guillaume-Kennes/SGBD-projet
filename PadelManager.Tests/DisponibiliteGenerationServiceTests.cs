using Moq;
using PadelManager.Interfaces;
using PadelManager.Services;
using Xunit;

namespace PadelManager.Tests;

public class DisponibiliteGenerationServiceTests {
    private readonly Mock<IHoraireSiteRepository> _horaireRepoMock;
    private readonly Mock<IJourFermetureRepository> _jourFermetureRepoMock;
    private readonly Mock<IFermetureHebdoGlobaleRepository> _fermetureHebdoGlobaleRepoMock;
    private readonly Mock<IDisponibiliteRepository> _disponibiliteRepoMock;
    private readonly DisponibiliteGenerationService _service;

    public DisponibiliteGenerationServiceTests() {
        _horaireRepoMock = new Mock<IHoraireSiteRepository>();
        _jourFermetureRepoMock = new Mock<IJourFermetureRepository>();
        _fermetureHebdoGlobaleRepoMock = new Mock<IFermetureHebdoGlobaleRepository>();
        _disponibiliteRepoMock = new Mock<IDisponibiliteRepository>();
        _service = new DisponibiliteGenerationService(
            _horaireRepoMock.Object, _jourFermetureRepoMock.Object, _fermetureHebdoGlobaleRepoMock.Object, _disponibiliteRepoMock.Object);

        _jourFermetureRepoMock.Setup(r => r.GetForSiteAndAnneeAsync(It.IsAny<int>(), It.IsAny<short>()))
            .ReturnsAsync(new List<JourFermeture>());
        _fermetureHebdoGlobaleRepoMock.Setup(r => r.GetByAnneeAsync(It.IsAny<short>()))
            .ReturnsAsync((FermetureHebdoGlobale?)null);
    }

    private static DateOnly PremierJourDe(short annee, DayOfWeek jour) {
        var date = new DateOnly(annee, 1, 1);
        while (date.DayOfWeek != jour)
            date = date.AddDays(1);
        return date;
    }

    private async Task<List<Disponibilite>> GenererEtCapturer(int siteId, short annee) {
        List<Disponibilite>? capture = null;
        _disponibiliteRepoMock
            .Setup(r => r.RemplacerPourSiteEtAnneeAsync(siteId, annee, It.IsAny<List<Disponibilite>>()))
            .Callback<int, short, List<Disponibilite>>((_, _, liste) => capture = liste)
            .Returns(Task.CompletedTask);

        await _service.GenererPourSiteEtAnneeAsync(siteId, annee);
        return capture!;
    }

    [Fact]
    public async Task GenererPourSiteEtAnneeAsync_AucunHoraireConfigure_RetourneNull() {
        // Arrange
        _horaireRepoMock.Setup(r => r.GetBySiteAndAnneeAsync(1, 2026)).ReturnsAsync((HoraireSite?)null);

        // Act
        var resultat = await _service.GenererPourSiteEtAnneeAsync(1, 2026);

        // Assert
        Assert.Null(resultat);
        _disponibiliteRepoMock.Verify(r => r.RemplacerPourSiteEtAnneeAsync(It.IsAny<int>(), It.IsAny<short>(), It.IsAny<List<Disponibilite>>()), Times.Never);
    }

    // Reproduit l'horaire réel du site 1 (docs/horaires par site + correctif.txt) : LUN, MAR, MER,
    // VEN, DIM, 9h-21h -> 7 créneaux par jour ouvert.
    [Fact]
    public async Task GenererPourSiteEtAnneeAsync_JourOuvert_GenereLesSeptCreneauxAttendus() {
        // Arrange
        _horaireRepoMock.Setup(r => r.GetBySiteAndAnneeAsync(1, 2026)).ReturnsAsync(new HoraireSite {
            SiteId = 1,
            Annee = 2026,
            JoursOuverture = "LUN,MAR,MER,VEN,DIM",
            HeureDebutReservation = new TimeOnly(9, 0),
            HeureFinReservation = new TimeOnly(21, 0)
        });
        var lundi = PremierJourDe(2026, DayOfWeek.Monday);

        // Act
        var disponibilites = await GenererEtCapturer(1, 2026);

        // Assert
        var creneauxLundi = disponibilites.Where(d => d.Date == lundi).OrderBy(d => d.HeureDebut).ToList();
        var attendus = new (TimeOnly debut, TimeOnly fin)[] {
            (new TimeOnly(9, 0), new TimeOnly(10, 30)),
            (new TimeOnly(10, 45), new TimeOnly(12, 15)),
            (new TimeOnly(12, 30), new TimeOnly(14, 0)),
            (new TimeOnly(14, 15), new TimeOnly(15, 45)),
            (new TimeOnly(16, 0), new TimeOnly(17, 30)),
            (new TimeOnly(17, 45), new TimeOnly(19, 15)),
            (new TimeOnly(19, 30), new TimeOnly(21, 0)),
        };
        Assert.Equal(7, creneauxLundi.Count);
        for (int i = 0; i < attendus.Length; i++) {
            Assert.Equal(attendus[i].debut, creneauxLundi[i].HeureDebut);
            Assert.Equal(attendus[i].fin, creneauxLundi[i].HeureFin);
        }
    }

    // Reproduit l'horaire 2025 du site 1 : 8h-23h30 -> 9 créneaux.
    [Fact]
    public async Task GenererPourSiteEtAnneeAsync_HoraireLong_GenereNeufCreneaux() {
        // Arrange
        _horaireRepoMock.Setup(r => r.GetBySiteAndAnneeAsync(1, 2025)).ReturnsAsync(new HoraireSite {
            SiteId = 1,
            Annee = 2025,
            JoursOuverture = "LUN,MER,VEN,SAM,DIM",
            HeureDebutReservation = new TimeOnly(8, 0),
            HeureFinReservation = new TimeOnly(23, 30)
        });
        var lundi = PremierJourDe(2025, DayOfWeek.Monday);

        // Act
        var disponibilites = await GenererEtCapturer(1, 2025);

        // Assert
        var creneauxLundi = disponibilites.Where(d => d.Date == lundi).OrderBy(d => d.HeureDebut).ToList();
        Assert.Equal(9, creneauxLundi.Count);
        Assert.Equal(new TimeOnly(8, 0), creneauxLundi[0].HeureDebut);
        Assert.Equal(new TimeOnly(23, 30), creneauxLundi[^1].HeureFin);
    }

    [Fact]
    public async Task GenererPourSiteEtAnneeAsync_JourFermeParHoraire_AucunCreneau() {
        // Arrange : JEU n'est jamais dans joursOuverture
        _horaireRepoMock.Setup(r => r.GetBySiteAndAnneeAsync(1, 2026)).ReturnsAsync(new HoraireSite {
            SiteId = 1,
            Annee = 2026,
            JoursOuverture = "LUN,MAR,MER,VEN,DIM",
            HeureDebutReservation = new TimeOnly(9, 0),
            HeureFinReservation = new TimeOnly(21, 0)
        });

        // Act
        var disponibilites = await GenererEtCapturer(1, 2026);

        // Assert
        Assert.DoesNotContain(disponibilites, d => d.Date.DayOfWeek == DayOfWeek.Thursday);
    }

    [Fact]
    public async Task GenererPourSiteEtAnneeAsync_JourFermetureSiteSpecifique_ExclutLaDate() {
        // Arrange
        var lundi = PremierJourDe(2026, DayOfWeek.Monday);
        _horaireRepoMock.Setup(r => r.GetBySiteAndAnneeAsync(1, 2026)).ReturnsAsync(new HoraireSite {
            SiteId = 1,
            Annee = 2026,
            JoursOuverture = "LUN,MAR,MER,VEN,DIM",
            HeureDebutReservation = new TimeOnly(9, 0),
            HeureFinReservation = new TimeOnly(21, 0)
        });
        _jourFermetureRepoMock.Setup(r => r.GetForSiteAndAnneeAsync(1, 2026))
            .ReturnsAsync(new List<JourFermeture> { new() { SiteId = 1, Date = lundi } });

        // Act
        var disponibilites = await GenererEtCapturer(1, 2026);

        // Assert
        Assert.DoesNotContain(disponibilites, d => d.Date == lundi);
    }

    [Fact]
    public async Task GenererPourSiteEtAnneeAsync_JourFermetureGlobale_ExclutLaDate() {
        // Arrange : fermeture ponctuelle à siteId NULL (ex. Noël, décidée par l'admin global)
        var lundi = PremierJourDe(2026, DayOfWeek.Monday);
        _horaireRepoMock.Setup(r => r.GetBySiteAndAnneeAsync(1, 2026)).ReturnsAsync(new HoraireSite {
            SiteId = 1,
            Annee = 2026,
            JoursOuverture = "LUN,MAR,MER,VEN,DIM",
            HeureDebutReservation = new TimeOnly(9, 0),
            HeureFinReservation = new TimeOnly(21, 0)
        });
        _jourFermetureRepoMock.Setup(r => r.GetForSiteAndAnneeAsync(1, 2026))
            .ReturnsAsync(new List<JourFermeture> { new() { SiteId = null, Date = lundi } });

        // Act
        var disponibilites = await GenererEtCapturer(1, 2026);

        // Assert
        Assert.DoesNotContain(disponibilites, d => d.Date == lundi);
    }

    // Filet de sécurité R-STR-006 : si FERMETURE_HEBDO_GLOBALE est ajoutée après le paramétrage
    // de l'horaire (son CRUD n'est pas géré par cette issue), la (ré)génération doit quand même
    // exclure le jour concerné, même s'il fait partie de joursOuverture.
    [Fact]
    public async Task GenererPourSiteEtAnneeAsync_JourFermeHebdoGlobalementApresCoup_ExclutLeJour() {
        // Arrange
        _horaireRepoMock.Setup(r => r.GetBySiteAndAnneeAsync(1, 2026)).ReturnsAsync(new HoraireSite {
            SiteId = 1,
            Annee = 2026,
            JoursOuverture = "LUN,MAR,MER,VEN,DIM",
            HeureDebutReservation = new TimeOnly(9, 0),
            HeureFinReservation = new TimeOnly(21, 0)
        });
        _fermetureHebdoGlobaleRepoMock.Setup(r => r.GetByAnneeAsync(2026))
            .ReturnsAsync(new FermetureHebdoGlobale { Annee = 2026, JoursFermes = "LUN" });

        // Act
        var disponibilites = await GenererEtCapturer(1, 2026);

        // Assert
        Assert.DoesNotContain(disponibilites, d => d.Date.DayOfWeek == DayOfWeek.Monday);
        Assert.Contains(disponibilites, d => d.Date.DayOfWeek == DayOfWeek.Tuesday);
    }

    [Fact]
    public async Task GenererPourSiteEtAnneeAsync_RetourneLeNombreDeCreneauxGeneres() {
        // Arrange
        _horaireRepoMock.Setup(r => r.GetBySiteAndAnneeAsync(1, 2026)).ReturnsAsync(new HoraireSite {
            SiteId = 1,
            Annee = 2026,
            JoursOuverture = "LUN,MAR,MER,VEN,DIM",
            HeureDebutReservation = new TimeOnly(9, 0),
            HeureFinReservation = new TimeOnly(21, 0)
        });
        List<Disponibilite>? capture = null;
        _disponibiliteRepoMock
            .Setup(r => r.RemplacerPourSiteEtAnneeAsync(1, 2026, It.IsAny<List<Disponibilite>>()))
            .Callback<int, short, List<Disponibilite>>((_, _, liste) => capture = liste)
            .Returns(Task.CompletedTask);

        // Act
        var resultat = await _service.GenererPourSiteEtAnneeAsync(1, 2026);

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal(capture!.Count, resultat);
        Assert.True(resultat > 0);
    }
}

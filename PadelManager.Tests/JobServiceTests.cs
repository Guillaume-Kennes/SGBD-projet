using Moq;
using PadelManager.Interfaces;
using PadelManager.Services;
using Xunit;

namespace PadelManager.Tests;

public class JobServiceTests {
    private readonly Mock<IJobRepository> _jobRepoMock;
    private readonly JobService _service;

    public JobServiceTests() {
        _jobRepoMock = new Mock<IJobRepository>();
        _service = new JobService(_jobRepoMock.Object);
    }

    private static Match MatchAvecParticipations(int id, string organisateur, string visibilite, string statut, int nombrePayees, int nombreNonPayees) {
        var participations = new List<Participation>();
        for (var i = 0; i < nombrePayees; i++) {
            participations.Add(new Participation {
                Id = i + 1, MatchId = id, MembreMatricule = $"P{i}", DateInscription = DateTime.Now,
                Paiement = new Paiement { MontantParticipation = 15.00m, MontantDetteReportee = 0.00m, DatePaiement = DateTime.Now }
            });
        }
        for (var i = 0; i < nombreNonPayees; i++) {
            participations.Add(new Participation { Id = 100 + i, MatchId = id, MembreMatricule = $"NP{i}", DateInscription = DateTime.Now });
        }

        return new Match {
            Id = id, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0),
            Visibilite = visibilite, OrganisateurMatricule = organisateur, Statut = statut,
            Participations = participations
        };
    }

    // --- ExecuterBasculeAsync (EF-bk-009/010) ---

    [Fact]
    public async Task ExecuterBasculeAsync_MatchIncomplet_BasculeEnPublicEtPenalise() {
        // Arrange : organisateur payé + 1 joueur payé + 2 non payés (2/4 payés).
        var match = MatchAvecParticipations(1, "G001", "PRIVE", "INCOMPLET", nombrePayees: 2, nombreNonPayees: 2);
        _jobRepoMock.Setup(r => r.GetMatchsPrivesDeLaDateAsync(It.IsAny<DateOnly>())).ReturnsAsync(new List<Match> { match });

        var demain = DateOnly.FromDateTime(DateTime.Today).AddDays(1);

        // Act
        await _service.ExecuterBasculeAsync(demain);

        // Assert
        _jobRepoMock.Verify(r => r.BasculerAsync(
            match,
            It.Is<List<Participation>>(l => l.Count == 2 && l.All(p => p.Paiement == null)),
            It.Is<Penalite>(p =>
                p.MembreMatricule == "G001" &&
                p.MatchOrigineId == 1 &&
                p.DelaiJusquAu == DateOnly.FromDateTime(DateTime.Today).AddDays(7))),
            Times.Once);
    }

    [Fact]
    public async Task ExecuterBasculeAsync_MatchComplet_NestPasConcerne() {
        var match = MatchAvecParticipations(1, "G001", "PRIVE", "COMPLET", nombrePayees: 4, nombreNonPayees: 0);
        _jobRepoMock.Setup(r => r.GetMatchsPrivesDeLaDateAsync(It.IsAny<DateOnly>())).ReturnsAsync(new List<Match> { match });

        await _service.ExecuterBasculeAsync(DateOnly.FromDateTime(DateTime.Today).AddDays(1));

        _jobRepoMock.Verify(r => r.BasculerAsync(It.IsAny<Match>(), It.IsAny<List<Participation>>(), It.IsAny<Penalite>()), Times.Never);
    }

    [Fact]
    public async Task ExecuterBasculeAsync_PlusieursMatchs_NeBasculeQueLesIncomplets() {
        var matchIncomplet = MatchAvecParticipations(1, "G001", "PRIVE", "INCOMPLET", nombrePayees: 1, nombreNonPayees: 3);
        var matchComplet = MatchAvecParticipations(2, "S001", "PRIVE", "COMPLET", nombrePayees: 4, nombreNonPayees: 0);
        _jobRepoMock.Setup(r => r.GetMatchsPrivesDeLaDateAsync(It.IsAny<DateOnly>())).ReturnsAsync(new List<Match> { matchIncomplet, matchComplet });

        await _service.ExecuterBasculeAsync(DateOnly.FromDateTime(DateTime.Today).AddDays(1));

        _jobRepoMock.Verify(r => r.BasculerAsync(matchIncomplet, It.IsAny<List<Participation>>(), It.IsAny<Penalite>()), Times.Once);
        _jobRepoMock.Verify(r => r.BasculerAsync(matchComplet, It.IsAny<List<Participation>>(), It.IsAny<Penalite>()), Times.Never);
    }

    // --- ExecuterClotureAsync (EF-bk-008, R-VAL-004) ---

    [Fact]
    public async Task ExecuterClotureAsync_MatchIncomplet_CreeLaDetteEtScelleTermine() {
        // 4 - 2 payés = 30€ de dette.
        var match = MatchAvecParticipations(1, "G001", "PRIVE", "INCOMPLET", nombrePayees: 2, nombreNonPayees: 0);
        _jobRepoMock.Setup(r => r.GetMatchsDeLaDateAsync(It.IsAny<DateOnly>())).ReturnsAsync(new List<Match> { match });

        await _service.ExecuterClotureAsync(DateOnly.FromDateTime(DateTime.Today).AddDays(-1));

        _jobRepoMock.Verify(r => r.CreerDetteAsync(It.Is<Dette>(d =>
            d.MembreMatricule == "G001" && d.MatchOrigineId == 1 && d.Montant == 30.00m && !d.Soldee)), Times.Once);
        _jobRepoMock.Verify(r => r.ScellerTermineAsync(match), Times.Once);
    }

    [Fact]
    public async Task ExecuterClotureAsync_MatchComplet_PasDeDetteMaisScelleQuandMeme() {
        var match = MatchAvecParticipations(1, "G001", "PUBLIC", "COMPLET", nombrePayees: 4, nombreNonPayees: 0);
        _jobRepoMock.Setup(r => r.GetMatchsDeLaDateAsync(It.IsAny<DateOnly>())).ReturnsAsync(new List<Match> { match });

        await _service.ExecuterClotureAsync(DateOnly.FromDateTime(DateTime.Today).AddDays(-1));

        _jobRepoMock.Verify(r => r.CreerDetteAsync(It.IsAny<Dette>()), Times.Never);
        _jobRepoMock.Verify(r => r.ScellerTermineAsync(match), Times.Once);
    }

    // Idempotence (ENF-011) : une exécution précédente a déjà scellé ce match, une nouvelle
    // exécution ne doit ni recréer de dette ni rescellé.
    [Fact]
    public async Task ExecuterClotureAsync_MatchDejaScelleTermine_NeRetraitePas() {
        var match = MatchAvecParticipations(1, "G001", "PRIVE", "TERMINE", nombrePayees: 1, nombreNonPayees: 0);
        _jobRepoMock.Setup(r => r.GetMatchsDeLaDateAsync(It.IsAny<DateOnly>())).ReturnsAsync(new List<Match> { match });

        await _service.ExecuterClotureAsync(DateOnly.FromDateTime(DateTime.Today).AddDays(-1));

        _jobRepoMock.Verify(r => r.CreerDetteAsync(It.IsAny<Dette>()), Times.Never);
        _jobRepoMock.Verify(r => r.ScellerTermineAsync(It.IsAny<Match>()), Times.Never);
    }
}

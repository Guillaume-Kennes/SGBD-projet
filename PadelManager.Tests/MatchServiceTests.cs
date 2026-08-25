using Moq;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;
using PadelManager.Services;
using Xunit;

namespace PadelManager.Tests;

public class MatchServiceTests {
    private readonly Mock<ISiteRepository> _siteRepoMock;
    private readonly Mock<ITerrainRepository> _terrainRepoMock;
    private readonly Mock<IMembreRepository> _membreRepoMock;
    private readonly Mock<IDetteRepository> _detteRepoMock;
    private readonly Mock<IPenaliteRepository> _penaliteRepoMock;
    private readonly Mock<IDisponibiliteRepository> _disponibiliteRepoMock;
    private readonly Mock<IMatchRepository> _matchRepoMock;
    private readonly MatchService _service;

    private static readonly DateOnly Aujourdhui = DateOnly.FromDateTime(DateTime.Today);

    public MatchServiceTests() {
        _siteRepoMock = new Mock<ISiteRepository>();
        _terrainRepoMock = new Mock<ITerrainRepository>();
        _membreRepoMock = new Mock<IMembreRepository>();
        _detteRepoMock = new Mock<IDetteRepository>();
        _penaliteRepoMock = new Mock<IPenaliteRepository>();
        _disponibiliteRepoMock = new Mock<IDisponibiliteRepository>();
        _matchRepoMock = new Mock<IMatchRepository>();
        _service = new MatchService(
            _siteRepoMock.Object, _terrainRepoMock.Object, _membreRepoMock.Object, _detteRepoMock.Object,
            _penaliteRepoMock.Object, _disponibiliteRepoMock.Object, _matchRepoMock.Object);

        // Contexte par défaut : organisateur GLOBAL (21j), site 1 / terrain 11 existants, créneau
        // dans DISPONIBILITE, terrain libre, aucune dette ni pénalité, 3 joueurs valides prêts à
        // être invités (RequeteValide() est désormais "valide" au sens strict : un match privé
        // exige exactement 3 joueurs ajoutés).
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("G0001")).ReturnsAsync(MembreValide("G0001", "GLOBAL", null, 21));
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("L00001")).ReturnsAsync(MembreValide("L00001", "LIBRE", null, 5));
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("L00002")).ReturnsAsync(MembreValide("L00002", "LIBRE", null, 5));
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("L00003")).ReturnsAsync(MembreValide("L00003", "LIBRE", null, 5));
        _siteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Site { Id = 1, Nom = "Site 1" });
        _terrainRepoMock.Setup(r => r.GetByIdAsync(11)).ReturnsAsync(new Terrain { Id = 11, SiteId = 1, Numero = 1 });
        _detteRepoMock.Setup(r => r.ExisteDetteNonSoldeeAsync(It.IsAny<string>())).ReturnsAsync(false);
        _detteRepoMock.Setup(r => r.GetNonSoldeeAsync(It.IsAny<string>())).ReturnsAsync((Dette?)null);
        _penaliteRepoMock.Setup(r => r.GetPlusRecenteAsync(It.IsAny<string>())).ReturnsAsync((Penalite?)null);
        _disponibiliteRepoMock.Setup(r => r.ExisteAsync(1, It.IsAny<DateOnly>(), It.IsAny<TimeOnly>())).ReturnsAsync(true);
        _matchRepoMock.Setup(r => r.ExisteAsync(It.IsAny<int>(), It.IsAny<DateTime>())).ReturnsAsync(false);
        _matchRepoMock.Setup(r => r.AddAsync(It.IsAny<Match>())).ReturnsAsync((Match m) => { m.Id = 42; return m; });
    }

    private static Membre MembreValide(string matricule, string type, int? siteId, int anticipationMaxJours) => new() {
        Matricule = matricule,
        TypeMembre = type,
        SiteId = siteId,
        TypeMembreNavigation = new TypeMembre { Code = type, AnticipationMaxJours = anticipationMaxJours, Libelle = type, PrefixeMatricule = "X" }
    };

    private static CreerMatchPriveRequestDto RequeteValide() => new() {
        OrganisateurMatricule = "G0001",
        SiteId = 1,
        TerrainId = 11,
        Date = Aujourdhui.AddDays(1),
        HeureDebut = new TimeOnly(9, 0),
        Joueurs = new List<string> { "L00001", "L00002", "L00003" }
    };

    [Fact]
    public async Task CreerMatchPriveAsync_RequeteValide_CreeLeMatchAvecParticipationEtPaiementOrganisateurEtTroisJoueurs() {
        // Act
        var resultat = await _service.CreerMatchPriveAsync(RequeteValide());

        // Assert
        Assert.True(resultat.Succes);
        Assert.NotNull(resultat.Match);
        Assert.Equal("INCOMPLET", resultat.Match!.Statut);
        Assert.Equal("PRIVE", resultat.Match.Visibilite);
        Assert.Equal(new[] { "G0001", "L00001", "L00002", "L00003" }, resultat.Match.Joueurs);

        _matchRepoMock.Verify(r => r.AddAsync(It.Is<Match>(m =>
            m.SiteId == 1 && m.TerrainId == 11 && m.OrganisateurMatricule == "G0001" &&
            m.Statut == "INCOMPLET" && m.Visibilite == "PRIVE" &&
            m.Participations.Count == 4 &&
            m.Participations.Single(p => p.MembreMatricule == "G0001").Paiement != null &&
            m.Participations.Single(p => p.MembreMatricule == "G0001").Paiement!.MontantParticipation == 15.00m &&
            m.Participations.Single(p => p.MembreMatricule == "L00001").Paiement == null &&
            m.Participations.Single(p => p.MembreMatricule == "L00002").Paiement == null &&
            m.Participations.Single(p => p.MembreMatricule == "L00003").Paiement == null)), Times.Once);
    }

    [Fact]
    public async Task CreerMatchPriveAsync_OrganisateurIntrouvable_RetourneEchec() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("XXXX")).ReturnsAsync((Membre?)null);
        var requete = RequeteValide();
        requete.OrganisateurMatricule = "XXXX";

        var resultat = await _service.CreerMatchPriveAsync(requete);

        Assert.False(resultat.Succes);
        _matchRepoMock.Verify(r => r.AddAsync(It.IsAny<Match>()), Times.Never);
    }

    [Fact]
    public async Task CreerMatchPriveAsync_SiteIntrouvable_RetourneEchec() {
        _siteRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Site?)null);
        var requete = RequeteValide();
        requete.SiteId = 99;

        var resultat = await _service.CreerMatchPriveAsync(requete);

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task CreerMatchPriveAsync_TerrainIntrouvable_RetourneEchec() {
        _terrainRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Terrain?)null);
        var requete = RequeteValide();
        requete.TerrainId = 999;

        var resultat = await _service.CreerMatchPriveAsync(requete);

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task CreerMatchPriveAsync_TerrainDUnAutreSite_RetourneEchec() {
        _terrainRepoMock.Setup(r => r.GetByIdAsync(21)).ReturnsAsync(new Terrain { Id = 21, SiteId = 2, Numero = 1 });
        var requete = RequeteValide();
        requete.TerrainId = 21;

        var resultat = await _service.CreerMatchPriveAsync(requete);

        Assert.False(resultat.Succes);
    }

    // R-ACC-002 / EF-bk-012
    [Fact]
    public async Task CreerMatchPriveAsync_MembreSiteSurUnAutreSite_RetourneEchec() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("S00003")).ReturnsAsync(MembreValide("S00003", "SITE", 2, 14));
        var requete = RequeteValide();
        requete.OrganisateurMatricule = "S00003"; // rattaché au site 2, match sur le site 1

        var resultat = await _service.CreerMatchPriveAsync(requete);

        Assert.False(resultat.Succes);
        _matchRepoMock.Verify(r => r.AddAsync(It.IsAny<Match>()), Times.Never);
    }

    [Fact]
    public async Task CreerMatchPriveAsync_MembreSiteSurSonPropreSite_Autorise() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("S00001")).ReturnsAsync(MembreValide("S00001", "SITE", 1, 14));
        var requete = RequeteValide();
        requete.OrganisateurMatricule = "S00001";

        var resultat = await _service.CreerMatchPriveAsync(requete);

        Assert.True(resultat.Succes);
    }

    // R-ACC-006
    [Fact]
    public async Task CreerMatchPriveAsync_DetteNonSoldee_RetourneEchec() {
        _detteRepoMock.Setup(r => r.ExisteDetteNonSoldeeAsync("G0001")).ReturnsAsync(true);

        var resultat = await _service.CreerMatchPriveAsync(RequeteValide());

        Assert.False(resultat.Succes);
        _matchRepoMock.Verify(r => r.AddAsync(It.IsAny<Match>()), Times.Never);
    }

    // R-CALC-004 : blocage total tant que la pénalité n'est pas atteinte
    [Fact]
    public async Task CreerMatchPriveAsync_PenaliteActive_RetourneEchecMemePourUneDateEloignee() {
        _penaliteRepoMock.Setup(r => r.GetPlusRecenteAsync("G0001"))
            .ReturnsAsync(new Penalite { MembreMatricule = "G0001", MatchOrigineId = 1, DelaiJusquAu = Aujourdhui.AddDays(7) });
        var requete = RequeteValide();
        requete.Date = Aujourdhui.AddDays(20); // dans la fenêtre normale (21j), mais pénalité active

        var resultat = await _service.CreerMatchPriveAsync(requete);

        Assert.False(resultat.Succes);
        _matchRepoMock.Verify(r => r.AddAsync(It.IsAny<Match>()), Times.Never);
    }

    [Fact]
    public async Task CreerMatchPriveAsync_PenaliteExpiree_Autorise() {
        _penaliteRepoMock.Setup(r => r.GetPlusRecenteAsync("G0001"))
            .ReturnsAsync(new Penalite { MembreMatricule = "G0001", MatchOrigineId = 1, DelaiJusquAu = Aujourdhui.AddDays(-1) });

        var resultat = await _service.CreerMatchPriveAsync(RequeteValide());

        Assert.True(resultat.Succes);
    }

    [Fact]
    public async Task CreerMatchPriveAsync_DateDansLePasse_RetourneEchec() {
        var requete = RequeteValide();
        requete.Date = Aujourdhui.AddDays(-1);

        var resultat = await _service.CreerMatchPriveAsync(requete);

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task CreerMatchPriveAsync_DateTropLointaine_RetourneEchec() {
        var requete = RequeteValide();
        requete.Date = Aujourdhui.AddDays(22); // > 21j pour un membre GLOBAL

        var resultat = await _service.CreerMatchPriveAsync(requete);

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task CreerMatchPriveAsync_DateAujourdhui_Autorise() {
        var requete = RequeteValide();
        requete.Date = Aujourdhui;

        var resultat = await _service.CreerMatchPriveAsync(requete);

        Assert.True(resultat.Succes);
    }

    [Fact]
    public async Task CreerMatchPriveAsync_DateALaLimiteDAnticipation_Autorise() {
        var requete = RequeteValide();
        requete.Date = Aujourdhui.AddDays(21); // exactement la limite pour GLOBAL

        var resultat = await _service.CreerMatchPriveAsync(requete);

        Assert.True(resultat.Succes);
    }

    // EF-bk-020
    [Fact]
    public async Task CreerMatchPriveAsync_CreneauHorsDisponibilite_RetourneEchec() {
        _disponibiliteRepoMock.Setup(r => r.ExisteAsync(1, It.IsAny<DateOnly>(), It.IsAny<TimeOnly>())).ReturnsAsync(false);

        var resultat = await _service.CreerMatchPriveAsync(RequeteValide());

        Assert.False(resultat.Succes);
    }

    // EF-bk-019
    [Fact]
    public async Task CreerMatchPriveAsync_TerrainDejaPris_RetourneEchec() {
        _matchRepoMock.Setup(r => r.ExisteAsync(11, It.IsAny<DateTime>())).ReturnsAsync(true);

        var resultat = await _service.CreerMatchPriveAsync(RequeteValide());

        Assert.False(resultat.Succes);
        _matchRepoMock.Verify(r => r.AddAsync(It.IsAny<Match>()), Times.Never);
    }

    [Fact]
    public async Task CreerMatchPriveAsync_ConflitConcurrentALEcriture_TraduitEnEchec() {
        _matchRepoMock.Setup(r => r.AddAsync(It.IsAny<Match>())).ThrowsAsync(new CreneauIndisponibleException());

        var resultat = await _service.CreerMatchPriveAsync(RequeteValide());

        Assert.False(resultat.Succes);
    }

    // R-STR-002
    [Fact]
    public async Task CreerMatchPriveAsync_PlusDeTroisJoueurs_RetourneEchec() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync(It.Is<string>(m => m != "G0001"))).ReturnsAsync(MembreValide("X", "LIBRE", null, 5));
        var requete = RequeteValide();
        requete.Joueurs = new List<string> { "L00001", "L00002", "L00003", "L00004" };

        var resultat = await _service.CreerMatchPriveAsync(requete);

        Assert.False(resultat.Succes);
    }

    // EF-bk-004 : un match privé compte toujours exactement 4 participants (organisateur inclus),
    // pas moins — le bug initialement rapporté acceptait 1 ou 2 joueurs ajoutés à tort.
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task CreerMatchPriveAsync_MoinsDeTroisJoueurs_RetourneEchec(int nombreJoueurs) {
        var requete = RequeteValide();
        requete.Joueurs = new List<string> { "L00001", "L00002", "L00003" }.Take(nombreJoueurs).ToList();

        var resultat = await _service.CreerMatchPriveAsync(requete);

        Assert.False(resultat.Succes);
        _matchRepoMock.Verify(r => r.AddAsync(It.IsAny<Match>()), Times.Never);
    }

    [Fact]
    public async Task CreerMatchPriveAsync_JoueurDuplique_RetourneEchec() {
        var requete = RequeteValide();
        requete.Joueurs = new List<string> { "L00001", "L00001", "L00002" };

        var resultat = await _service.CreerMatchPriveAsync(requete);

        Assert.False(resultat.Succes);
    }

    // R-ACC-005
    [Fact]
    public async Task CreerMatchPriveAsync_JoueurEstLOrganisateur_RetourneEchec() {
        var requete = RequeteValide();
        requete.Joueurs = new List<string> { "G0001", "L00001", "L00002" };

        var resultat = await _service.CreerMatchPriveAsync(requete);

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task CreerMatchPriveAsync_JoueurIntrouvable_RetourneEchec() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("INCONNU")).ReturnsAsync((Membre?)null);
        var requete = RequeteValide();
        requete.Joueurs = new List<string> { "INCONNU", "L00001", "L00002" };

        var resultat = await _service.CreerMatchPriveAsync(requete);

        Assert.False(resultat.Succes);
    }

    // --- CreerMatchPublicAsync (EF-bk-002) ---

    private static CreerMatchPublicRequestDto RequeteValidePublic() => new() {
        OrganisateurMatricule = "G0001",
        SiteId = 1,
        TerrainId = 11,
        Date = Aujourdhui.AddDays(1),
        HeureDebut = new TimeOnly(9, 0)
    };

    [Fact]
    public async Task CreerMatchPublicAsync_RequeteValide_CreeLeMatchAvecSeuleLaParticipationDeLOrganisateur() {
        // Act
        var resultat = await _service.CreerMatchPublicAsync(RequeteValidePublic());

        // Assert
        Assert.True(resultat.Succes);
        Assert.NotNull(resultat.Match);
        Assert.Equal("PUBLIC", resultat.Match!.Visibilite);
        Assert.Equal("INCOMPLET", resultat.Match.Statut);
        Assert.Equal(new[] { "G0001" }, resultat.Match.Joueurs);

        _matchRepoMock.Verify(r => r.AddAsync(It.Is<Match>(m =>
            m.Visibilite == "PUBLIC" && m.OrganisateurMatricule == "G0001" &&
            m.Participations.Count == 1 &&
            m.Participations.First().Paiement != null &&
            m.Participations.First().Paiement!.MontantParticipation == 15.00m)), Times.Once);
    }

    // R-ACC-005 : aucun joueur ajoutable à la création d'un match public (à la différence de
    // CreerMatchPriveAsync, la requête n'a même pas de champ Joueurs).
    [Fact]
    public async Task CreerMatchPublicAsync_OrganisateurIntrouvable_RetourneEchec() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("XXXX")).ReturnsAsync((Membre?)null);
        var requete = RequeteValidePublic();
        requete.OrganisateurMatricule = "XXXX";

        var resultat = await _service.CreerMatchPublicAsync(requete);

        Assert.False(resultat.Succes);
        _matchRepoMock.Verify(r => r.AddAsync(It.IsAny<Match>()), Times.Never);
    }

    [Fact]
    public async Task CreerMatchPublicAsync_MembreSiteSurUnAutreSite_RetourneEchec() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("S00003")).ReturnsAsync(MembreValide("S00003", "SITE", 2, 14));
        var requete = RequeteValidePublic();
        requete.OrganisateurMatricule = "S00003";

        var resultat = await _service.CreerMatchPublicAsync(requete);

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task CreerMatchPublicAsync_DetteNonSoldee_RetourneEchec() {
        _detteRepoMock.Setup(r => r.ExisteDetteNonSoldeeAsync("G0001")).ReturnsAsync(true);

        var resultat = await _service.CreerMatchPublicAsync(RequeteValidePublic());

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task CreerMatchPublicAsync_PenaliteActive_RetourneEchec() {
        _penaliteRepoMock.Setup(r => r.GetPlusRecenteAsync("G0001"))
            .ReturnsAsync(new Penalite { MembreMatricule = "G0001", MatchOrigineId = 1, DelaiJusquAu = Aujourdhui.AddDays(7) });

        var resultat = await _service.CreerMatchPublicAsync(RequeteValidePublic());

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task CreerMatchPublicAsync_DateTropLointaine_RetourneEchec() {
        var requete = RequeteValidePublic();
        requete.Date = Aujourdhui.AddDays(22); // > 21j pour un membre GLOBAL

        var resultat = await _service.CreerMatchPublicAsync(requete);

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task CreerMatchPublicAsync_CreneauHorsDisponibilite_RetourneEchec() {
        _disponibiliteRepoMock.Setup(r => r.ExisteAsync(1, It.IsAny<DateOnly>(), It.IsAny<TimeOnly>())).ReturnsAsync(false);

        var resultat = await _service.CreerMatchPublicAsync(RequeteValidePublic());

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task CreerMatchPublicAsync_TerrainDejaPris_RetourneEchec() {
        _matchRepoMock.Setup(r => r.ExisteAsync(11, It.IsAny<DateTime>())).ReturnsAsync(true);

        var resultat = await _service.CreerMatchPublicAsync(RequeteValidePublic());

        Assert.False(resultat.Succes);
        _matchRepoMock.Verify(r => r.AddAsync(It.IsAny<Match>()), Times.Never);
    }

    [Fact]
    public async Task CreerMatchPublicAsync_ConflitConcurrentALEcriture_TraduitEnEchec() {
        _matchRepoMock.Setup(r => r.AddAsync(It.IsAny<Match>())).ThrowsAsync(new CreneauIndisponibleException());

        var resultat = await _service.CreerMatchPublicAsync(RequeteValidePublic());

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task ObtenirCreneauxDisponiblesAsync_SiteInconnu_RetourneNull() {
        _siteRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Site?)null);

        var resultat = await _service.ObtenirCreneauxDisponiblesAsync(99, Aujourdhui);

        Assert.Null(resultat);
    }

    [Fact]
    public async Task ObtenirCreneauxDisponiblesAsync_CroiseDisponibiliteEtTerrainsEnExcluantLesMatchsPris() {
        // Arrange : 2 créneaux DISPONIBILITE x 2 terrains = 4 combinaisons, dont 1 déjà prise
        var date = Aujourdhui.AddDays(1);
        _disponibiliteRepoMock.Setup(r => r.GetBySiteAndPeriodeAsync(1, date, date)).ReturnsAsync(new List<Disponibilite> {
            new() { SiteId = 1, Date = date, HeureDebut = new TimeOnly(9, 0), HeureFin = new TimeOnly(10, 30) },
            new() { SiteId = 1, Date = date, HeureDebut = new TimeOnly(10, 45), HeureFin = new TimeOnly(12, 15) }
        });
        _terrainRepoMock.Setup(r => r.GetBySiteIdAsync(1)).ReturnsAsync(new List<Terrain> {
            new() { Id = 11, SiteId = 1, Numero = 1 },
            new() { Id = 12, SiteId = 1, Numero = 2 }
        });
        _matchRepoMock.Setup(r => r.GetForSiteAndDateAsync(1, date)).ReturnsAsync(new List<Match> {
            new() { SiteId = 1, TerrainId = 11, DateHeure = date.ToDateTime(new TimeOnly(9, 0)), Visibilite = "PRIVE", OrganisateurMatricule = "G0001", Statut = "INCOMPLET" }
        });

        // Act
        var resultat = await _service.ObtenirCreneauxDisponiblesAsync(1, date);

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal(3, resultat!.Count);
        Assert.DoesNotContain(resultat, c => c.TerrainId == 11 && c.HeureDebut == new TimeOnly(9, 0));
        Assert.Contains(resultat, c => c.TerrainId == 12 && c.HeureDebut == new TimeOnly(9, 0));
    }

    // --- ObtenirMatchsPublicsAsync / RejoindreMatchPublicAsync (EF-bk-005/006/007/018) ---

    private static Match MatchPublic(int id, int siteId, DateTime dateHeure, params string[] participants) {
        var match = new Match {
            Id = id,
            SiteId = siteId,
            TerrainId = 11,
            DateHeure = dateHeure,
            Visibilite = "PUBLIC",
            OrganisateurMatricule = participants.Length > 0 ? participants[0] : "G0001",
            Statut = "INCOMPLET",
            Site = new Site { Id = siteId, Nom = $"Site {siteId}" },
            Terrain = new Terrain { Id = 11, SiteId = siteId, Numero = 1 }
        };
        foreach (var p in participants)
            match.Participations.Add(new Participation { MembreMatricule = p, DateInscription = DateTime.Now });
        return match;
    }

    [Fact]
    public async Task ObtenirMatchsPublicsAsync_MembreInconnu_RetourneNull() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("XXXX")).ReturnsAsync((Membre?)null);

        var resultat = await _service.ObtenirMatchsPublicsAsync("XXXX");

        Assert.Null(resultat);
    }

    [Fact]
    public async Task ObtenirMatchsPublicsAsync_MembreGlobal_VoitTousLesSites() {
        _matchRepoMock.Setup(r => r.GetPublicsIncompletsAsync(It.IsAny<DateTime>())).ReturnsAsync(new List<Match> {
            MatchPublic(1, 1, DateTime.Now.AddDays(1), "L00001"),
            MatchPublic(2, 2, DateTime.Now.AddDays(20), "L00001")
        });

        var resultat = await _service.ObtenirMatchsPublicsAsync("G0001");

        Assert.NotNull(resultat);
        Assert.Equal(2, resultat!.Count);
    }

    [Fact]
    public async Task ObtenirMatchsPublicsAsync_MembreSite_NeVoitQueSonSite() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("S00001")).ReturnsAsync(MembreValide("S00001", "SITE", 1, 14));
        _matchRepoMock.Setup(r => r.GetPublicsIncompletsAsync(It.IsAny<DateTime>())).ReturnsAsync(new List<Match> {
            MatchPublic(1, 1, DateTime.Now.AddDays(1), "L00001"),
            MatchPublic(2, 2, DateTime.Now.AddDays(1), "L00001")
        });

        var resultat = await _service.ObtenirMatchsPublicsAsync("S00001");

        Assert.NotNull(resultat);
        Assert.Single(resultat!);
        Assert.Equal(1, resultat![0].SiteId);
    }

    // R-VAL-003 (CDC v0.11) : l'anticipation maximum par type de membre ne borne que la création
    // d'un match, jamais la consultation — un membre Libre voit tous les sites, sans aucune limite
    // de délai, y compris un match très éloigné dans le temps.
    [Fact]
    public async Task ObtenirMatchsPublicsAsync_MembreLibre_VoitSansLimiteDeDelai() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("L00001")).ReturnsAsync(MembreValide("L00001", "LIBRE", null, 5));
        _matchRepoMock.Setup(r => r.GetPublicsIncompletsAsync(It.IsAny<DateTime>())).ReturnsAsync(new List<Match> {
            MatchPublic(1, 1, DateTime.Now.AddDays(5), "G0001"),
            MatchPublic(2, 1, DateTime.Now.AddDays(60), "G0001")
        });

        var resultat = await _service.ObtenirMatchsPublicsAsync("L00001");

        Assert.NotNull(resultat);
        Assert.Equal(2, resultat!.Count);
    }

    [Fact]
    public async Task ObtenirMatchsPublicsAsync_MembreDejaInscrit_NeLeVoitPlus() {
        _matchRepoMock.Setup(r => r.GetPublicsIncompletsAsync(It.IsAny<DateTime>())).ReturnsAsync(new List<Match> {
            MatchPublic(1, 1, DateTime.Now.AddDays(1), "G0001") // G0001 déjà organisateur/participant
        });

        var resultat = await _service.ObtenirMatchsPublicsAsync("G0001");

        Assert.NotNull(resultat);
        Assert.Empty(resultat!);
    }

    [Fact]
    public async Task ObtenirMatchsPublicsAsync_CalculeLesPlacesRestantes() {
        _matchRepoMock.Setup(r => r.GetPublicsIncompletsAsync(It.IsAny<DateTime>())).ReturnsAsync(new List<Match> {
            MatchPublic(1, 1, DateTime.Now.AddDays(1), "L00001", "L00002")
        });

        var resultat = await _service.ObtenirMatchsPublicsAsync("G0001");

        Assert.Equal(2, resultat![0].PlacesRestantes);
    }

    [Fact]
    public async Task RejoindreMatchPublicAsync_MembreInconnu_RetourneEchec() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("XXXX")).ReturnsAsync((Membre?)null);

        var resultat = await _service.RejoindreMatchPublicAsync(1, "XXXX");

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task RejoindreMatchPublicAsync_MatchIntrouvable_RetourneEchec() {
        _matchRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Match?)null);

        var resultat = await _service.RejoindreMatchPublicAsync(99, "G0001");

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task RejoindreMatchPublicAsync_MatchPrive_RetourneEchec() {
        _matchRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(
            new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now.AddDays(1), Visibilite = "PRIVE", OrganisateurMatricule = "G0001", Statut = "INCOMPLET" });

        var resultat = await _service.RejoindreMatchPublicAsync(1, "L00001");

        Assert.False(resultat.Succes);
        _matchRepoMock.Verify(r => r.InscrireEtPayerAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Dette>()), Times.Never);
    }

    [Fact]
    public async Task RejoindreMatchPublicAsync_MatchDejaCommence_RetourneEchec() {
        _matchRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(
            new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now.AddHours(-1), Visibilite = "PUBLIC", OrganisateurMatricule = "G0001", Statut = "INCOMPLET" });

        var resultat = await _service.RejoindreMatchPublicAsync(1, "L00001");

        Assert.False(resultat.Succes);
    }

    // R-ACC-002
    [Fact]
    public async Task RejoindreMatchPublicAsync_MembreSiteSurUnAutreSite_RetourneEchec() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("S00003")).ReturnsAsync(MembreValide("S00003", "SITE", 2, 14));
        _matchRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(
            new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now.AddDays(1), Visibilite = "PUBLIC", OrganisateurMatricule = "G0001", Statut = "INCOMPLET" });

        var resultat = await _service.RejoindreMatchPublicAsync(1, "S00003");

        Assert.False(resultat.Succes);
        _matchRepoMock.Verify(r => r.InscrireEtPayerAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Dette>()), Times.Never);
    }

    [Fact]
    public async Task RejoindreMatchPublicAsync_MembreSiteSurSonPropreSite_Autorise() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("S00001")).ReturnsAsync(MembreValide("S00001", "SITE", 1, 14));
        _matchRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(
            new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now.AddDays(30), Visibilite = "PUBLIC", OrganisateurMatricule = "G0001", Statut = "INCOMPLET" });

        var resultat = await _service.RejoindreMatchPublicAsync(1, "S00001");

        // Aucune restriction de délai pour un membre de site qui rejoint (seulement pour organiser).
        Assert.True(resultat.Succes);
    }

    // R-VAL-003 (CDC v0.11) : l'anticipation maximum par type de membre ne borne que la création
    // d'un match, jamais l'inscription à une place libre — aucune limite de délai pour Libre.
    [Fact]
    public async Task RejoindreMatchPublicAsync_MembreLibreSansLimiteDeDelai_Autorise() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("L00001")).ReturnsAsync(MembreValide("L00001", "LIBRE", null, 5));
        _matchRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(
            new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now.AddDays(60), Visibilite = "PUBLIC", OrganisateurMatricule = "G0001", Statut = "INCOMPLET" });

        var resultat = await _service.RejoindreMatchPublicAsync(1, "L00001");

        Assert.True(resultat.Succes);
    }

    // R-ACC-006 / EF-bk-018 : contrairement à la création, une dette ne bloque pas l'inscription
    // — elle est réglée automatiquement.
    [Fact]
    public async Task RejoindreMatchPublicAsync_DetteActive_NEmpecheJamaisEtEstReglee() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("G0002")).ReturnsAsync(MembreValide("G0002", "GLOBAL", null, 21));
        _detteRepoMock.Setup(r => r.GetNonSoldeeAsync("G0002")).ReturnsAsync(new Dette { Id = 7, MembreMatricule = "G0002", MatchOrigineId = 5, Montant = 30.00m, Soldee = false });
        _matchRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(
            new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now.AddDays(1), Visibilite = "PUBLIC", OrganisateurMatricule = "G0001", Statut = "INCOMPLET" });

        var resultat = await _service.RejoindreMatchPublicAsync(1, "G0002");

        Assert.True(resultat.Succes);
        Assert.True(resultat.DetteReglee);
        Assert.Equal(45.00m, resultat.MontantPaye); // 15€ + 30€ de dette reportée
        _matchRepoMock.Verify(r => r.InscrireEtPayerAsync(1, "G0002", It.Is<Dette>(d => d.Id == 7)), Times.Once);
    }

    // R-CALC-004 : une pénalité active ne bloque pas non plus l'inscription (à la différence de
    // la création, cf. ValiderCreationAsync).
    [Fact]
    public async Task RejoindreMatchPublicAsync_PenaliteActive_NEmpechePasLInscription() {
        _penaliteRepoMock.Setup(r => r.GetPlusRecenteAsync("G0001")).ReturnsAsync(new Penalite { MembreMatricule = "G0001", MatchOrigineId = 1, DelaiJusquAu = Aujourdhui.AddDays(30) });
        _matchRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(
            new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now.AddDays(1), Visibilite = "PUBLIC", OrganisateurMatricule = "G0002", Statut = "INCOMPLET" });

        var resultat = await _service.RejoindreMatchPublicAsync(1, "G0001");

        Assert.True(resultat.Succes);
    }

    [Fact]
    public async Task RejoindreMatchPublicAsync_MatchComplet_RetourneEchec() {
        _matchRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(
            new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now.AddDays(1), Visibilite = "PUBLIC", OrganisateurMatricule = "G0001", Statut = "INCOMPLET" });
        _matchRepoMock.Setup(r => r.InscrireEtPayerAsync(1, "L00001", It.IsAny<Dette>())).ThrowsAsync(new MatchCompletException());

        var resultat = await _service.RejoindreMatchPublicAsync(1, "L00001");

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task RejoindreMatchPublicAsync_DejaInscrit_RetourneEchec() {
        _matchRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(
            new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now.AddDays(1), Visibilite = "PUBLIC", OrganisateurMatricule = "G0001", Statut = "INCOMPLET" });
        _matchRepoMock.Setup(r => r.InscrireEtPayerAsync(1, "L00001", It.IsAny<Dette>())).ThrowsAsync(new DejaInscritException());

        var resultat = await _service.RejoindreMatchPublicAsync(1, "L00001");

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task RejoindreMatchPublicAsync_SansDette_MontantPayeEst15() {
        _matchRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(
            new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now.AddDays(1), Visibilite = "PUBLIC", OrganisateurMatricule = "G0002", Statut = "INCOMPLET" });

        var resultat = await _service.RejoindreMatchPublicAsync(1, "G0001");

        Assert.True(resultat.Succes);
        Assert.False(resultat.DetteReglee);
        Assert.Equal(15.00m, resultat.MontantPaye);
    }

    // --- ObtenirMontantAPayerAsync / PayerParticipationAsync (EF-bk-007) ---

    [Fact]
    public async Task ObtenirMontantAPayerAsync_SansDette_Retourne15() {
        var resultat = await _service.ObtenirMontantAPayerAsync("G0001");

        Assert.Equal(15.00m, resultat.MontantParticipation);
        Assert.Null(resultat.MontantDette);
        Assert.Equal(15.00m, resultat.MontantTotal);
    }

    [Fact]
    public async Task ObtenirMontantAPayerAsync_AvecDette_ReporteLeMontant() {
        _detteRepoMock.Setup(r => r.GetNonSoldeeAsync("G0001")).ReturnsAsync(new Dette { Id = 1, MembreMatricule = "G0001", MatchOrigineId = 1, Montant = 45.00m, Soldee = false });

        var resultat = await _service.ObtenirMontantAPayerAsync("G0001");

        Assert.Equal(15.00m, resultat.MontantParticipation);
        Assert.Equal(45.00m, resultat.MontantDette);
        Assert.Equal(60.00m, resultat.MontantTotal);
    }

    [Fact]
    public async Task PayerParticipationAsync_MembreInconnu_RetourneEchec() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("XXXX")).ReturnsAsync((Membre?)null);

        var resultat = await _service.PayerParticipationAsync(1, "XXXX");

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task PayerParticipationAsync_ParticipationIntrouvable_RetourneEchec() {
        _matchRepoMock.Setup(r => r.GetParticipationByIdAsync(99)).ReturnsAsync((Participation?)null);

        var resultat = await _service.PayerParticipationAsync(99, "G0001");

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task PayerParticipationAsync_ParticipationDUnAutreMembre_RetourneEchec() {
        _matchRepoMock.Setup(r => r.GetParticipationByIdAsync(1)).ReturnsAsync(
            new Participation { Id = 1, MatchId = 1, MembreMatricule = "L00001", DateInscription = DateTime.Now });

        var resultat = await _service.PayerParticipationAsync(1, "G0001");

        Assert.False(resultat.Succes);
        _matchRepoMock.Verify(r => r.PayerParticipationAsync(It.IsAny<Participation>(), It.IsAny<Dette>()), Times.Never);
    }

    [Fact]
    public async Task PayerParticipationAsync_DejaPayee_RetourneEchec() {
        _matchRepoMock.Setup(r => r.GetParticipationByIdAsync(1)).ReturnsAsync(
            new Participation {
                Id = 1, MatchId = 1, MembreMatricule = "G0001", DateInscription = DateTime.Now,
                Paiement = new Paiement { MontantParticipation = 15.00m, MontantDetteReportee = 0.00m, DatePaiement = DateTime.Now }
            });

        var resultat = await _service.PayerParticipationAsync(1, "G0001");

        Assert.False(resultat.Succes);
        _matchRepoMock.Verify(r => r.PayerParticipationAsync(It.IsAny<Participation>(), It.IsAny<Dette>()), Times.Never);
    }

    [Fact]
    public async Task PayerParticipationAsync_RequeteValideSansDette_Retourne15() {
        var participation = new Participation { Id = 1, MatchId = 1, MembreMatricule = "G0001", DateInscription = DateTime.Now };
        _matchRepoMock.Setup(r => r.GetParticipationByIdAsync(1)).ReturnsAsync(participation);
        _matchRepoMock.Setup(r => r.PayerParticipationAsync(participation, null)).ReturnsAsync(participation);

        var resultat = await _service.PayerParticipationAsync(1, "G0001");

        Assert.True(resultat.Succes);
        Assert.False(resultat.DetteReglee);
        Assert.Equal(15.00m, resultat.MontantPaye);
    }

    // EF-bk-018 : une dette active est réglée automatiquement, comme pour l'inscription publique.
    [Fact]
    public async Task PayerParticipationAsync_AvecDette_LaRegleEtReporteLeMontant() {
        var participation = new Participation { Id = 1, MatchId = 1, MembreMatricule = "G0001", DateInscription = DateTime.Now };
        var dette = new Dette { Id = 7, MembreMatricule = "G0001", MatchOrigineId = 5, Montant = 45.00m, Soldee = false };
        _matchRepoMock.Setup(r => r.GetParticipationByIdAsync(1)).ReturnsAsync(participation);
        _detteRepoMock.Setup(r => r.GetNonSoldeeAsync("G0001")).ReturnsAsync(dette);
        _matchRepoMock.Setup(r => r.PayerParticipationAsync(participation, dette)).ReturnsAsync(participation);

        var resultat = await _service.PayerParticipationAsync(1, "G0001");

        Assert.True(resultat.Succes);
        Assert.True(resultat.DetteReglee);
        Assert.Equal(60.00m, resultat.MontantPaye);
    }

    [Fact]
    public async Task PayerParticipationAsync_ConflitConcurrent_RetourneEchec() {
        var participation = new Participation { Id = 1, MatchId = 1, MembreMatricule = "G0001", DateInscription = DateTime.Now };
        _matchRepoMock.Setup(r => r.GetParticipationByIdAsync(1)).ReturnsAsync(participation);
        _matchRepoMock.Setup(r => r.PayerParticipationAsync(participation, null)).ThrowsAsync(new ParticipationDejaPayeeException());

        var resultat = await _service.PayerParticipationAsync(1, "G0001");

        Assert.False(resultat.Succes);
    }

    [Fact]
    public async Task ObtenirParticipationsEnAttenteAsync_MembreInconnu_RetourneNull() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("XXXX")).ReturnsAsync((Membre?)null);

        var resultat = await _service.ObtenirParticipationsEnAttenteAsync("XXXX");

        Assert.Null(resultat);
    }

    [Fact]
    public async Task ObtenirParticipationsEnAttenteAsync_MembreConnu_RetourneLesParticipationsTrieesParDate() {
        var site = new Site { Id = 1, Nom = "Site 1" };
        var terrain = new Terrain { Id = 11, SiteId = 1, Numero = 3 };
        var matchProche = new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Today.AddDays(2), Visibilite = "PRIVE", OrganisateurMatricule = "S00001", Statut = "INCOMPLET", Site = site, Terrain = terrain };
        var matchLointain = new Match { Id = 2, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Today.AddDays(5), Visibilite = "PRIVE", OrganisateurMatricule = "S00002", Statut = "INCOMPLET", Site = site, Terrain = terrain };
        var participations = new List<Participation> {
            new() { Id = 20, MatchId = 2, MembreMatricule = "G0001", DateInscription = DateTime.Now, Match = matchLointain },
            new() { Id = 10, MatchId = 1, MembreMatricule = "G0001", DateInscription = DateTime.Now, Match = matchProche }
        };
        _matchRepoMock.Setup(r => r.GetParticipationsEnAttenteAsync("G0001")).ReturnsAsync(participations);

        var resultat = await _service.ObtenirParticipationsEnAttenteAsync("G0001");

        Assert.NotNull(resultat);
        Assert.Equal(2, resultat!.Count);
        Assert.Equal(10, resultat[0].ParticipationId);
        Assert.Equal("Site 1", resultat[0].NomSite);
        Assert.Equal(3, resultat[0].NumeroTerrain);
        Assert.Equal("S00001", resultat[0].OrganisateurMatricule);
        Assert.Equal(20, resultat[1].ParticipationId);
    }

    // --- ObtenirReservationsAsync (EF-bk-013) ---

    [Fact]
    public async Task ObtenirReservationsAsync_MembreInconnu_RetourneNull() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("XXXX")).ReturnsAsync((Membre?)null);

        var resultat = await _service.ObtenirReservationsAsync("XXXX");

        Assert.Null(resultat);
    }

    [Fact]
    public async Task ObtenirReservationsAsync_MembreConnu_RetourneTrieesParDateDecroissanteAvecRoleOrganisateur() {
        var site = new Site { Id = 1, Nom = "Site 1" };
        var terrain = new Terrain { Id = 11, SiteId = 1, Numero = 2 };
        var matchOrganise = new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Today.AddDays(-3), Visibilite = "PRIVE", OrganisateurMatricule = "G0001", Statut = "TERMINE", Site = site, Terrain = terrain };
        var matchParticipe = new Match { Id = 2, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Today.AddDays(3), Visibilite = "PUBLIC", OrganisateurMatricule = "S00001", Statut = "INCOMPLET", Site = site, Terrain = terrain };
        _matchRepoMock.Setup(r => r.GetReservationsAsync("G0001")).ReturnsAsync(new List<Match> { matchOrganise, matchParticipe });

        var resultat = await _service.ObtenirReservationsAsync("G0001");

        Assert.NotNull(resultat);
        Assert.Equal(2, resultat!.Count);
        // Le plus proche/imminent en premier (tri décroissant sur la date).
        Assert.Equal(2, resultat[0].Id);
        Assert.False(resultat[0].EstOrganisateur);
        Assert.Equal(1, resultat[1].Id);
        Assert.True(resultat[1].EstOrganisateur);
    }

    // "Statut TERMINE d'un match" (calcul hybride, CDC) : tant que le job de clôture (issue #10)
    // n'a pas scellé le match, l'affichage doit calculer TERMINE dès que l'heure courante dépasse
    // dateHeure + 1h30, sans jamais réécrire MATCH.statut en base.
    [Fact]
    public async Task ObtenirReservationsAsync_MatchPasseNonScelle_AfficheTermineCalcule() {
        var site = new Site { Id = 1, Nom = "Site 1" };
        var terrain = new Terrain { Id = 11, SiteId = 1, Numero = 3 };
        var matchPasse = new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now.AddDays(-1), Visibilite = "PRIVE", OrganisateurMatricule = "G0001", Statut = "INCOMPLET", Site = site, Terrain = terrain };
        _matchRepoMock.Setup(r => r.GetReservationsAsync("G0001")).ReturnsAsync(new List<Match> { matchPasse });

        var resultat = await _service.ObtenirReservationsAsync("G0001");

        Assert.Equal("TERMINE", resultat![0].Statut);
    }

    [Fact]
    public async Task ObtenirReservationsAsync_MatchDansLeCreneauEnCours_GardeLeStatutBrut() {
        // Commencé il y a 30 minutes : encore dans le créneau de 1h30, pas terminé.
        var site = new Site { Id = 1, Nom = "Site 1" };
        var terrain = new Terrain { Id = 11, SiteId = 1, Numero = 3 };
        var matchEnCours = new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = DateTime.Now.AddMinutes(-30), Visibilite = "PRIVE", OrganisateurMatricule = "G0001", Statut = "COMPLET", Site = site, Terrain = terrain };
        _matchRepoMock.Setup(r => r.GetReservationsAsync("G0001")).ReturnsAsync(new List<Match> { matchEnCours });

        var resultat = await _service.ObtenirReservationsAsync("G0001");

        Assert.Equal("COMPLET", resultat![0].Statut);
    }

    // --- ObtenirDetailAsync (EF-bk-021) ---

    private static Match MatchDetailValide(string visibilite = "PRIVE", string organisateur = "G0001", int siteId = 1, List<Participation>? participations = null) => new() {
        Id = 1,
        SiteId = siteId,
        TerrainId = 11,
        DateHeure = DateTime.Today.AddDays(3),
        Visibilite = visibilite,
        OrganisateurMatricule = organisateur,
        Statut = "INCOMPLET",
        Site = new Site { Id = siteId, Nom = "Site " + siteId },
        Terrain = new Terrain { Id = 11, SiteId = siteId, Numero = 2 },
        Participations = participations ?? new List<Participation>()
    };

    [Fact]
    public async Task ObtenirDetailAsync_MembreInconnu_RetourneNull() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("XXXX")).ReturnsAsync((Membre?)null);

        var resultat = await _service.ObtenirDetailAsync(1, "XXXX");

        Assert.Null(resultat);
    }

    [Fact]
    public async Task ObtenirDetailAsync_MatchInconnu_RetourneNull() {
        _matchRepoMock.Setup(r => r.GetDetailAsync(99)).ReturnsAsync((Match?)null);

        var resultat = await _service.ObtenirDetailAsync(99, "G0001");

        Assert.Null(resultat);
    }

    [Fact]
    public async Task ObtenirDetailAsync_Organisateur_RetourneLeDetail() {
        var match = MatchDetailValide(organisateur: "G0001");
        _matchRepoMock.Setup(r => r.GetDetailAsync(1)).ReturnsAsync(match);

        var resultat = await _service.ObtenirDetailAsync(1, "G0001");

        Assert.NotNull(resultat);
        Assert.Equal("Site 1", resultat!.NomSite);
        Assert.Equal(2, resultat.NumeroTerrain);
    }

    // "Statut TERMINE d'un match" (calcul hybride, CDC) — même règle que sur la liste des
    // réservations.
    [Fact]
    public async Task ObtenirDetailAsync_MatchPasseNonScelle_AfficheTermineCalcule() {
        var match = MatchDetailValide(organisateur: "G0001");
        match.DateHeure = DateTime.Now.AddDays(-1); // toujours INCOMPLET en base (MatchDetailValide)
        _matchRepoMock.Setup(r => r.GetDetailAsync(1)).ReturnsAsync(match);

        var resultat = await _service.ObtenirDetailAsync(1, "G0001");

        Assert.Equal("TERMINE", resultat!.Statut);
    }

    [Fact]
    public async Task ObtenirDetailAsync_MatchDejaScelleTermine_RestTermine() {
        var match = MatchDetailValide(organisateur: "G0001");
        match.Statut = "TERMINE";
        match.DateHeure = DateTime.Now.AddDays(-30);
        _matchRepoMock.Setup(r => r.GetDetailAsync(1)).ReturnsAsync(match);

        var resultat = await _service.ObtenirDetailAsync(1, "G0001");

        Assert.Equal("TERMINE", resultat!.Statut);
    }

    [Fact]
    public async Task ObtenirDetailAsync_Participant_RetourneLeDetailAvecStatutDePaiement() {
        var participations = new List<Participation> {
            new() { MembreMatricule = "G0001", DateInscription = DateTime.Now, Paiement = new Paiement { MontantParticipation = 15.00m, MontantDetteReportee = 0.00m, DatePaiement = DateTime.Now } },
            new() { MembreMatricule = "L00001", DateInscription = DateTime.Now } // en attente
        };
        var match = MatchDetailValide(organisateur: "S00001", participations: participations);
        _matchRepoMock.Setup(r => r.GetDetailAsync(1)).ReturnsAsync(match);

        var resultat = await _service.ObtenirDetailAsync(1, "L00001");

        Assert.NotNull(resultat);
        Assert.True(resultat!.Joueurs.Single(j => j.MembreMatricule == "G0001").Paye);
        Assert.False(resultat.Joueurs.Single(j => j.MembreMatricule == "L00001").Paye);
    }

    [Fact]
    public async Task ObtenirDetailAsync_NonImpliqueMatchPrive_RetourneNull() {
        var match = MatchDetailValide(visibilite: "PRIVE", organisateur: "S00001");
        _matchRepoMock.Setup(r => r.GetDetailAsync(1)).ReturnsAsync(match);

        var resultat = await _service.ObtenirDetailAsync(1, "G0001");

        Assert.Null(resultat);
    }

    [Fact]
    public async Task ObtenirDetailAsync_NonImpliqueMatchPublicGlobal_RetourneLeDetail() {
        var match = MatchDetailValide(visibilite: "PUBLIC", organisateur: "S00001", siteId: 2);
        _matchRepoMock.Setup(r => r.GetDetailAsync(1)).ReturnsAsync(match);

        var resultat = await _service.ObtenirDetailAsync(1, "G0001");

        Assert.NotNull(resultat);
    }

    // R-ACC-002 / EF-bk-012 : même portée que pour rejoindre, mais sans la fenêtre de délai
    // (R-VAL-003 ne borne que la validation d'une inscription, pas la consultation).
    [Fact]
    public async Task ObtenirDetailAsync_NonImpliqueMembreSiteMemeSite_RetourneLeDetail() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("S00003")).ReturnsAsync(MembreValide("S00003", "SITE", 1, 14));
        var match = MatchDetailValide(visibilite: "PUBLIC", organisateur: "G0001", siteId: 1);
        _matchRepoMock.Setup(r => r.GetDetailAsync(1)).ReturnsAsync(match);

        var resultat = await _service.ObtenirDetailAsync(1, "S00003");

        Assert.NotNull(resultat);
    }

    [Fact]
    public async Task ObtenirDetailAsync_NonImpliqueMembreSiteAutreSite_RetourneNull() {
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("S00003")).ReturnsAsync(MembreValide("S00003", "SITE", 2, 14));
        var match = MatchDetailValide(visibilite: "PUBLIC", organisateur: "G0001", siteId: 1);
        _matchRepoMock.Setup(r => r.GetDetailAsync(1)).ReturnsAsync(match);

        var resultat = await _service.ObtenirDetailAsync(1, "S00003");

        Assert.Null(resultat);
    }
}

using Microsoft.EntityFrameworkCore;
using PadelManager.Interfaces;
using PadelManager.Repositories;
using Xunit;

namespace PadelManager.Tests;

public class MatchRepositoryTests {
    private static PadelManagerDbContext CreerContexteEnMemoire() {
        var options = new DbContextOptionsBuilder<PadelManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PadelManagerDbContext(options);
    }

    private static async Task<PadelManagerDbContext> CreerContexteAvecDonneesDeBaseAsync() {
        var context = CreerContexteEnMemoire();
        context.Sites.Add(new Site { Id = 1, Nom = "Site 1" });
        context.Terrains.Add(new Terrain { Id = 11, SiteId = 1, Numero = 1 });
        context.Membres.Add(new Membre { Matricule = "G001", TypeMembre = "GLOBAL" });
        await context.SaveChangesAsync();
        return context;
    }

    [Fact]
    public async Task GetForSiteAndDateAsync_RetourneLesMatchsDeLaDate() {
        // Arrange
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        context.Matches.AddRange(
            new Match { SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" },
            new Match { SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 20, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" },
            new Match { SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 6, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" }); // autre date
        await context.SaveChangesAsync();

        var repository = new MatchRepository(context);

        // Act
        var resultat = await repository.GetForSiteAndDateAsync(1, new DateOnly(2026, 1, 5));

        // Assert
        Assert.Equal(2, resultat.Count);
    }

    [Fact]
    public async Task ExisteAsync_CreneauPris_RetourneTrue() {
        // Arrange
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        var dateHeure = new DateTime(2026, 1, 5, 9, 0, 0);
        context.Matches.Add(new Match { SiteId = 1, TerrainId = 11, DateHeure = dateHeure, Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" });
        await context.SaveChangesAsync();

        var repository = new MatchRepository(context);

        // Act & Assert
        Assert.True(await repository.ExisteAsync(11, dateHeure));
        Assert.False(await repository.ExisteAsync(11, dateHeure.AddMinutes(15)));
    }

    [Fact]
    public async Task AddAsync_InsereLeMatchEtSonGrapheDeParticipations() {
        // Arrange
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        var repository = new MatchRepository(context);
        var match = new Match {
            SiteId = 1,
            TerrainId = 11,
            DateHeure = new DateTime(2026, 1, 5, 9, 0, 0),
            Visibilite = "PRIVE",
            OrganisateurMatricule = "G001",
            Statut = "INCOMPLET"
        };
        match.Participations.Add(new Participation {
            MembreMatricule = "G001",
            DateInscription = DateTime.Now,
            Paiement = new Paiement { MontantParticipation = 15.00m, MontantDetteReportee = 0.00m, DatePaiement = DateTime.Now }
        });

        // Act
        var resultat = await repository.AddAsync(match);

        // Assert
        Assert.NotEqual(0, resultat.Id);
        Assert.Single(context.Matches);
        Assert.Single(context.Participations);
        Assert.Single(context.Paiements);
    }

    // Pas de test InMemory pour la traduction DbUpdateException -> CreneauIndisponibleException :
    // le provider InMemory n'enforce pas UQ_MATCH_terrain_creneau (vérifié empiriquement, aucune
    // exception levée sur un doublon), contrairement à SQL Server. La traduction elle-même est
    // couverte au niveau service (MatchServiceTests, avec le repository mocké) ; le vrai conflit
    // DB est vérifié manuellement via un test HTTP contre SQL Server (cf. plan de vérification).

    [Fact]
    public async Task GetByIdAsync_MatchExistant_RetourneLeMatch() {
        // Arrange
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        context.Matches.Add(new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PUBLIC", OrganisateurMatricule = "G001", Statut = "INCOMPLET" });
        await context.SaveChangesAsync();

        var repository = new MatchRepository(context);

        // Act
        var resultat = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal("PUBLIC", resultat!.Visibilite);
    }

    [Fact]
    public async Task GetByIdAsync_Inexistant_RetourneNull() {
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        var repository = new MatchRepository(context);

        var resultat = await repository.GetByIdAsync(999);

        Assert.Null(resultat);
    }

    [Fact]
    public async Task GetPublicsIncompletsAsync_FiltreVisibiliteStatutEtDate() {
        // Arrange
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        var maintenant = new DateTime(2026, 1, 1, 12, 0, 0);
        context.Matches.AddRange(
            new Match { SiteId = 1, TerrainId = 11, DateHeure = maintenant.AddDays(1), Visibilite = "PUBLIC", OrganisateurMatricule = "G001", Statut = "INCOMPLET" }, // visible
            new Match { SiteId = 1, TerrainId = 11, DateHeure = maintenant.AddDays(2), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" }, // privé
            new Match { SiteId = 1, TerrainId = 11, DateHeure = maintenant.AddDays(3), Visibilite = "PUBLIC", OrganisateurMatricule = "G001", Statut = "COMPLET" }, // complet
            new Match { SiteId = 1, TerrainId = 11, DateHeure = maintenant.AddDays(-1), Visibilite = "PUBLIC", OrganisateurMatricule = "G001", Statut = "INCOMPLET" }); // déjà passé
        await context.SaveChangesAsync();

        var repository = new MatchRepository(context);

        // Act
        var resultat = await repository.GetPublicsIncompletsAsync(maintenant);

        // Assert
        Assert.Single(resultat);
        Assert.Equal(maintenant.AddDays(1), resultat[0].DateHeure);
    }

    [Fact]
    public async Task GetParticipationByIdAsync_ParticipationExistante_RetourneAvecPaiement() {
        // Arrange
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        context.Matches.Add(new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" });
        context.Participations.Add(new Participation {
            Id = 1, MatchId = 1, MembreMatricule = "G001", DateInscription = DateTime.Now,
            Paiement = new Paiement { MontantParticipation = 15.00m, MontantDetteReportee = 0.00m, DatePaiement = DateTime.Now }
        });
        await context.SaveChangesAsync();

        var repository = new MatchRepository(context);

        // Act
        var resultat = await repository.GetParticipationByIdAsync(1);

        // Assert
        Assert.NotNull(resultat);
        Assert.NotNull(resultat!.Paiement);
    }

    [Fact]
    public async Task GetParticipationByIdAsync_EnAttente_RetourneSansPaiement() {
        // Arrange
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        context.Matches.Add(new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" });
        context.Participations.Add(new Participation { Id = 1, MatchId = 1, MembreMatricule = "L001", DateInscription = DateTime.Now });
        await context.SaveChangesAsync();

        var repository = new MatchRepository(context);

        // Act
        var resultat = await repository.GetParticipationByIdAsync(1);

        // Assert
        Assert.NotNull(resultat);
        Assert.Null(resultat!.Paiement);
    }

    [Fact]
    public async Task GetParticipationByIdAsync_Inexistante_RetourneNull() {
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        var repository = new MatchRepository(context);

        var resultat = await repository.GetParticipationByIdAsync(999);

        Assert.Null(resultat);
    }

    // Pas de test InMemory pour PayerParticipationAsync : comme InscrireEtPayerAsync, il commence
    // par un SELECT ... WITH (UPDLOCK, HOLDLOCK) (FromSqlInterpolated), non supporté par le
    // provider InMemory (relationnel uniquement). Couvert par MatchServiceTests (repository
    // mocké) et vérifié en HTTP réel contre SQL Server.

    [Fact]
    public async Task GetParticipationsEnAttenteAsync_NeRetourneQueLesParticipationsNonPayeesDuMembre() {
        // Arrange
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        context.Membres.Add(new Membre { Matricule = "L001", TypeMembre = "LIBRE" });
        context.Matches.Add(new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" });
        context.Participations.AddRange(
            new Participation { Id = 1, MatchId = 1, MembreMatricule = "L001", DateInscription = DateTime.Now }, // en attente, du bon membre
            new Participation { Id = 2, MatchId = 1, MembreMatricule = "L001", DateInscription = DateTime.Now, Paiement = new Paiement { MontantParticipation = 15.00m, MontantDetteReportee = 0.00m, DatePaiement = DateTime.Now } }, // déjà payée
            new Participation { Id = 3, MatchId = 1, MembreMatricule = "G001", DateInscription = DateTime.Now }); // en attente, d'un autre membre
        await context.SaveChangesAsync();

        var repository = new MatchRepository(context);

        // Act
        var resultat = await repository.GetParticipationsEnAttenteAsync("L001");

        // Assert
        Assert.Single(resultat);
        Assert.Equal(1, resultat[0].Id);
        Assert.Equal("Site 1", resultat[0].Match.Site.Nom);
    }

    [Fact]
    public async Task GetReservationsAsync_RetourneLesMatchsOuLeMembreEstOrganisateurOuParticipant() {
        // Arrange
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        context.Membres.Add(new Membre { Matricule = "S001", TypeMembre = "SITE", SiteId = 1 });
        context.Matches.AddRange(
            new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" }, // organisateur
            new Match { Id = 2, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 6, 9, 0, 0), Visibilite = "PUBLIC", OrganisateurMatricule = "S001", Statut = "INCOMPLET" }, // participant
            new Match { Id = 3, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 7, 9, 0, 0), Visibilite = "PUBLIC", OrganisateurMatricule = "S001", Statut = "INCOMPLET" }); // étranger
        context.Participations.Add(new Participation { MatchId = 2, MembreMatricule = "G001", DateInscription = DateTime.Now });
        await context.SaveChangesAsync();

        var repository = new MatchRepository(context);

        // Act
        var resultat = await repository.GetReservationsAsync("G001");

        // Assert
        Assert.Equal(2, resultat.Count);
        Assert.Contains(resultat, m => m.Id == 1);
        Assert.Contains(resultat, m => m.Id == 2);
        Assert.DoesNotContain(resultat, m => m.Id == 3);
    }

    [Fact]
    public async Task GetDetailAsync_MatchExistant_RetourneAvecParticipationsEtPaiements() {
        // Arrange
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        context.Membres.Add(new Membre { Matricule = "L001", TypeMembre = "LIBRE" });
        context.Matches.Add(new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" });
        context.Participations.AddRange(
            new Participation { MatchId = 1, MembreMatricule = "G001", DateInscription = DateTime.Now, Paiement = new Paiement { MontantParticipation = 15.00m, MontantDetteReportee = 0.00m, DatePaiement = DateTime.Now } },
            new Participation { MatchId = 1, MembreMatricule = "L001", DateInscription = DateTime.Now });
        await context.SaveChangesAsync();

        var repository = new MatchRepository(context);

        // Act
        var resultat = await repository.GetDetailAsync(1);

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal("Site 1", resultat!.Site.Nom);
        Assert.Equal(2, resultat.Participations.Count);
        Assert.NotNull(resultat.Participations.Single(p => p.MembreMatricule == "G001").Paiement);
        Assert.Null(resultat.Participations.Single(p => p.MembreMatricule == "L001").Paiement);
    }

    [Fact]
    public async Task GetDetailAsync_Inexistant_RetourneNull() {
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        var repository = new MatchRepository(context);

        var resultat = await repository.GetDetailAsync(999);

        Assert.Null(resultat);
    }
}

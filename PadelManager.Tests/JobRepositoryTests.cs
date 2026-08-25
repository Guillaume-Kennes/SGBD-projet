using Microsoft.EntityFrameworkCore;
using PadelManager.Repositories;
using Xunit;

namespace PadelManager.Tests;

public class JobRepositoryTests {
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
    public async Task GetMatchsPrivesDeLaDateAsync_NeRetourneQueLesMatchsPrivesDeCetteDate() {
        // Arrange
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        context.Matches.AddRange(
            new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" }, // visible
            new Match { Id = 2, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 20, 0, 0), Visibilite = "PUBLIC", OrganisateurMatricule = "G001", Statut = "INCOMPLET" }, // public
            new Match { Id = 3, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 6, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" }); // autre date
        context.Participations.Add(new Participation {
            MatchId = 1, MembreMatricule = "G001", DateInscription = DateTime.Now,
            Paiement = new Paiement { MontantParticipation = 15.00m, MontantDetteReportee = 0.00m, DatePaiement = DateTime.Now }
        });
        await context.SaveChangesAsync();

        var repository = new JobRepository(context);

        // Act
        var resultat = await repository.GetMatchsPrivesDeLaDateAsync(new DateOnly(2026, 1, 5));

        // Assert
        Assert.Single(resultat);
        Assert.Equal(1, resultat[0].Id);
        Assert.Single(resultat[0].Participations);
        Assert.NotNull(resultat[0].Participations.First().Paiement);
    }

    [Fact]
    public async Task GetMatchsDeLaDateAsync_RetourneTousLesMatchsDeCetteDateQuelleQueSoitLaVisibilite() {
        // Arrange
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        context.Matches.AddRange(
            new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" },
            new Match { Id = 2, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 20, 0, 0), Visibilite = "PUBLIC", OrganisateurMatricule = "G001", Statut = "TERMINE" },
            new Match { Id = 3, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 6, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" }); // autre date
        await context.SaveChangesAsync();

        var repository = new JobRepository(context);

        // Act
        var resultat = await repository.GetMatchsDeLaDateAsync(new DateOnly(2026, 1, 5));

        // Assert
        Assert.Equal(2, resultat.Count);
        Assert.Contains(resultat, m => m.Id == 1);
        Assert.Contains(resultat, m => m.Id == 2);
    }

    [Fact]
    public async Task BasculerAsync_SupprimeLesParticipationsNonPayeesEtPasseEnPublicAvecPenalite() {
        // Arrange
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        context.Membres.Add(new Membre { Matricule = "L001", TypeMembre = "LIBRE" });
        var match = new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" };
        context.Matches.Add(match);
        var participationPayee = new Participation {
            Id = 1, MatchId = 1, MembreMatricule = "G001", DateInscription = DateTime.Now,
            Paiement = new Paiement { MontantParticipation = 15.00m, MontantDetteReportee = 0.00m, DatePaiement = DateTime.Now }
        };
        var participationNonPayee = new Participation { Id = 2, MatchId = 1, MembreMatricule = "L001", DateInscription = DateTime.Now };
        context.Participations.AddRange(participationPayee, participationNonPayee);
        await context.SaveChangesAsync();

        var repository = new JobRepository(context);
        var penalite = new Penalite { MembreMatricule = "G001", MatchOrigineId = 1, DateApplication = DateTime.Now, DelaiJusquAu = DateOnly.FromDateTime(DateTime.Today).AddDays(7) };

        // Act
        await repository.BasculerAsync(match, new List<Participation> { participationNonPayee }, penalite);

        // Assert
        Assert.Equal("PUBLIC", match.Visibilite);
        Assert.Single(context.Participations); // seule la payée reste
        Assert.Equal("G001", context.Participations.Single().MembreMatricule);
        Assert.Single(context.Penalites);
        Assert.Equal("G001", context.Penalites.Single().MembreMatricule);
    }

    [Fact]
    public async Task CreerDetteAsync_AjouteLaDette() {
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        context.Matches.Add(new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "INCOMPLET" });
        await context.SaveChangesAsync();
        var repository = new JobRepository(context);
        var dette = new Dette { MembreMatricule = "G001", MatchOrigineId = 1, Montant = 45.00m, Soldee = false, DateCreation = DateTime.Now };

        await repository.CreerDetteAsync(dette);

        Assert.Single(context.Dettes);
        Assert.Equal(45.00m, context.Dettes.Single().Montant);
    }

    [Fact]
    public async Task ScellerTermineAsync_MetLeStatutATermine() {
        await using var context = await CreerContexteAvecDonneesDeBaseAsync();
        var match = new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "COMPLET" };
        context.Matches.Add(match);
        await context.SaveChangesAsync();
        var repository = new JobRepository(context);

        await repository.ScellerTermineAsync(match);

        Assert.Equal("TERMINE", match.Statut);
    }
}

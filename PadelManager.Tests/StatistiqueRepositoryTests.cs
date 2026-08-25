using Microsoft.EntityFrameworkCore;
using PadelManager.Repositories;
using Xunit;

namespace PadelManager.Tests;

public class StatistiqueRepositoryTests {
    private static PadelManagerDbContext CreerContexteEnMemoire() {
        var options = new DbContextOptionsBuilder<PadelManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PadelManagerDbContext(options);
    }

    private static async Task<PadelManagerDbContext> CreerContexteAvecDeuxSitesAsync() {
        var context = CreerContexteEnMemoire();
        context.Sites.AddRange(new Site { Id = 1, Nom = "Site 1" }, new Site { Id = 2, Nom = "Site 2" });
        context.Terrains.AddRange(new Terrain { Id = 11, SiteId = 1, Numero = 1 }, new Terrain { Id = 21, SiteId = 2, Numero = 1 });
        context.Membres.Add(new Membre { Matricule = "G001", TypeMembre = "GLOBAL" });
        context.Matches.AddRange(
            new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "TERMINE" },
            new Match { Id = 2, SiteId = 2, TerrainId = 21, DateHeure = new DateTime(2026, 1, 6, 9, 0, 0), Visibilite = "PUBLIC", OrganisateurMatricule = "G001", Statut = "TERMINE" });
        context.Participations.AddRange(
            new Participation { Id = 1, MatchId = 1, MembreMatricule = "G001", DateInscription = DateTime.Now },
            new Participation { Id = 2, MatchId = 2, MembreMatricule = "G001", DateInscription = DateTime.Now });
        context.Paiements.AddRange(
            new Paiement { ParticipationId = 1, MontantParticipation = 15.00m, MontantDetteReportee = 0.00m, DatePaiement = DateTime.Now },
            new Paiement { ParticipationId = 2, MontantParticipation = 15.00m, MontantDetteReportee = 30.00m, DatePaiement = DateTime.Now });
        await context.SaveChangesAsync();
        return context;
    }

    [Fact]
    public async Task GetPaiementsAsync_SansFiltre_RetourneTousLesPaiements() {
        // Arrange
        await using var context = await CreerContexteAvecDeuxSitesAsync();
        var repository = new StatistiqueRepository(context);

        // Act
        var resultat = await repository.GetPaiementsAsync(null);

        // Assert
        Assert.Equal(2, resultat.Count);
    }

    [Fact]
    public async Task GetPaiementsAsync_AvecFiltreSite_NeRetourneQueLesPaiementsDeCeSite() {
        // Arrange : rattachement via PARTICIPATION.matchId -> MATCH.siteId (site 1 uniquement).
        await using var context = await CreerContexteAvecDeuxSitesAsync();
        var repository = new StatistiqueRepository(context);

        // Act
        var resultat = await repository.GetPaiementsAsync(1);

        // Assert
        Assert.Single(resultat);
        Assert.Equal(1, resultat[0].Participation.MatchId);
    }

    [Fact]
    public async Task GetParticipationsAsync_SansFiltre_RetourneToutesLesParticipations() {
        await using var context = await CreerContexteAvecDeuxSitesAsync();
        var repository = new StatistiqueRepository(context);

        var resultat = await repository.GetParticipationsAsync(null);

        Assert.Equal(2, resultat.Count);
    }

    [Fact]
    public async Task GetParticipationsAsync_AvecFiltreSite_NeRetourneQueCeSite_MemeNonPayees() {
        // Arrange : une participation NON payée (pas de PAIEMENT) doit quand même être retournée
        // ("peu importe payée ou non", EF-bk-016) — contrairement à GetPaiementsAsync.
        await using var context = await CreerContexteAvecDeuxSitesAsync();
        context.Membres.Add(new Membre { Matricule = "L001", TypeMembre = "LIBRE" });
        context.Participations.Add(new Participation { Id = 3, MatchId = 1, MembreMatricule = "L001", DateInscription = DateTime.Now });
        await context.SaveChangesAsync();

        var repository = new StatistiqueRepository(context);

        var resultat = await repository.GetParticipationsAsync(1);

        Assert.Equal(2, resultat.Count);
        Assert.Contains(resultat, p => p.MembreMatricule == "L001");
    }
}

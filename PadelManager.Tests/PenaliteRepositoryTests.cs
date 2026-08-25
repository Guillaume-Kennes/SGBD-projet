using Microsoft.EntityFrameworkCore;
using PadelManager.Repositories;
using Xunit;

namespace PadelManager.Tests;

public class PenaliteRepositoryTests {
    private static PadelManagerDbContext CreerContexteEnMemoire() {
        var options = new DbContextOptionsBuilder<PadelManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PadelManagerDbContext(options);
    }

    private static async Task<PadelManagerDbContext> CreerContexteAvecMatchAsync() {
        var context = CreerContexteEnMemoire();
        context.Sites.Add(new Site { Id = 1, Nom = "Site 1" });
        context.Terrains.Add(new Terrain { Id = 11, SiteId = 1, Numero = 1 });
        context.Membres.Add(new Membre { Matricule = "G001", TypeMembre = "GLOBAL" });
        context.Matches.Add(new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G001", Statut = "TERMINE" });
        await context.SaveChangesAsync();
        return context;
    }

    [Fact]
    public async Task GetPlusRecenteAsync_PlusieursPenalites_RetourneLaPlusRecente() {
        // Arrange
        await using var context = await CreerContexteAvecMatchAsync();
        context.Penalites.AddRange(
            new Penalite { MembreMatricule = "G001", MatchOrigineId = 1, DateApplication = new DateTime(2026, 1, 1), DelaiJusquAu = new DateOnly(2026, 1, 8) },
            new Penalite { MembreMatricule = "G001", MatchOrigineId = 1, DateApplication = new DateTime(2026, 2, 1), DelaiJusquAu = new DateOnly(2026, 2, 8) });
        await context.SaveChangesAsync();

        var repository = new PenaliteRepository(context);

        // Act
        var resultat = await repository.GetPlusRecenteAsync("G001");

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal(new DateOnly(2026, 2, 8), resultat!.DelaiJusquAu);
    }

    [Fact]
    public async Task GetPlusRecenteAsync_Aucune_RetourneNull() {
        await using var context = await CreerContexteAvecMatchAsync();
        var repository = new PenaliteRepository(context);

        var resultat = await repository.GetPlusRecenteAsync("G001");

        Assert.Null(resultat);
    }
}

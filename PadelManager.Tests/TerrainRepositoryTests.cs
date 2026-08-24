using Microsoft.EntityFrameworkCore;
using PadelManager.Repositories;
using Xunit;

namespace PadelManager.Tests;

public class TerrainRepositoryTests {
    private static PadelManagerDbContext CreerContexteEnMemoire() {
        var options = new DbContextOptionsBuilder<PadelManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PadelManagerDbContext(options);
    }

    [Fact]
    public async Task GetByIdAsync_TerrainExistant_RetourneLeTerrain() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.Add(new Site { Id = 1, Nom = "Site 1" });
        context.Terrains.Add(new Terrain { Id = 11, SiteId = 1, Numero = 1 });
        await context.SaveChangesAsync();

        var repository = new TerrainRepository(context);

        // Act
        var resultat = await repository.GetByIdAsync(11);

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal(1, resultat!.SiteId);
    }

    [Fact]
    public async Task GetByIdAsync_Inexistant_RetourneNull() {
        await using var context = CreerContexteEnMemoire();
        var repository = new TerrainRepository(context);

        var resultat = await repository.GetByIdAsync(999);

        Assert.Null(resultat);
    }

    [Fact]
    public async Task GetBySiteIdAsync_RetourneLesTerrainsDuSiteTriesParNumero() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.AddRange(new Site { Id = 1, Nom = "Site 1" }, new Site { Id = 2, Nom = "Site 2" });
        context.Terrains.AddRange(
            new Terrain { Id = 12, SiteId = 1, Numero = 2 },
            new Terrain { Id = 11, SiteId = 1, Numero = 1 },
            new Terrain { Id = 21, SiteId = 2, Numero = 1 });
        await context.SaveChangesAsync();

        var repository = new TerrainRepository(context);

        // Act
        var resultat = await repository.GetBySiteIdAsync(1);

        // Assert
        Assert.Equal(2, resultat.Count);
        Assert.Equal(11, resultat[0].Id);
        Assert.Equal(12, resultat[1].Id);
    }
}

using Microsoft.EntityFrameworkCore;
using PadelManager.Repositories;
using Xunit;

namespace PadelManager.Tests;

public class SiteRepositoryTests {
    private static PadelManagerDbContext CreerContexteEnMemoire() {
        var options = new DbContextOptionsBuilder<PadelManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PadelManagerDbContext(options);
    }

    [Fact]
    public async Task GetByIdAsync_SiteExistant_RetourneLeSite() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.Add(new Site { Id = 1, Nom = "Site 1" });
        await context.SaveChangesAsync();

        var repository = new SiteRepository(context);

        // Act
        var resultat = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal("Site 1", resultat!.Nom);
    }

    [Fact]
    public async Task GetByIdAsync_SiteInexistant_RetourneNull() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        var repository = new SiteRepository(context);

        // Act
        var resultat = await repository.GetByIdAsync(99);

        // Assert
        Assert.Null(resultat);
    }

    [Fact]
    public async Task GetAllAsync_RetourneTousLesSitesTriesParNom() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.AddRange(
            new Site { Id = 2, Nom = "Site B" },
            new Site { Id = 1, Nom = "Site A" });
        await context.SaveChangesAsync();

        var repository = new SiteRepository(context);

        // Act
        var resultat = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, resultat.Count);
        Assert.Equal("Site A", resultat[0].Nom);
        Assert.Equal("Site B", resultat[1].Nom);
    }
}

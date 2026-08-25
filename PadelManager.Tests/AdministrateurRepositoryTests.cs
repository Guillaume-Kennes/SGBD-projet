using Microsoft.EntityFrameworkCore;
using PadelManager.Models;
using PadelManager.Repositories;
using Xunit;

namespace PadelManager.Tests;

public class AdministrateurRepositoryTests {
    private static PadelManagerDbContext CreerContexteEnMemoire() {
        var options = new DbContextOptionsBuilder<PadelManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // base isolée par test
            .Options;
        return new PadelManagerDbContext(options);
    }

    [Fact]
    public async Task GetByMatriculeAsync_AdministrateurGlobalExistant_RetourneLAdministrateur() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Administrateurs.Add(new Administrateur { Matricule = "AG01", Type = "GLOBAL", SiteId = null });
        await context.SaveChangesAsync();

        var repository = new AdministrateurRepository(context);

        // Act
        var resultat = await repository.GetByMatriculeAsync("AG01");

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal("AG01", resultat!.Matricule);
        Assert.Equal("GLOBAL", resultat.Type);
        Assert.Null(resultat.SiteId);
    }

    [Fact]
    public async Task GetByMatriculeAsync_AdministrateurDeSiteExistant_RetourneLAdministrateurAvecSonSite() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.Add(new Site { Id = 1, Nom = "Site 1" });
        context.Administrateurs.Add(new Administrateur { Matricule = "AS01", Type = "SITE", SiteId = 1 });
        await context.SaveChangesAsync();

        var repository = new AdministrateurRepository(context);

        // Act
        var resultat = await repository.GetByMatriculeAsync("AS01");

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal("SITE", resultat!.Type);
        Assert.NotNull(resultat.Site);
        Assert.Equal("Site 1", resultat.Site!.Nom);
    }

    [Fact]
    public async Task GetByMatriculeAsync_MatriculeInexistant_RetourneNull() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        var repository = new AdministrateurRepository(context);

        // Act
        var resultat = await repository.GetByMatriculeAsync("X9999");

        // Assert
        Assert.Null(resultat);
    }
}

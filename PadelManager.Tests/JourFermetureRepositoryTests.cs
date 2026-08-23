using Microsoft.EntityFrameworkCore;
using PadelManager.Repositories;
using Xunit;

namespace PadelManager.Tests;

public class JourFermetureRepositoryTests {
    private static PadelManagerDbContext CreerContexteEnMemoire() {
        var options = new DbContextOptionsBuilder<PadelManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PadelManagerDbContext(options);
    }

    [Fact]
    public async Task GetForSiteAndAnneeAsync_RetourneLesFermeturesDuSiteEtGlobales() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.AddRange(new Site { Id = 1, Nom = "Site 1" }, new Site { Id = 2, Nom = "Site 2" });
        context.JourFermetures.AddRange(
            new JourFermeture { SiteId = 1, Date = new DateOnly(2026, 12, 24) },      // propre au site 1
            new JourFermeture { SiteId = null, Date = new DateOnly(2026, 12, 25) },   // ponctuelle globale
            new JourFermeture { SiteId = 2, Date = new DateOnly(2026, 12, 26) },      // propre à un autre site
            new JourFermeture { SiteId = 1, Date = new DateOnly(2025, 12, 24) });     // hors année demandée
        await context.SaveChangesAsync();

        var repository = new JourFermetureRepository(context);

        // Act
        var resultat = await repository.GetForSiteAndAnneeAsync(1, 2026);

        // Assert
        Assert.Equal(2, resultat.Count);
        Assert.Contains(resultat, j => j.Date == new DateOnly(2026, 12, 24));
        Assert.Contains(resultat, j => j.Date == new DateOnly(2026, 12, 25));
    }

    [Fact]
    public async Task GetForSiteAndAnneeAsync_Aucune_RetourneListeVide() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        var repository = new JourFermetureRepository(context);

        // Act
        var resultat = await repository.GetForSiteAndAnneeAsync(1, 2026);

        // Assert
        Assert.Empty(resultat);
    }

    [Fact]
    public async Task GetByIdAsync_FermetureExistante_RetourneLaFermeture() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.Add(new Site { Id = 1, Nom = "Site 1" });
        var jour = new JourFermeture { SiteId = 1, Date = new DateOnly(2026, 12, 24) };
        context.JourFermetures.Add(jour);
        await context.SaveChangesAsync();

        var repository = new JourFermetureRepository(context);

        // Act
        var resultat = await repository.GetByIdAsync(jour.Id);

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal(new DateOnly(2026, 12, 24), resultat!.Date);
    }

    [Fact]
    public async Task GetByIdAsync_Inexistante_RetourneNull() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        var repository = new JourFermetureRepository(context);

        // Act
        var resultat = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(resultat);
    }

    [Fact]
    public async Task ExisteAsync_DejaDeclaree_RetourneTrue() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.Add(new Site { Id = 1, Nom = "Site 1" });
        context.JourFermetures.Add(new JourFermeture { SiteId = 1, Date = new DateOnly(2026, 12, 24) });
        await context.SaveChangesAsync();

        var repository = new JourFermetureRepository(context);

        // Act & Assert
        Assert.True(await repository.ExisteAsync(1, new DateOnly(2026, 12, 24)));
        Assert.False(await repository.ExisteAsync(1, new DateOnly(2026, 12, 25)));
        Assert.False(await repository.ExisteAsync(null, new DateOnly(2026, 12, 24)));
    }

    [Fact]
    public async Task AddAsync_Ajoute() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.Add(new Site { Id = 1, Nom = "Site 1" });
        await context.SaveChangesAsync();

        var repository = new JourFermetureRepository(context);

        // Act
        var resultat = await repository.AddAsync(new JourFermeture { SiteId = 1, Date = new DateOnly(2026, 12, 24) });

        // Assert
        Assert.NotEqual(0, resultat.Id);
        Assert.Single(context.JourFermetures);
    }

    [Fact]
    public async Task DeleteAsync_FermetureExistante_Supprime() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.Add(new Site { Id = 1, Nom = "Site 1" });
        var jour = new JourFermeture { SiteId = 1, Date = new DateOnly(2026, 12, 24) };
        context.JourFermetures.Add(jour);
        await context.SaveChangesAsync();

        var repository = new JourFermetureRepository(context);

        // Act
        await repository.DeleteAsync(jour.Id);

        // Assert
        Assert.Empty(context.JourFermetures);
    }

    [Fact]
    public async Task DeleteAsync_Inexistante_NeFaitRien() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        var repository = new JourFermetureRepository(context);

        // Act & Assert (ne lève pas)
        await repository.DeleteAsync(999);
    }
}

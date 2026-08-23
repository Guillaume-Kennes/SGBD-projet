using Microsoft.EntityFrameworkCore;
using PadelManager.Repositories;
using Xunit;

namespace PadelManager.Tests;

public class FermetureHebdoGlobaleRepositoryTests {
    private static PadelManagerDbContext CreerContexteEnMemoire() {
        var options = new DbContextOptionsBuilder<PadelManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PadelManagerDbContext(options);
    }

    [Fact]
    public async Task GetByAnneeAsync_AnneeExistante_RetourneLaFermeture() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.FermetureHebdoGlobales.Add(new FermetureHebdoGlobale { Annee = 2026, JoursFermes = "JEU" });
        await context.SaveChangesAsync();

        var repository = new FermetureHebdoGlobaleRepository(context);

        // Act
        var resultat = await repository.GetByAnneeAsync(2026);

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal("JEU", resultat!.JoursFermes);
    }

    [Fact]
    public async Task GetByAnneeAsync_AnneeInexistante_RetourneNull() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        var repository = new FermetureHebdoGlobaleRepository(context);

        // Act
        var resultat = await repository.GetByAnneeAsync(2026);

        // Assert
        Assert.Null(resultat);
    }

    [Fact]
    public async Task UpsertAsync_AucuneExistante_Cree() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        var repository = new FermetureHebdoGlobaleRepository(context);

        // Act
        await repository.UpsertAsync(new FermetureHebdoGlobale { Annee = 2026, JoursFermes = "LUN" });

        // Assert
        Assert.Single(context.FermetureHebdoGlobales);
    }

    [Fact]
    public async Task UpsertAsync_ExistanteDejaPourLAnnee_MetAJourSansDupliquer() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.FermetureHebdoGlobales.Add(new FermetureHebdoGlobale { Annee = 2026, JoursFermes = "LUN" });
        await context.SaveChangesAsync();

        var repository = new FermetureHebdoGlobaleRepository(context);

        // Act
        await repository.UpsertAsync(new FermetureHebdoGlobale { Annee = 2026, JoursFermes = "LUN,MAR" });

        // Assert
        Assert.Single(context.FermetureHebdoGlobales);
        var resultat = await repository.GetByAnneeAsync(2026);
        Assert.Equal("LUN,MAR", resultat!.JoursFermes);
    }

    [Fact]
    public async Task DeleteAsync_ExistanteAnnee_Supprime() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.FermetureHebdoGlobales.Add(new FermetureHebdoGlobale { Annee = 2026, JoursFermes = "LUN" });
        await context.SaveChangesAsync();

        var repository = new FermetureHebdoGlobaleRepository(context);

        // Act
        await repository.DeleteAsync(2026);

        // Assert
        Assert.Empty(context.FermetureHebdoGlobales);
    }

    [Fact]
    public async Task DeleteAsync_AnneeInexistante_NeFaitRien() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        var repository = new FermetureHebdoGlobaleRepository(context);

        // Act & Assert (ne lève pas)
        await repository.DeleteAsync(2026);
    }
}

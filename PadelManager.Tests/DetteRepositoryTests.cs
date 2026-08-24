using Microsoft.EntityFrameworkCore;
using PadelManager.Repositories;
using Xunit;

namespace PadelManager.Tests;

public class DetteRepositoryTests {
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
        context.Membres.Add(new Membre { Matricule = "G0001", TypeMembre = "GLOBAL" });
        context.Matches.Add(new Match { Id = 1, SiteId = 1, TerrainId = 11, DateHeure = new DateTime(2026, 1, 5, 9, 0, 0), Visibilite = "PRIVE", OrganisateurMatricule = "G0001", Statut = "TERMINE" });
        await context.SaveChangesAsync();
        return context;
    }

    [Fact]
    public async Task ExisteDetteNonSoldeeAsync_DetteActive_RetourneTrue() {
        // Arrange
        await using var context = await CreerContexteAvecMatchAsync();
        context.Dettes.Add(new Dette { MembreMatricule = "G0001", MatchOrigineId = 1, Montant = 15.00m, Soldee = false, DateCreation = DateTime.Now });
        await context.SaveChangesAsync();

        var repository = new DetteRepository(context);

        // Act & Assert
        Assert.True(await repository.ExisteDetteNonSoldeeAsync("G0001"));
    }

    [Fact]
    public async Task ExisteDetteNonSoldeeAsync_SeulementDesDettesSoldees_RetourneFalse() {
        // Arrange
        await using var context = await CreerContexteAvecMatchAsync();
        context.Dettes.Add(new Dette {
            MembreMatricule = "G0001", MatchOrigineId = 1, MatchReglementId = 1, Montant = 15.00m,
            Soldee = true, DateCreation = DateTime.Now, DateReglement = DateTime.Now
        });
        await context.SaveChangesAsync();

        var repository = new DetteRepository(context);

        // Act & Assert
        Assert.False(await repository.ExisteDetteNonSoldeeAsync("G0001"));
    }

    [Fact]
    public async Task ExisteDetteNonSoldeeAsync_AucuneDette_RetourneFalse() {
        await using var context = await CreerContexteAvecMatchAsync();
        var repository = new DetteRepository(context);

        Assert.False(await repository.ExisteDetteNonSoldeeAsync("G0001"));
    }

    [Fact]
    public async Task GetNonSoldeeAsync_DetteActive_RetourneLaDette() {
        // Arrange
        await using var context = await CreerContexteAvecMatchAsync();
        context.Dettes.Add(new Dette { MembreMatricule = "G0001", MatchOrigineId = 1, Montant = 30.00m, Soldee = false, DateCreation = DateTime.Now });
        await context.SaveChangesAsync();

        var repository = new DetteRepository(context);

        // Act
        var resultat = await repository.GetNonSoldeeAsync("G0001");

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal(30.00m, resultat!.Montant);
    }

    [Fact]
    public async Task GetNonSoldeeAsync_AucuneDetteActive_RetourneNull() {
        await using var context = await CreerContexteAvecMatchAsync();
        var repository = new DetteRepository(context);

        var resultat = await repository.GetNonSoldeeAsync("G0001");

        Assert.Null(resultat);
    }
}

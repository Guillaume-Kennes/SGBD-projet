using Microsoft.EntityFrameworkCore;
using PadelManager.Models;
using PadelManager.Repositories;
using Xunit;

namespace PadelManager.Tests;

public class MembreRepositoryTests {
    private static PadelManagerDbContext CreerContexteEnMemoire() {
        var options = new DbContextOptionsBuilder<PadelManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // base isolée par test
            .Options;
        return new PadelManagerDbContext(options);
    }

    [Fact]
    public async Task GetByMatriculeAsync_MatriculeExistant_RetourneLeMembre() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.TypeMembres.Add(new TypeMembre { Code = "GLOBAL", Libelle = "Membre global", AnticipationMaxJours = 21, PrefixeMatricule = "G" });
        context.Membres.Add(new Membre { Matricule = "G001", TypeMembre = "GLOBAL", SiteId = null });
        await context.SaveChangesAsync();

        var repository = new MembreRepository(context);

        // Act
        var resultat = await repository.GetByMatriculeAsync("G001");

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal("G001", resultat!.Matricule);
        Assert.NotNull(resultat.TypeMembreNavigation);
        Assert.Equal("GLOBAL", resultat.TypeMembreNavigation.Code);
    }

    [Fact]
    public async Task GetByMatriculeAsync_MatriculeInexistant_RetourneNull() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        var repository = new MembreRepository(context);

        // Act
        var resultat = await repository.GetByMatriculeAsync("X9999");

        // Assert
        Assert.Null(resultat);
    }

    private static async Task<PadelManagerDbContext> CreerContexteAvecMembresVariesAsync() {
        var context = CreerContexteEnMemoire();
        context.Membres.AddRange(
            new Membre { Matricule = "G001", TypeMembre = "GLOBAL", SiteId = null },
            new Membre { Matricule = "L001", TypeMembre = "LIBRE", SiteId = null },
            new Membre { Matricule = "S001", TypeMembre = "SITE", SiteId = 1 },
            new Membre { Matricule = "S002", TypeMembre = "SITE", SiteId = 2 });
        await context.SaveChangesAsync();
        return context;
    }

    [Fact]
    public async Task GetTousAsync_SansSiteId_RetourneTousLesMembresTousTypes() {
        // Arrange
        await using var context = await CreerContexteAvecMembresVariesAsync();
        var repository = new MembreRepository(context);

        // Act
        var resultat = await repository.GetTousAsync(null);

        // Assert
        Assert.Equal(4, resultat.Count);
    }

    [Fact]
    public async Task GetTousAsync_AvecSiteId_NeRetourneQueLesMembresSiteDeCeSite() {
        // Arrange : G001 (Global) et L001 (Libre) ne sont rattachés à aucun site, ils
        // n'apparaissent donc jamais dans une vue Site (EF-bk-017), même si on filtrait par
        // coïncidence sur siteId=1 : seul S001 doit apparaître, pas G001/L001/S002.
        await using var context = await CreerContexteAvecMembresVariesAsync();
        var repository = new MembreRepository(context);

        // Act
        var resultat = await repository.GetTousAsync(1);

        // Assert
        Assert.Single(resultat);
        Assert.Equal("S001", resultat[0].Matricule);
    }
}


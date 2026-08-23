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
        context.Membres.Add(new Membre { Matricule = "G0001", TypeMembre = "GLOBAL", SiteId = null });
        await context.SaveChangesAsync();

        var repository = new MembreRepository(context);

        // Act
        var resultat = await repository.GetByMatriculeAsync("G0001");

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal("G0001", resultat!.Matricule);
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
}


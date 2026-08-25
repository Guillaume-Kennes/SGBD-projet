using Microsoft.EntityFrameworkCore;
using PadelManager.Repositories;
using Xunit;

namespace PadelManager.Tests;

public class DisponibiliteRepositoryTests {
    private static PadelManagerDbContext CreerContexteEnMemoire() {
        var options = new DbContextOptionsBuilder<PadelManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PadelManagerDbContext(options);
    }

    [Fact]
    public async Task GetBySiteAndPeriodeAsync_RetourneLesCreneauxDansLaPeriodeTries() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.Add(new Site { Id = 1, Nom = "Site 1" });
        context.Disponibilites.AddRange(
            new Disponibilite { SiteId = 1, Date = new DateOnly(2026, 1, 5), HeureDebut = new TimeOnly(10, 45), HeureFin = new TimeOnly(12, 15) },
            new Disponibilite { SiteId = 1, Date = new DateOnly(2026, 1, 5), HeureDebut = new TimeOnly(9, 0), HeureFin = new TimeOnly(10, 30) },
            new Disponibilite { SiteId = 1, Date = new DateOnly(2026, 2, 1), HeureDebut = new TimeOnly(9, 0), HeureFin = new TimeOnly(10, 30) }, // hors période
            new Disponibilite { SiteId = 2, Date = new DateOnly(2026, 1, 5), HeureDebut = new TimeOnly(9, 0), HeureFin = new TimeOnly(10, 30) }); // autre site
        await context.SaveChangesAsync();

        var repository = new DisponibiliteRepository(context);

        // Act
        var resultat = await repository.GetBySiteAndPeriodeAsync(1, new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 31));

        // Assert
        Assert.Equal(2, resultat.Count);
        Assert.Equal(new TimeOnly(9, 0), resultat[0].HeureDebut); // trié par heure
    }

    [Fact]
    public async Task ExisteAsync_CreneauExistant_RetourneTrue() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.Add(new Site { Id = 1, Nom = "Site 1" });
        context.Disponibilites.Add(new Disponibilite { SiteId = 1, Date = new DateOnly(2026, 1, 5), HeureDebut = new TimeOnly(9, 0), HeureFin = new TimeOnly(10, 30) });
        await context.SaveChangesAsync();

        var repository = new DisponibiliteRepository(context);

        // Act & Assert
        Assert.True(await repository.ExisteAsync(1, new DateOnly(2026, 1, 5), new TimeOnly(9, 0)));
        Assert.False(await repository.ExisteAsync(1, new DateOnly(2026, 1, 5), new TimeOnly(10, 45)));
        Assert.False(await repository.ExisteAsync(2, new DateOnly(2026, 1, 5), new TimeOnly(9, 0)));
    }

    [Fact]
    public async Task RemplacerPourSiteEtAnneeAsync_SupprimeLExistantEtInsereLesNouvelles() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.Add(new Site { Id = 1, Nom = "Site 1" });
        context.Disponibilites.AddRange(
            new Disponibilite { SiteId = 1, Date = new DateOnly(2026, 1, 5), HeureDebut = new TimeOnly(9, 0), HeureFin = new TimeOnly(10, 30) },
            new Disponibilite { SiteId = 1, Date = new DateOnly(2025, 1, 5), HeureDebut = new TimeOnly(9, 0), HeureFin = new TimeOnly(10, 30) }); // autre année, doit rester
        await context.SaveChangesAsync();

        var repository = new DisponibiliteRepository(context);
        var nouvelles = new List<Disponibilite> {
            new() { SiteId = 1, Date = new DateOnly(2026, 3, 1), HeureDebut = new TimeOnly(9, 0), HeureFin = new TimeOnly(10, 30) }
        };

        // Act
        await repository.RemplacerPourSiteEtAnneeAsync(1, 2026, nouvelles);

        // Assert
        var toutes = context.Disponibilites.ToList();
        Assert.Equal(2, toutes.Count);
        Assert.Contains(toutes, d => d.Date.Year == 2025);
        Assert.Contains(toutes, d => d.Date == new DateOnly(2026, 3, 1));
    }

    [Fact]
    public async Task CountBySiteAsync_CompteToutesLesDatesSansFiltreDePeriode() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.AddRange(new Site { Id = 1, Nom = "Site 1" }, new Site { Id = 2, Nom = "Site 2" });
        context.Disponibilites.AddRange(
            new Disponibilite { SiteId = 1, Date = new DateOnly(2025, 1, 5), HeureDebut = new TimeOnly(9, 0), HeureFin = new TimeOnly(10, 30) },
            new Disponibilite { SiteId = 1, Date = new DateOnly(2026, 6, 1), HeureDebut = new TimeOnly(9, 0), HeureFin = new TimeOnly(10, 30) }, // année différente, comptée quand même
            new Disponibilite { SiteId = 2, Date = new DateOnly(2025, 1, 5), HeureDebut = new TimeOnly(9, 0), HeureFin = new TimeOnly(10, 30) }); // autre site
        await context.SaveChangesAsync();

        var repository = new DisponibiliteRepository(context);

        // Act
        var resultat = await repository.CountBySiteAsync(1);

        // Assert
        Assert.Equal(2, resultat);
    }
}

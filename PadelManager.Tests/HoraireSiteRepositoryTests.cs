using Microsoft.EntityFrameworkCore;
using PadelManager.Repositories;
using Xunit;

namespace PadelManager.Tests;

public class HoraireSiteRepositoryTests {
    private static PadelManagerDbContext CreerContexteEnMemoire() {
        var options = new DbContextOptionsBuilder<PadelManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PadelManagerDbContext(options);
    }

    [Fact]
    public async Task GetBySiteAndAnneeAsync_HoraireExistant_RetourneLHoraire() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.Add(new Site { Id = 1, Nom = "Site 1" });
        context.HoraireSites.Add(new HoraireSite {
            SiteId = 1,
            Annee = 2026,
            JoursOuverture = "LUN,MER,VEN",
            HeureDebutReservation = new TimeOnly(9, 0),
            HeureFinReservation = new TimeOnly(21, 0)
        });
        await context.SaveChangesAsync();

        var repository = new HoraireSiteRepository(context);

        // Act
        var resultat = await repository.GetBySiteAndAnneeAsync(1, 2026);

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal("LUN,MER,VEN", resultat!.JoursOuverture);
    }

    [Fact]
    public async Task GetBySiteAndAnneeAsync_AucunHoraire_RetourneNull() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        var repository = new HoraireSiteRepository(context);

        // Act
        var resultat = await repository.GetBySiteAndAnneeAsync(1, 2026);

        // Assert
        Assert.Null(resultat);
    }

    [Fact]
    public async Task UpsertAsync_AucunHoraireExistant_Cree() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.Add(new Site { Id = 1, Nom = "Site 1" });
        await context.SaveChangesAsync();

        var repository = new HoraireSiteRepository(context);

        // Act
        await repository.UpsertAsync(new HoraireSite {
            SiteId = 1,
            Annee = 2026,
            JoursOuverture = "LUN,MAR",
            HeureDebutReservation = new TimeOnly(9, 0),
            HeureFinReservation = new TimeOnly(21, 0)
        });

        // Assert
        Assert.Single(context.HoraireSites);
    }

    [Fact]
    public async Task UpsertAsync_HoraireExistant_MetAJourSansDupliquer() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.Add(new Site { Id = 1, Nom = "Site 1" });
        context.HoraireSites.Add(new HoraireSite {
            SiteId = 1,
            Annee = 2026,
            JoursOuverture = "LUN",
            HeureDebutReservation = new TimeOnly(9, 0),
            HeureFinReservation = new TimeOnly(21, 0)
        });
        await context.SaveChangesAsync();

        var repository = new HoraireSiteRepository(context);

        // Act
        await repository.UpsertAsync(new HoraireSite {
            SiteId = 1,
            Annee = 2026,
            JoursOuverture = "LUN,MAR,MER",
            HeureDebutReservation = new TimeOnly(8, 0),
            HeureFinReservation = new TimeOnly(22, 0)
        });

        // Assert
        Assert.Single(context.HoraireSites);
        var horaire = await repository.GetBySiteAndAnneeAsync(1, 2026);
        Assert.Equal("LUN,MAR,MER", horaire!.JoursOuverture);
        Assert.Equal(new TimeOnly(8, 0), horaire.HeureDebutReservation);
    }
}

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
    public async Task GetAllForAnneeAsync_PlusieursSites_RetourneTousLesHorairesDeLAnnee() {
        // Arrange
        await using var context = CreerContexteEnMemoire();
        context.Sites.AddRange(new Site { Id = 1, Nom = "Site 1" }, new Site { Id = 2, Nom = "Site 2" });
        context.HoraireSites.AddRange(
            new HoraireSite { SiteId = 1, Annee = 2026, JoursOuverture = "LUN", HeureDebutReservation = new TimeOnly(9, 0), HeureFinReservation = new TimeOnly(21, 0) },
            new HoraireSite { SiteId = 2, Annee = 2026, JoursOuverture = "MAR", HeureDebutReservation = new TimeOnly(9, 0), HeureFinReservation = new TimeOnly(21, 0) },
            new HoraireSite { SiteId = 1, Annee = 2025, JoursOuverture = "MER", HeureDebutReservation = new TimeOnly(9, 0), HeureFinReservation = new TimeOnly(21, 0) });
        await context.SaveChangesAsync();

        var repository = new HoraireSiteRepository(context);

        // Act
        var resultat = await repository.GetAllForAnneeAsync(2026);

        // Assert
        Assert.Equal(2, resultat.Count);
        Assert.Contains(resultat, h => h.SiteId == 1 && h.JoursOuverture == "LUN");
        Assert.Contains(resultat, h => h.SiteId == 2 && h.JoursOuverture == "MAR");
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

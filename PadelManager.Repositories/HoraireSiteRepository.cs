using Microsoft.EntityFrameworkCore;
using PadelManager.Interfaces;
using PadelManager.Models;

namespace PadelManager.Repositories;

public class HoraireSiteRepository : IHoraireSiteRepository {
    private readonly PadelManagerDbContext _context;

    public HoraireSiteRepository(PadelManagerDbContext context) {
        _context = context;
    }

    public async Task<HoraireSite?> GetBySiteAndAnneeAsync(int siteId, short annee) {
        return await _context.HoraireSites
            .FirstOrDefaultAsync(h => h.SiteId == siteId && h.Annee == annee);
    }

    public async Task<List<HoraireSite>> GetAllForAnneeAsync(short annee) {
        return await _context.HoraireSites
            .Where(h => h.Annee == annee)
            .ToListAsync();
    }

    public async Task UpsertAsync(HoraireSite horaire) {
        var existant = await _context.HoraireSites
            .FirstOrDefaultAsync(h => h.SiteId == horaire.SiteId && h.Annee == horaire.Annee);

        if (existant == null) {
            _context.HoraireSites.Add(horaire);
        } else {
            existant.JoursOuverture = horaire.JoursOuverture;
            existant.HeureDebutReservation = horaire.HeureDebutReservation;
            existant.HeureFinReservation = horaire.HeureFinReservation;
        }

        await _context.SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using PadelManager.Interfaces;
using PadelManager.Models;

namespace PadelManager.Repositories;

public class DisponibiliteRepository : IDisponibiliteRepository {
    private readonly PadelManagerDbContext _context;

    public DisponibiliteRepository(PadelManagerDbContext context) {
        _context = context;
    }

    public async Task<List<Disponibilite>> GetBySiteAndPeriodeAsync(int siteId, DateOnly from, DateOnly to) {
        return await _context.Disponibilites
            .Where(d => d.SiteId == siteId && d.Date >= from && d.Date <= to)
            .OrderBy(d => d.Date).ThenBy(d => d.HeureDebut)
            .ToListAsync();
    }

    public async Task RemplacerPourSiteEtAnneeAsync(int siteId, short annee, List<Disponibilite> nouvelles) {
        var existantes = await _context.Disponibilites
            .Where(d => d.SiteId == siteId && d.Date.Year == annee)
            .ToListAsync();

        _context.Disponibilites.RemoveRange(existantes);
        await _context.Disponibilites.AddRangeAsync(nouvelles);

        await _context.SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using PadelManager.Interfaces;
using PadelManager.Models;

namespace PadelManager.Repositories;

public class JourFermetureRepository : IJourFermetureRepository {
    private readonly PadelManagerDbContext _context;

    public JourFermetureRepository(PadelManagerDbContext context) {
        _context = context;
    }

    public async Task<List<JourFermeture>> GetForSiteAndAnneeAsync(int siteId, short annee) {
        return await _context.JourFermetures
            .Where(j => (j.SiteId == siteId || j.SiteId == null) && j.Date.Year == annee)
            .ToListAsync();
    }

    public async Task<JourFermeture?> GetByIdAsync(int id) {
        return await _context.JourFermetures.FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<bool> ExisteAsync(int? siteId, DateOnly date) {
        return await _context.JourFermetures.AnyAsync(j => j.SiteId == siteId && j.Date == date);
    }

    public async Task<JourFermeture> AddAsync(JourFermeture jour) {
        _context.JourFermetures.Add(jour);
        await _context.SaveChangesAsync();
        return jour;
    }

    public async Task DeleteAsync(int id) {
        var existant = await _context.JourFermetures.FirstOrDefaultAsync(j => j.Id == id);
        if (existant == null)
            return;

        _context.JourFermetures.Remove(existant);
        await _context.SaveChangesAsync();
    }
}

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
}

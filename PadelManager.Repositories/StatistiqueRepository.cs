using Microsoft.EntityFrameworkCore;
using PadelManager.Interfaces;
using PadelManager.Models;

namespace PadelManager.Repositories;

public class StatistiqueRepository : IStatistiqueRepository {
    private readonly PadelManagerDbContext _context;

    public StatistiqueRepository(PadelManagerDbContext context) {
        _context = context;
    }

    public async Task<List<Paiement>> GetPaiementsAsync(int? siteId) {
        var query = _context.Paiements
            .Include(p => p.Participation).ThenInclude(part => part.Match)
            .AsQueryable();

        if (siteId.HasValue)
            query = query.Where(p => p.Participation.Match.SiteId == siteId.Value);

        return await query.ToListAsync();
    }

    public async Task<List<Participation>> GetParticipationsAsync(int? siteId) {
        var query = _context.Participations.Include(p => p.Match).AsQueryable();

        if (siteId.HasValue)
            query = query.Where(p => p.Match.SiteId == siteId.Value);

        return await query.ToListAsync();
    }
}

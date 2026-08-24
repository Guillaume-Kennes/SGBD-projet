using Microsoft.EntityFrameworkCore;
using PadelManager.Interfaces;
using PadelManager.Models;

namespace PadelManager.Repositories;

public class TerrainRepository : ITerrainRepository {
    private readonly PadelManagerDbContext _context;

    public TerrainRepository(PadelManagerDbContext context) {
        _context = context;
    }

    public async Task<Terrain?> GetByIdAsync(int id) {
        return await _context.Terrains.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Terrain>> GetBySiteIdAsync(int siteId) {
        return await _context.Terrains
            .Where(t => t.SiteId == siteId)
            .OrderBy(t => t.Numero)
            .ToListAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using PadelManager.Interfaces;
using PadelManager.Models;

namespace PadelManager.Repositories;

public class SiteRepository : ISiteRepository {
    private readonly PadelManagerDbContext _context;

    public SiteRepository(PadelManagerDbContext context) {
        _context = context;
    }

    public async Task<Site?> GetByIdAsync(int id) {
        return await _context.Sites.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Site>> GetAllAsync() {
        return await _context.Sites.OrderBy(s => s.Nom).ToListAsync();
    }
}

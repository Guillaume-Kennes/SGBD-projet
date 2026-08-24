using Microsoft.EntityFrameworkCore;
using PadelManager.Interfaces;
using PadelManager.Models;

namespace PadelManager.Repositories;

public class PenaliteRepository : IPenaliteRepository {
    private readonly PadelManagerDbContext _context;

    public PenaliteRepository(PadelManagerDbContext context) {
        _context = context;
    }

    public async Task<Penalite?> GetPlusRecenteAsync(string membreMatricule) {
        return await _context.Penalites
            .Where(p => p.MembreMatricule == membreMatricule)
            .OrderByDescending(p => p.DateApplication)
            .FirstOrDefaultAsync();
    }
}

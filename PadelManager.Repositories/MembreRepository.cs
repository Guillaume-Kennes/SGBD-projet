using Microsoft.EntityFrameworkCore;
using PadelManager.Interfaces;
using PadelManager.Models;

namespace PadelManager.Repositories;

public class MembreRepository : IMembreRepository {
    private readonly PadelManagerDbContext _context;

    public MembreRepository(PadelManagerDbContext context) {
        _context = context;
    }

    public async Task<Membre?> GetByMatriculeAsync(string matricule) {
        return await _context.Membres
            .Include(m => m.TypeMembreNavigation)
            .Include(m => m.Site)
            .FirstOrDefaultAsync(m => m.Matricule == matricule);
    }
}


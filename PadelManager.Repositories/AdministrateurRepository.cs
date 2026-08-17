using Microsoft.EntityFrameworkCore;
using PadelManager.Interfaces;
using PadelManager.Models;

namespace PadelManager.Repositories;

public class AdministrateurRepository : IAdministrateurRepository {
    private readonly PadelManagerDbContext _context;

    public AdministrateurRepository(PadelManagerDbContext context) {
        _context = context;
    }

    public async Task<Administrateur?> GetByMatriculeAsync(string matricule) {
        return await _context.Administrateurs
            .Include(a => a.Site)
            .FirstOrDefaultAsync(a => a.Matricule == matricule);
    }
}


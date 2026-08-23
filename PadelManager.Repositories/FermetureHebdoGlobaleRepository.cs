using Microsoft.EntityFrameworkCore;
using PadelManager.Interfaces;
using PadelManager.Models;

namespace PadelManager.Repositories;

public class FermetureHebdoGlobaleRepository : IFermetureHebdoGlobaleRepository {
    private readonly PadelManagerDbContext _context;

    public FermetureHebdoGlobaleRepository(PadelManagerDbContext context) {
        _context = context;
    }

    public async Task<FermetureHebdoGlobale?> GetByAnneeAsync(short annee) {
        return await _context.FermetureHebdoGlobales
            .FirstOrDefaultAsync(f => f.Annee == annee);
    }
}

using Microsoft.EntityFrameworkCore;
using PadelManager.Interfaces;

namespace PadelManager.Repositories;

public class DetteRepository : IDetteRepository {
    private readonly PadelManagerDbContext _context;

    public DetteRepository(PadelManagerDbContext context) {
        _context = context;
    }

    public async Task<bool> ExisteDetteNonSoldeeAsync(string membreMatricule) {
        return await _context.Dettes.AnyAsync(d => d.MembreMatricule == membreMatricule && !d.Soldee);
    }
}

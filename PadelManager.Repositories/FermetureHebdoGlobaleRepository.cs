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

    public async Task UpsertAsync(FermetureHebdoGlobale fermeture) {
        var existante = await _context.FermetureHebdoGlobales
            .FirstOrDefaultAsync(f => f.Annee == fermeture.Annee);

        if (existante == null) {
            _context.FermetureHebdoGlobales.Add(fermeture);
        } else {
            existante.JoursFermes = fermeture.JoursFermes;
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(short annee) {
        var existante = await _context.FermetureHebdoGlobales
            .FirstOrDefaultAsync(f => f.Annee == annee);
        if (existante == null)
            return;

        _context.FermetureHebdoGlobales.Remove(existante);
        await _context.SaveChangesAsync();
    }
}

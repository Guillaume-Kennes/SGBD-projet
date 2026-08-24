using Microsoft.EntityFrameworkCore;
using PadelManager.Interfaces;
using PadelManager.Models;

namespace PadelManager.Repositories;

public class MatchRepository : IMatchRepository {
    private readonly PadelManagerDbContext _context;

    public MatchRepository(PadelManagerDbContext context) {
        _context = context;
    }

    public async Task<List<Match>> GetForSiteAndDateAsync(int siteId, DateOnly date) {
        // Bornes explicites plutôt que DateHeure.Date == ... (non-sargable, empêche l'usage
        // d'un index sur la colonne).
        var debut = date.ToDateTime(TimeOnly.MinValue);
        var fin = date.AddDays(1).ToDateTime(TimeOnly.MinValue);

        return await _context.Matches
            .Where(m => m.SiteId == siteId && m.DateHeure >= debut && m.DateHeure < fin)
            .ToListAsync();
    }

    public async Task<bool> ExisteAsync(int terrainId, DateTime dateHeure) {
        return await _context.Matches.AnyAsync(m => m.TerrainId == terrainId && m.DateHeure == dateHeure);
    }

    public async Task<Match> AddAsync(Match match) {
        _context.Matches.Add(match);

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateException) {
            // Filet de sécurité DB (UQ_MATCH_terrain_creneau) : conflit concurrent réel malgré
            // la revérification déjà faite par le service (EF-bk-019, même principe qu'ENF-010).
            throw new CreneauIndisponibleException();
        }

        return match;
    }
}

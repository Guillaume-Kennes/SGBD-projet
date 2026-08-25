using Microsoft.EntityFrameworkCore;
using PadelManager.Interfaces;
using PadelManager.Models;

namespace PadelManager.Repositories;

public class JobRepository : IJobRepository {
    private readonly PadelManagerDbContext _context;

    public JobRepository(PadelManagerDbContext context) {
        _context = context;
    }

    public async Task<List<Match>> GetMatchsPrivesDeLaDateAsync(DateOnly date) {
        var debut = date.ToDateTime(TimeOnly.MinValue);
        var fin = date.AddDays(1).ToDateTime(TimeOnly.MinValue);

        return await _context.Matches
            .Include(m => m.Participations).ThenInclude(p => p.Paiement)
            .Where(m => m.Visibilite == "PRIVE" && m.DateHeure >= debut && m.DateHeure < fin)
            .ToListAsync();
    }

    public async Task<List<Match>> GetMatchsDeLaDateAsync(DateOnly date) {
        var debut = date.ToDateTime(TimeOnly.MinValue);
        var fin = date.AddDays(1).ToDateTime(TimeOnly.MinValue);

        return await _context.Matches
            .Include(m => m.Participations).ThenInclude(p => p.Paiement)
            .Where(m => m.DateHeure >= debut && m.DateHeure < fin)
            .ToListAsync();
    }

    public async Task BasculerAsync(Match match, List<Participation> participationsNonPayees, Penalite penalite) {
        _context.Participations.RemoveRange(participationsNonPayees);
        match.Visibilite = "PUBLIC";
        _context.Penalites.Add(penalite);
        await _context.SaveChangesAsync();
    }

    public async Task CreerDetteAsync(Dette dette) {
        _context.Dettes.Add(dette);
        await _context.SaveChangesAsync();
    }

    public async Task ScellerTermineAsync(Match match) {
        match.Statut = "TERMINE";
        await _context.SaveChangesAsync();
    }
}

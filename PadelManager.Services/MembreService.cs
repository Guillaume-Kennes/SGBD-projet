using PadelManager.Interfaces;
using PadelManager.Models.Dtos;

namespace PadelManager.Services;

public class MembreService : IMembreService {
    private readonly IMembreRepository _membreRepository;
    private readonly IDetteRepository _detteRepository;
    private readonly IPenaliteRepository _penaliteRepository;

    public MembreService(IMembreRepository membreRepository, IDetteRepository detteRepository, IPenaliteRepository penaliteRepository) {
        _membreRepository = membreRepository;
        _detteRepository = detteRepository;
        _penaliteRepository = penaliteRepository;
    }

    public async Task<List<MembreAdminDto>> ObtenirMembresAsync(int? siteId) {
        var membres = await _membreRepository.GetTousAsync(siteId);
        var aujourdHui = DateOnly.FromDateTime(DateTime.Today);

        var resultat = new List<MembreAdminDto>();
        foreach (var membre in membres) {
            var detteActive = await _detteRepository.ExisteDetteNonSoldeeAsync(membre.Matricule);

            // R-CALC-004 : même règle qu'à la création d'un match — bloquante tant que la date du
            // jour est strictement avant delaiJusquAu.
            var penalite = await _penaliteRepository.GetPlusRecenteAsync(membre.Matricule);
            var penaliteActive = penalite != null && penalite.DelaiJusquAu > aujourdHui;

            resultat.Add(new MembreAdminDto {
                Matricule = membre.Matricule,
                TypeMembre = membre.TypeMembre,
                SiteId = membre.SiteId,
                DetteActive = detteActive,
                PenaliteActive = penaliteActive
            });
        }

        return resultat.OrderBy(m => m.Matricule).ToList();
    }
}

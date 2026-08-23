using PadelManager.Interfaces;
using PadelManager.Models;
using PadelManager.Models.Dtos;

namespace PadelManager.Services;

public class DisponibiliteService : IDisponibiliteService {
    private readonly ISiteRepository _siteRepository;
    private readonly IDisponibiliteRepository _disponibiliteRepository;

    public DisponibiliteService(ISiteRepository siteRepository, IDisponibiliteRepository disponibiliteRepository) {
        _siteRepository = siteRepository;
        _disponibiliteRepository = disponibiliteRepository;
    }

    public async Task<List<DisponibiliteDto>?> ConsulterPlanningAsync(int siteId, DateOnly from, DateOnly to) {
        if (await _siteRepository.GetByIdAsync(siteId) == null)
            return null;

        if (from > to)
            return new List<DisponibiliteDto>();

        var disponibilites = await _disponibiliteRepository.GetBySiteAndPeriodeAsync(siteId, from, to);
        return disponibilites.Select(VersDto).ToList();
    }

    private static DisponibiliteDto VersDto(Disponibilite disponibilite) => new() {
        SiteId = disponibilite.SiteId,
        Date = disponibilite.Date,
        HeureDebut = disponibilite.HeureDebut,
        HeureFin = disponibilite.HeureFin
    };
}

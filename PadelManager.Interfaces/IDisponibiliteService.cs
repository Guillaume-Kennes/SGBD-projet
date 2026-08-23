using PadelManager.Models.Dtos;

namespace PadelManager.Interfaces;

public interface IDisponibiliteService {
    // Consultation du planning (EF-bk-002). Retourne null si le site est inconnu.
    Task<List<DisponibiliteDto>?> ConsulterPlanningAsync(int siteId, DateOnly from, DateOnly to);
}

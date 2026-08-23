using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface IHoraireSiteRepository {
    Task<HoraireSite?> GetBySiteAndAnneeAsync(int siteId, short annee);

    // Crée ou met à jour l'horaire du site pour l'année (unique par siteId+annee).
    Task UpsertAsync(HoraireSite horaire);
}

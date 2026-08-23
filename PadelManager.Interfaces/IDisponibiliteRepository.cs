using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface IDisponibiliteRepository {
    Task<List<Disponibilite>> GetBySiteAndPeriodeAsync(int siteId, DateOnly from, DateOnly to);

    // Supprime toutes les disponibilités existantes du site pour l'année, puis insère les nouvelles.
    Task RemplacerPourSiteEtAnneeAsync(int siteId, short annee, List<Disponibilite> nouvelles);
}

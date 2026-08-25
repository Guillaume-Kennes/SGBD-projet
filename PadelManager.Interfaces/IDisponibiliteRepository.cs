using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface IDisponibiliteRepository {
    Task<List<Disponibilite>> GetBySiteAndPeriodeAsync(int siteId, DateOnly from, DateOnly to);

    // Le créneau (site, date, heureDebut) fait-il partie des disponibilités générées ?
    // (EF-bk-020, évite de charger toute une période pour vérifier un seul créneau.)
    Task<bool> ExisteAsync(int siteId, DateOnly date, TimeOnly heureDebut);

    // Supprime toutes les disponibilités existantes du site pour l'année, puis insère les nouvelles.
    Task RemplacerPourSiteEtAnneeAsync(int siteId, short annee, List<Disponibilite> nouvelles);
}

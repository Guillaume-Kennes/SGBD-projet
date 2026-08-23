using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface IHoraireSiteRepository {
    Task<HoraireSite?> GetBySiteAndAnneeAsync(int siteId, short annee);

    // Tous les HORAIRE_SITE existants pour l'année, tous sites confondus. Utilisé pour valider
    // R-STR-006 lors de l'écriture de FERMETURE_HEBDO_GLOBALE (sens inverse de la vérification
    // déjà faite par HoraireSiteService).
    Task<List<HoraireSite>> GetAllForAnneeAsync(short annee);

    // Crée ou met à jour l'horaire du site pour l'année (unique par siteId+annee).
    Task UpsertAsync(HoraireSite horaire);
}

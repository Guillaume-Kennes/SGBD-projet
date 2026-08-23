using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface IJourFermetureRepository {
    // Fermetures ponctuelles concernant le site (siteId = celui du site OU siteId NULL,
    // fermeture ponctuelle globale) pour l'année donnée.
    Task<List<JourFermeture>> GetForSiteAndAnneeAsync(int siteId, short annee);

    Task<JourFermeture?> GetByIdAsync(int id);

    // Une même date ne peut être déclarée fermée qu'une fois pour un site donné (siteId NULL
    // pour une fermeture globale) : cf. contrainte UQ_JOUR_FERMETURE_site_date.
    Task<bool> ExisteAsync(int? siteId, DateOnly date);

    Task<JourFermeture> AddAsync(JourFermeture jour);

    Task DeleteAsync(int id);
}

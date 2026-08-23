using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface IJourFermetureRepository {
    // Fermetures ponctuelles concernant le site (siteId = celui du site OU siteId NULL,
    // fermeture ponctuelle globale) pour l'année donnée.
    Task<List<JourFermeture>> GetForSiteAndAnneeAsync(int siteId, short annee);
}

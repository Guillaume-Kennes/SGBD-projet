namespace PadelManager.Interfaces;

public interface IDisponibiliteGenerationService {
    // Génère (en remplaçant l'existant) les disponibilités du site pour l'année, à partir de
    // HORAIRE_SITE et JOUR_FERMETURE (EF-bk-022).
    // Retourne le nombre de créneaux générés, ou null si aucun HoraireSite n'est configuré.
    Task<int?> GenererPourSiteEtAnneeAsync(int siteId, short annee);
}

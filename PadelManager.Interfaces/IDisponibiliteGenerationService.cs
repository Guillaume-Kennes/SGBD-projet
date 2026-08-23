namespace PadelManager.Interfaces;

public interface IDisponibiliteGenerationService {
    // Génère (en remplaçant l'existant) les disponibilités du site pour l'année, à partir de
    // HORAIRE_SITE et JOUR_FERMETURE (EF-bk-022).
    // Retourne le nombre de créneaux générés, ou null si aucun HoraireSite n'est configuré.
    Task<int?> GenererPourSiteEtAnneeAsync(int siteId, short annee);

    // Régénère les disponibilités de tous les sites ayant un HORAIRE_SITE configuré pour
    // l'année (sites sans horaire simplement ignorés). Utilisé après une écriture portant sur
    // FERMETURE_HEBDO_GLOBALE ou sur une fermeture ponctuelle globale (JOUR_FERMETURE.siteId
    // NULL), qui impactent potentiellement tous les sites. Retourne le nombre total de
    // créneaux générés.
    Task<int> GenererPourTousLesSitesEtAnneeAsync(short annee);
}

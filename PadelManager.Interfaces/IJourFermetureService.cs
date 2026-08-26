using PadelManager.Models.Dtos;

namespace PadelManager.Interfaces;

public interface IJourFermetureService {
    Task<List<JourFermetureDto>> ObtenirPourSiteEtAnneeAsync(int siteId, short annee);

    // Récupère une fermeture ponctuelle par id (issue #13) : permet au contrôleur de connaître
    // son SiteId (jamais fourni par le client sur une suppression par id) avant de vérifier la
    // portée de l'admin appelant. Retourne null si l'id est inconnu.
    Task<JourFermetureDto?> ObtenirParIdAsync(int id);

    // Déclaration d'une fermeture ponctuelle (EF-bk-024), pour un site donné ou globale
    // (requete.SiteId == null). Régénère automatiquement les disponibilités concernées
    // (EF-bk-022) en cas de succès.
    Task<DeclarerFermetureResultatDto> DeclarerAsync(JourFermetureRequestDto requete);

    // Annule une fermeture ponctuelle déjà déclarée. Retourne false si l'id est inconnu.
    Task<bool> SupprimerAsync(int id);
}

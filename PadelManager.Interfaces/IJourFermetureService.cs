using PadelManager.Models.Dtos;

namespace PadelManager.Interfaces;

public interface IJourFermetureService {
    Task<List<JourFermetureDto>> ObtenirPourSiteEtAnneeAsync(int siteId, short annee);

    // Déclaration d'une fermeture ponctuelle (EF-bk-024), pour un site donné ou globale
    // (requete.SiteId == null). Régénère automatiquement les disponibilités concernées
    // (EF-bk-022) en cas de succès.
    Task<DeclarerFermetureResultatDto> DeclarerAsync(JourFermetureRequestDto requete);

    // Annule une fermeture ponctuelle déjà déclarée. Retourne false si l'id est inconnu.
    Task<bool> SupprimerAsync(int id);
}

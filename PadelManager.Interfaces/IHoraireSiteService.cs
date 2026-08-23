using PadelManager.Models.Dtos;

namespace PadelManager.Interfaces;

public interface IHoraireSiteService {
    Task<HoraireSiteDto?> ObtenirHoraireAsync(int siteId, short annee);

    // Paramétrage annuel du site (EF-bk-003). Régénère automatiquement les disponibilités
    // (EF-bk-022) en cas de succès.
    Task<DefinirHoraireResultatDto> DefinirHoraireAsync(int siteId, short annee, HoraireSiteRequestDto requete);
}

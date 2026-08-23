using PadelManager.Models.Dtos;

namespace PadelManager.Interfaces;

public interface IFermetureHebdoGlobaleService {
    Task<FermetureHebdoGlobaleDto?> ObtenirAsync(short annee);

    // Paramétrage de la fermeture hebdomadaire globale de l'année (EF-bk-023), réservé à
    // l'administrateur global. Régénère automatiquement les disponibilités de tous les sites
    // (EF-bk-022) en cas de succès.
    Task<DefinirFermetureHebdoGlobaleResultatDto> DefinirAsync(short annee, FermetureHebdoGlobaleRequestDto requete);

    // Repasse l'année à "aucun jour fermé globalement". Retourne false si l'année n'avait
    // aucune fermeture hebdomadaire globale définie.
    Task<bool> SupprimerAsync(short annee);
}

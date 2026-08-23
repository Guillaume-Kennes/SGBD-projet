using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface IFermetureHebdoGlobaleRepository {
    Task<FermetureHebdoGlobale?> GetByAnneeAsync(short annee);

    // Crée ou met à jour la fermeture hebdomadaire globale de l'année (PK = annee).
    Task UpsertAsync(FermetureHebdoGlobale fermeture);

    // Supprime la ligne de l'année (retour à "aucun jour fermé globalement" pour cette année).
    Task DeleteAsync(short annee);
}

using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface IDetteRepository {
    // R-ACC-006 : un organisateur ayant un solde impayé ne peut créer aucune nouvelle réservation.
    Task<bool> ExisteDetteNonSoldeeAsync(string membreMatricule);

    // La dette active du membre, le cas échéant (jamais plus d'une à la fois : R-ACC-006 bloque
    // toute nouvelle création tant qu'une dette existe). Utilisée pour le report automatique
    // lors d'une inscription à un match public (EF-bk-018).
    Task<Dette?> GetNonSoldeeAsync(string membreMatricule);
}

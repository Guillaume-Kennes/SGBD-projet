namespace PadelManager.Interfaces;

public interface IDetteRepository {
    // R-ACC-006 : un organisateur ayant un solde impayé ne peut créer aucune nouvelle réservation.
    Task<bool> ExisteDetteNonSoldeeAsync(string membreMatricule);
}

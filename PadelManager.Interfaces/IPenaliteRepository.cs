using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface IPenaliteRepository {
    // Pénalité la plus récente du membre (R-CALC-004) ; peut être expirée (DelaiJusquAu passé),
    // au Service de juger si elle est encore active.
    Task<Penalite?> GetPlusRecenteAsync(string membreMatricule);
}

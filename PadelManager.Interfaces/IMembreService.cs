using PadelManager.Models.Dtos;

namespace PadelManager.Interfaces;

public interface IMembreService {
    // Liste des membres pour la vue administrateur (EF-bk-017). siteId fourni -> uniquement les
    // membres SITE de ce site ; omis -> tous les membres, tous types (portée non vérifiée côté
    // API, comme les autres écrans admin de ce projet). DetteActive/PenaliteActive réutilisent la
    // même logique de blocage qu'à la création d'un match (R-ACC-006 / R-CALC-004).
    Task<List<MembreAdminDto>> ObtenirMembresAsync(int? siteId);
}

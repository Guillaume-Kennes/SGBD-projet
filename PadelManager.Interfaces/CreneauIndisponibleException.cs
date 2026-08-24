namespace PadelManager.Interfaces;

// Levée par IMatchRepository.AddAsync lorsque le filet de sécurité DB (UQ_MATCH_terrain_creneau)
// détecte un conflit concurrent réel (EF-bk-019) : traduit une DbUpdateException en une erreur
// métier, sans faire fuiter Entity Framework Core jusqu'à la couche Services.
public class CreneauIndisponibleException : Exception {
}

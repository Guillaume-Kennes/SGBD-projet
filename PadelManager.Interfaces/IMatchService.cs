using PadelManager.Models.Dtos;

namespace PadelManager.Interfaces;

public interface IMatchService {
    // Créneaux réellement libres, par terrain, pour un site et une date donnés (DISPONIBILITE
    // croisée avec les MATCH déjà réservés). Retourne null si le site est inconnu.
    Task<List<CreneauMatchDto>?> ObtenirCreneauxDisponiblesAsync(int siteId, DateOnly date);

    // Création d'un match privé (EF-bk-004) : organisateur + jusqu'à 3 joueurs + paiement
    // immédiat de l'organisateur, en une seule opération (R-VAL-005).
    Task<CreerMatchResultatDto> CreerMatchPriveAsync(CreerMatchPriveRequestDto requete);

    // Création directe d'un match public (EF-bk-002) : organisateur + paiement immédiat
    // uniquement, sans ajout de joueur (R-ACC-005) — les 3 places restantes sont ouvertes à
    // l'inscription individuelle (EF-bk-006, hors périmètre de cette méthode).
    Task<CreerMatchResultatDto> CreerMatchPublicAsync(CreerMatchPublicRequestDto requete);
}

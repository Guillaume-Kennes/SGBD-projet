using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface IMatchRepository {
    // Matchs déjà réservés sur le site pour la date donnée (utilisé pour exclure les créneaux
    // déjà occupés lors du calcul des disponibilités réelles par terrain).
    Task<List<Match>> GetForSiteAndDateAsync(int siteId, DateOnly date);

    // Revérification explicite juste avant l'enregistrement (EF-bk-019) : le terrain a-t-il déjà
    // un match sur ce créneau exact ?
    Task<bool> ExisteAsync(int terrainId, DateTime dateHeure);

    // Insère le match avec son graphe complet (Participations + Paiement de l'organisateur),
    // construit par le service, en une seule opération (R-VAL-005).
    Task<Match> AddAsync(Match match);
}

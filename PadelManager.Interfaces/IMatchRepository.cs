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

    Task<Match?> GetByIdAsync(int id);

    // Matchs publics encore incomplets et pas encore commencés (EF-bk-005), Site/Terrain inclus
    // pour l'affichage ; le filtrage par portée/délai du membre est du ressort du Service.
    Task<List<Match>> GetPublicsIncompletsAsync(DateTime maintenant);

    // Inscrit le membre et valide sa participation par paiement immédiat (EF-bk-006/007), sous
    // verrou (UPDLOCK/HOLDLOCK sur la ligne MATCH) pour ne jamais dépasser 4 participations en
    // cas de concurrence (ENF-010, R-STR-002). Solde la dette fournie le cas échéant (EF-bk-018)
    // et bascule le match à COMPLET si c'est la 4e participation validée.
    Task<Participation> InscrireEtPayerAsync(int matchId, string membreMatricule, Dette? detteAReporter);

    Task<Participation?> GetParticipationByIdAsync(int id);

    // Valide par paiement une participation déjà existante et non payée (EF-bk-007 : joueur
    // ajouté à un match privé qui paie sa part en attente). Même verrou que InscrireEtPayerAsync
    // (sur le MATCH de cette participation) pour déterminer correctement, même en cas de
    // paiements concurrents par plusieurs joueurs du même match, si c'est la 4e participation
    // désormais payée (bascule à COMPLET). Solde la dette fournie le cas échéant (EF-bk-018).
    Task<Participation> PayerParticipationAsync(Participation participation, Dette? detteAReporter);

    // Participations d'un membre encore en attente de paiement (EF-bk-007 : ajouté par un
    // organisateur à un match privé), Match/Site/Terrain inclus pour l'affichage.
    Task<List<Participation>> GetParticipationsEnAttenteAsync(string membreMatricule);

    // Tous les matchs où le membre est organisateur OU participant, passés ou à venir, quel que
    // soit le statut ou la visibilité (EF-bk-013). Site/Terrain/Participations inclus pour
    // l'affichage.
    Task<List<Match>> GetReservationsAsync(string membreMatricule);

    // Détail complet d'un match (EF-bk-021) : Site/Terrain/Participations+Paiement inclus, pour
    // afficher le statut de paiement de chaque joueur.
    Task<Match?> GetDetailAsync(int id);
}

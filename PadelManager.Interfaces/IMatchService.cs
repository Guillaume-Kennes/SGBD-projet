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

    // Matchs publics encore incomplets, filtrés selon la portée du membre (EF-bk-005). Retourne
    // null si le membre est inconnu.
    Task<List<MatchPublicDto>?> ObtenirMatchsPublicsAsync(string membreMatricule);

    // Inscription + paiement immédiat d'une place libre (EF-bk-006/007), avec règlement
    // automatique d'une dette active le cas échéant (EF-bk-018).
    Task<InscriptionResultatDto> RejoindreMatchPublicAsync(int matchId, string membreMatricule);

    // Montant que le membre paierait pour valider une participation (15€, plus le report d'une
    // dette active le cas échéant) : à afficher avant l'action (rejoindre un match public, payer
    // une participation en attente), jamais après coup.
    Task<MontantAPayerDto> ObtenirMontantAPayerAsync(string membreMatricule);

    // Paiement d'une participation déjà existante et en attente (EF-bk-007 : joueur ajouté à un
    // match privé par l'organisateur, R-VAL-005). Règlement automatique d'une dette active le cas
    // échéant (EF-bk-018), comme pour l'inscription à un match public.
    Task<InscriptionResultatDto> PayerParticipationAsync(int participationId, string membreMatricule);

    // Participations d'un membre en attente de paiement, tous matchs privés confondus (EF-bk-007),
    // pour l'écran où il valide sa place en payant. Retourne null si le membre est inconnu.
    Task<List<ParticipationEnAttenteDto>?> ObtenirParticipationsEnAttenteAsync(string membreMatricule);

    // Tous les matchs où le membre est organisateur ou participant, passés ou à venir (EF-bk-013).
    // Retourne null si le membre est inconnu.
    Task<List<ReservationDto>?> ObtenirReservationsAsync(string membreMatricule);

    // Détail d'un match (EF-bk-021) : autorisé si le membre est organisateur/participant, ou si le
    // match est public et dans son périmètre de consultation (EF-bk-012). Retourne null si le
    // match n'existe pas, si le membre est inconnu, OU si l'un et l'autre existent mais que le
    // membre n'est pas autorisé à le consulter — un match privé auquel il est étranger ne doit pas
    // se distinguer, côté client, d'un match qui n'existe simplement pas.
    Task<MatchDetailDto?> ObtenirDetailAsync(int matchId, string membreMatricule);
}

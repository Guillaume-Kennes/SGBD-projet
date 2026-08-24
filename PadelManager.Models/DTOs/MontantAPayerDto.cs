namespace PadelManager.Models.Dtos;

// Montant à payer pour valider une participation (organisateur à la création, joueur qui
// rejoint un match public, ou joueur qui paie sa participation en attente sur un match privé) :
// 15€ de base, plus le report d'une dette active éventuelle (EF-bk-018). Un membre avec une
// dette non soldée ne peut de toute façon rien créer (R-ACC-006) — ce montant n'a donc de sens
// qu'au moment de rejoindre/payer, jamais à la création.
public class MontantAPayerDto {
    public decimal MontantParticipation { get; set; }
    public decimal? MontantDette { get; set; }
    public decimal MontantTotal { get; set; }
}

namespace PadelManager.Models.Dtos;

// Chiffre d'affaires d'un site (EF-bk-015, R-CALC-005) : somme de PAIEMENT.montantTotal des
// paiements effectués sur ce site (rattachement via PARTICIPATION.matchId -> MATCH.siteId, donc
// le site où le paiement a eu lieu, pas le site du match d'origine d'une dette reportée).
public class ChiffreAffairesDto {
    public int SiteId { get; set; }
    public string NomSite { get; set; } = null!;
    public decimal Montant { get; set; }
}

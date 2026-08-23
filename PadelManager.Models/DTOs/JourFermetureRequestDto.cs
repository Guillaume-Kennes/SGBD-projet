namespace PadelManager.Models.Dtos;

// Corps de la requête POST de déclaration d'une fermeture ponctuelle (EF-bk-024).
// SiteId == null -> fermeture globale (tous les sites, réservé à l'admin global côté UI).
public class JourFermetureRequestDto {
    public int? SiteId { get; set; }
    public DateOnly Date { get; set; }
}

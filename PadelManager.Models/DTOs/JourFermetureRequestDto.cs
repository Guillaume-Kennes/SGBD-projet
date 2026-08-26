namespace PadelManager.Models.Dtos;

// Corps de la requête POST de déclaration d'une fermeture ponctuelle (EF-bk-024).
// SiteId == null -> fermeture globale (tous les sites, réservé à l'admin global — désormais
// vérifié aussi côté serveur, issue #13, cf. IAdminPorteeService.VerifierPorteeSiteAsync).
public class JourFermetureRequestDto {
    // Matricule de l'admin appelant (issue #13, contrôle de portée serveur).
    public string AdminMatricule { get; set; } = null!;
    public int? SiteId { get; set; }
    public DateOnly Date { get; set; }
}

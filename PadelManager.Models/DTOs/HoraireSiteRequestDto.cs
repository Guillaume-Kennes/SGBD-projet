namespace PadelManager.Models.Dtos;

// Corps de la requête PUT de paramétrage annuel (EF-bk-003).
// SiteId et Annee viennent de la route, pas du body.
public class HoraireSiteRequestDto {
    // Matricule de l'admin appelant (issue #13, contrôle de portée serveur) — vérifié contre le
    // siteId de la route avant tout traitement.
    public string AdminMatricule { get; set; } = null!;
    public List<string> JoursOuverture { get; set; } = new();
    public TimeOnly HeureDebutReservation { get; set; }
    public TimeOnly HeureFinReservation { get; set; }
}

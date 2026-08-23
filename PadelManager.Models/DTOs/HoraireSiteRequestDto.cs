namespace PadelManager.Models.Dtos;

// Corps de la requête PUT de paramétrage annuel (EF-bk-003).
// SiteId et Annee viennent de la route, pas du body.
public class HoraireSiteRequestDto {
    public List<string> JoursOuverture { get; set; } = new();
    public TimeOnly HeureDebutReservation { get; set; }
    public TimeOnly HeureFinReservation { get; set; }
}

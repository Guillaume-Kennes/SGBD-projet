namespace PadelManager.Models.Dtos;

public class HoraireSiteDto {
    public int SiteId { get; set; }
    public int Annee { get; set; }
    public List<string> JoursOuverture { get; set; } = new();
    public TimeOnly HeureDebutReservation { get; set; }
    public TimeOnly HeureFinReservation { get; set; }
}

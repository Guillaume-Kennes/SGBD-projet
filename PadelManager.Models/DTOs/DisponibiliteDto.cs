namespace PadelManager.Models.Dtos;

public class DisponibiliteDto {
    public int SiteId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly HeureDebut { get; set; }
    public TimeOnly HeureFin { get; set; }
}

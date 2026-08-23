namespace PadelManager.Models.Dtos;

public class JourFermetureDto {
    public int Id { get; set; }
    public int? SiteId { get; set; }
    public DateOnly Date { get; set; }
}

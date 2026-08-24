namespace PadelManager.Models.Dtos;

// Match public encore incomplet, tel que listé pour consultation (EF-bk-005).
public class MatchPublicDto {
    public int Id { get; set; }
    public int SiteId { get; set; }
    public string NomSite { get; set; } = null!;
    public int TerrainId { get; set; }
    public int NumeroTerrain { get; set; }
    public DateTime DateHeure { get; set; }
    public int PlacesRestantes { get; set; }
}

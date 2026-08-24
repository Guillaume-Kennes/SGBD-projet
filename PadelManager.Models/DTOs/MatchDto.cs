namespace PadelManager.Models.Dtos;

public class MatchDto {
    public int Id { get; set; }
    public int SiteId { get; set; }
    public int TerrainId { get; set; }
    public DateTime DateHeure { get; set; }
    public string Visibilite { get; set; } = null!;
    public string OrganisateurMatricule { get; set; } = null!;
    public string Statut { get; set; } = null!;

    // Tous les participants (organisateur inclus).
    public List<string> Joueurs { get; set; } = new();
}

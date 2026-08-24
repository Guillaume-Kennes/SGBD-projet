namespace PadelManager.Models.Dtos;

// Détail d'un match (EF-bk-021) : site, terrain, date/heure, visibilité, statut, et la liste des
// joueurs inscrits avec le statut de paiement de chacun.
public class MatchDetailDto {
    public int Id { get; set; }
    public int SiteId { get; set; }
    public string NomSite { get; set; } = null!;
    public int TerrainId { get; set; }
    public int NumeroTerrain { get; set; }
    public DateTime DateHeure { get; set; }
    public string Visibilite { get; set; } = null!;
    public string Statut { get; set; } = null!;
    public string OrganisateurMatricule { get; set; } = null!;
    public List<JoueurDetailDto> Joueurs { get; set; } = new();
}

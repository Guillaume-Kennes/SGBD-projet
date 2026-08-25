namespace PadelManager.Models.Dtos;

// État d'un match pour la vue administrateur (EF-bk-014) : contrairement aux DTOs côté membre,
// les identifiants (match, terrain) sont exposés pour faciliter le contrôle/debug par l'admin.
public class AdminMatchDto {
    public int Id { get; set; }
    public int SiteId { get; set; }
    public string NomSite { get; set; } = null!;
    public int TerrainId { get; set; }
    public int NumeroTerrain { get; set; }
    public DateTime DateHeure { get; set; }
    public string Visibilite { get; set; } = null!;
    public string Statut { get; set; } = null!;
}

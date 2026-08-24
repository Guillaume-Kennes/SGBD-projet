namespace PadelManager.Models.Dtos;

// Créneau réellement libre pour un terrain donné (DISPONIBILITE du site croisée avec les MATCH
// déjà réservés sur ce terrain) : ce que l'organisateur choisit pour créer un match (EF-bk-004).
public class CreneauMatchDto {
    public int TerrainId { get; set; }
    public int NumeroTerrain { get; set; }
    public TimeOnly HeureDebut { get; set; }
    public TimeOnly HeureFin { get; set; }
}

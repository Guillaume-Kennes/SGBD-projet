namespace PadelManager.Models.Dtos;

// Une réservation dans la liste "Mes réservations" (EF-bk-013) : tout match où le membre est
// organisateur ou participant, passé ou à venir, quel que soit son statut ou sa visibilité.
public class ReservationDto {
    public int Id { get; set; }
    public int SiteId { get; set; }
    public string NomSite { get; set; } = null!;
    public int TerrainId { get; set; }
    public int NumeroTerrain { get; set; }
    public DateTime DateHeure { get; set; }
    public string Visibilite { get; set; } = null!;
    public string Statut { get; set; } = null!;
    public bool EstOrganisateur { get; set; }
}

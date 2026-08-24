namespace PadelManager.Models.Dtos;

// Participation à un match privé en attente de paiement (EF-bk-007) : le membre a été ajouté par
// l'organisateur à la création (CreerMatchPriveAsync), sa place n'est confirmée qu'une fois payée.
public class ParticipationEnAttenteDto {
    public int ParticipationId { get; set; }
    public int MatchId { get; set; }
    public int SiteId { get; set; }
    public string NomSite { get; set; } = null!;
    public int TerrainId { get; set; }
    public int NumeroTerrain { get; set; }
    public DateTime DateHeure { get; set; }
    public string OrganisateurMatricule { get; set; } = null!;
}

namespace PadelManager.Models.Dtos;

// Corps de la requête POST de création directe d'un match public (EF-bk-002). Terrain et
// créneau proviennent des disponibilités déjà proposées (EF-bk-019), comme pour un match privé
// (EF-bk-004) — mais aucun joueur n'est ajouté à la création (R-ACC-005) : les 3 places
// restantes sont ouvertes à l'inscription individuelle (EF-bk-006, à faire plus tard).
public class CreerMatchPublicRequestDto {
    public string OrganisateurMatricule { get; set; } = null!;
    public int SiteId { get; set; }
    public int TerrainId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly HeureDebut { get; set; }
}

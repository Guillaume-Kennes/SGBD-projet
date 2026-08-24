namespace PadelManager.Models.Dtos;

// Corps de la requête POST de création d'un match privé (EF-bk-004). Terrain et créneau
// proviennent des disponibilités déjà proposées (EF-bk-019), jamais saisis librement.
public class CreerMatchPriveRequestDto {
    public string OrganisateurMatricule { get; set; } = null!;
    public int SiteId { get; set; }
    public int TerrainId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly HeureDebut { get; set; }

    // Jusqu'à 3 joueurs ajoutés par matricule (R-STR-002) ; leur participation reste en attente
    // de leur propre paiement (R-VAL-005).
    public List<string> Joueurs { get; set; } = new();
}

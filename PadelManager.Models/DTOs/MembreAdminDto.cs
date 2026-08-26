namespace PadelManager.Models.Dtos;

// Un membre dans la liste administrateur (EF-bk-017). DetteActive/PenaliteActive réutilisent la
// même logique de blocage qu'à la création d'un match (R-ACC-006 / R-CALC-004).
public class MembreAdminDto {
    public string Matricule { get; set; } = null!;
    public string TypeMembre { get; set; } = null!;
    public int? SiteId { get; set; }
    public bool DetteActive { get; set; }
    public bool PenaliteActive { get; set; }
}

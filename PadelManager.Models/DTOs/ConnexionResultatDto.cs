namespace PadelManager.Models.Dtos;

public class ConnexionResultatDto {
    public string Matricule { get; set; } = null!;
    public string TypeUtilisateur { get; set; } = null!;  // "Membre" ou "Administrateur"
    public string Type { get; set; } = null!;              // GLOBAL, SITE, ou LIBRE (membre) / GLOBAL, SITE (admin)
    public int? SiteId { get; set; }
}


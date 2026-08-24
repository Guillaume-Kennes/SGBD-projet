namespace PadelManager.Models.Dtos;

public class ConnexionResultatDto {
    public string Matricule { get; set; } = null!;
    public string TypeUtilisateur { get; set; } = null!;  // "Membre" ou "Administrateur"
    public string Type { get; set; } = null!;              // GLOBAL, SITE, ou LIBRE (membre) / GLOBAL, SITE (admin)
    public int? SiteId { get; set; }

    // Fenêtre de réservation du membre (TYPE_MEMBRE.anticipationMaxJours), pour que le client
    // puisse l'afficher/la respecter avant même d'appeler la création de match (EF-bk-004).
    // Null pour un administrateur (non applicable).
    public int? AnticipationMaxJours { get; set; }
}


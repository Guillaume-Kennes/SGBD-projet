namespace PadelManager.Models.Dtos;

// Corps de la requête PUT de paramétrage de la fermeture hebdomadaire globale (EF-bk-023).
// Annee vient de la route, pas du body. Liste vide refusée : utiliser DELETE pour repasser
// l'année à "aucun jour fermé".
public class FermetureHebdoGlobaleRequestDto {
    // Matricule de l'admin appelant (issue #13, contrôle de portée serveur) — réservé à un admin
    // GLOBAL, cf. IAdminPorteeService.VerifierAdminGlobalAsync.
    public string AdminMatricule { get; set; } = null!;
    public List<string> JoursFermes { get; set; } = new();
}

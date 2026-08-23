namespace PadelManager.Models.Dtos;

// Corps de la requête PUT de paramétrage de la fermeture hebdomadaire globale (EF-bk-023).
// Annee vient de la route, pas du body. Liste vide refusée : utiliser DELETE pour repasser
// l'année à "aucun jour fermé".
public class FermetureHebdoGlobaleRequestDto {
    public List<string> JoursFermes { get; set; } = new();
}

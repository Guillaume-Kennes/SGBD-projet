namespace PadelManager.Models.Dtos;

// Un joueur inscrit dans le détail d'un match (EF-bk-021), avec le statut de paiement de sa
// participation.
public class JoueurDetailDto {
    public string MembreMatricule { get; set; } = null!;
    public bool Paye { get; set; }
}

namespace PadelManager.Models.Dtos;

// Corps de la requête POST d'inscription à un match public (EF-bk-006). MatchId vient de la
// route, pas du body.
public class RejoindreMatchRequestDto {
    public string MembreMatricule { get; set; } = null!;
}

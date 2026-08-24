namespace PadelManager.Models.Dtos;

// Corps de la requête POST de paiement d'une participation en attente (EF-bk-007, joueur ajouté
// à un match privé). ParticipationId vient de la route, pas du body.
public class PayerParticipationRequestDto {
    public string MembreMatricule { get; set; } = null!;
}

namespace PadelManager.Models.Dtos;

// Résultat d'une vérification de portée admin (issue #13, contrôle de portée serveur pour les
// écrans admin). Autorise indique si l'appelant peut agir/consulter sur le site demandé ;
// MessageErreur explique pourquoi si Autorise == false (renvoyé tel quel au client en 403).
public class PorteeAdminResultatDto {
    public bool Autorise { get; set; }
    public string? MessageErreur { get; set; }
}

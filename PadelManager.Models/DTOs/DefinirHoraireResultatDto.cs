namespace PadelManager.Models.Dtos;

// Résultat du paramétrage annuel : pas d'exception, on remonte une erreur métier lisible
// (site inconnu, jours invalides, conflit avec FERMETURE_HEBDO_GLOBALE, etc.).
public class DefinirHoraireResultatDto {
    public bool Succes { get; set; }
    public string? MessageErreur { get; set; }
    public HoraireSiteDto? Horaire { get; set; }
}

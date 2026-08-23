namespace PadelManager.Models.Dtos;

public class DeclarerFermetureResultatDto {
    public bool Succes { get; set; }
    public string? MessageErreur { get; set; }
    public JourFermetureDto? Fermeture { get; set; }
}

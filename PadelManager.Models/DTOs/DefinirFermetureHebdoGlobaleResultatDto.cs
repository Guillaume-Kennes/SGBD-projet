namespace PadelManager.Models.Dtos;

public class DefinirFermetureHebdoGlobaleResultatDto {
    public bool Succes { get; set; }
    public string? MessageErreur { get; set; }
    public FermetureHebdoGlobaleDto? Fermeture { get; set; }
}

namespace PadelManager.Models.Dtos;

public class FermetureHebdoGlobaleDto {
    public short Annee { get; set; }
    public List<string> JoursFermes { get; set; } = new();
}

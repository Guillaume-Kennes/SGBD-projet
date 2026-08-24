namespace PadelManager.Models.Dtos;

public class InscriptionResultatDto {
    public bool Succes { get; set; }
    public string? MessageErreur { get; set; }
    public decimal? MontantPaye { get; set; }
    public bool DetteReglee { get; set; }
}

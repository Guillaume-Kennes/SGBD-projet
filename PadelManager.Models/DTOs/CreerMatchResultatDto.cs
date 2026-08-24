namespace PadelManager.Models.Dtos;

public class CreerMatchResultatDto {
    public bool Succes { get; set; }
    public string? MessageErreur { get; set; }
    public MatchDto? Match { get; set; }
}

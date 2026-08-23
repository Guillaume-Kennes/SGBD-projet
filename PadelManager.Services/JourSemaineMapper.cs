namespace PadelManager.Services;

// Mapping entre System.DayOfWeek et les codes français à 3 lettres utilisés dans
// HORAIRE_SITE.joursOuverture et FERMETURE_HEBDO_GLOBALE.joursFermes (ex. "LUN,MER,VEN").
public static class JourSemaineMapper {
    public static readonly string[] CodesValides = { "LUN", "MAR", "MER", "JEU", "VEN", "SAM", "DIM" };

    private static readonly Dictionary<DayOfWeek, string> CodesParJour = new() {
        [DayOfWeek.Monday] = "LUN",
        [DayOfWeek.Tuesday] = "MAR",
        [DayOfWeek.Wednesday] = "MER",
        [DayOfWeek.Thursday] = "JEU",
        [DayOfWeek.Friday] = "VEN",
        [DayOfWeek.Saturday] = "SAM",
        [DayOfWeek.Sunday] = "DIM",
    };

    public static string CodePour(DayOfWeek jour) => CodesParJour[jour];

    public static bool EstCodeValide(string code) => CodesValides.Contains(code);
}

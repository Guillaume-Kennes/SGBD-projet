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

    // Découpe une liste CSV de codes jour (ex. "LUN,MER,VEN") en gérant proprement la chaîne
    // vide : "".Split(',') renvoie [""] en C# (un élément, pas zéro), ce qui fausserait le
    // contrat des DTOs pour un HORAIRE_SITE devenu vide (cf. FermetureHebdoGlobaleService, qui
    // peut désormais retirer tous les jours d'ouverture d'un site).
    public static List<string> ParseCsv(string? codes) =>
        string.IsNullOrEmpty(codes) ? new List<string>() : codes.Split(',').ToList();

    // Ordonne une liste de codes jour dans l'ordre canonique (LUN..DIM), indépendamment de
    // l'ordre de saisie — partagé par HoraireSiteService et FermetureHebdoGlobaleService.
    public static List<string> Ordonner(List<string> codes) =>
        CodesValides.Where(codes.Contains).ToList();

    // Valide une liste de codes jour (pas de doublon, tous valides) ; nomSingulier/nomPluriel
    // adaptent le message au contexte ("jour d'ouverture"/"jour fermé") sans dupliquer la
    // logique de validation elle-même — partagé par HoraireSiteService et
    // FermetureHebdoGlobaleService (chacun garde son propre message pour le cas liste vide, qui
    // diffère entre les deux).
    public static string? ValiderListe(List<string> codes, string nomSingulier, string nomPluriel) {
        if (codes.Distinct().Count() != codes.Count)
            return $"Un {nomSingulier} est dupliqué.";

        if (codes.Any(c => !EstCodeValide(c)))
            return $"Un des {nomPluriel} n'est pas valide.";

        return null;
    }
}

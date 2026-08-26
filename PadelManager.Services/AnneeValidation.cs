namespace PadelManager.Services;

// Bornes d'année partagées entre HoraireSiteService et FermetureHebdoGlobaleService (paramétrage
// annuel) — factorisées ici pour éviter de dupliquer les mêmes constantes et le même message.
public static class AnneeValidation {
    private const short AnneeMin = 2000;
    private const short AnneeMax = 2100;

    public static string? Valider(short annee) =>
        annee < AnneeMin || annee > AnneeMax ? $"Année hors bornes ({AnneeMin}-{AnneeMax})." : null;
}

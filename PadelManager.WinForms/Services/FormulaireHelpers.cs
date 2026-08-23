namespace PadelManager.WinForms.Services;

// Logique partagée entre les formulaires (HoraireSiteForm, PlanningForm) pour éviter de
// dupliquer le chargement de la liste des sites dans une ComboBox.
internal static class FormulaireHelpers {
    public static async Task<bool> ChargerSitesAsync(ApiClient apiClient, ComboBox comboSite, Label lblMessage) {
        try {
            var sites = await apiClient.ObtenirSitesAsync();
            if (sites == null) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
                return false;
            }

            comboSite.DataSource = sites;
            comboSite.DisplayMember = nameof(SiteResultat.Nom);
            comboSite.ValueMember = nameof(SiteResultat.Id);
            return true;
        } catch (HttpRequestException) {
            lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            return false;
        }
    }
}

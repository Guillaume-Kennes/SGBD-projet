namespace PadelManager.WinForms.Admin.Services;

// Logique partagée entre les formulaires admin pour éviter de dupliquer le chargement de la
// liste des sites dans une ComboBox.
internal static class FormulaireHelpers {
    public static async Task<bool> ChargerSitesAsync(ApiClient apiClient, ComboBox comboSite, Label lblMessage) {
        try {
            var sites = await apiClient.ObtenirSitesAsync();
            if (sites == null) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
                return false;
            }

            // DisplayMember/ValueMember AVANT DataSource : affecter DataSource déclenche
            // immédiatement SelectedIndexChanged (première ligne sélectionnée par défaut) ; si
            // ValueMember n'est pas encore posé à ce moment-là, SelectedValue renvoie l'objet
            // SiteResultat entier au lieu de l'Id, ce qui casse tout code y écoutant cet événement.
            comboSite.DisplayMember = nameof(SiteResultat.Nom);
            comboSite.ValueMember = nameof(SiteResultat.Id);
            comboSite.DataSource = sites;
            return true;
        } catch (HttpRequestException) {
            lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            return false;
        }
    }
}

using PadelManager.WinForms.Admin.Services;

namespace PadelManager.WinForms.Admin
{
    // Statistiques classiques (EF-bk-016) : matchs publics/privés, taux d'occupation, membres
    // actifs. Même sélecteur de site que les autres écrans admin (JourFermetureForm) : admin
    // Global voit tous les sites (ou filtre sur un site), admin de Site verrouillé sur le sien.
    public partial class StatistiquesForm : Form {

        private readonly ApiClient _apiClient = new();
        private readonly ConnexionResultat _connexion;

        private bool _pretACharger;

        public StatistiquesForm(ConnexionResultat connexion) {
            InitializeComponent();
            _connexion = connexion;
        }

        private async void StatistiquesForm_Load(object sender, EventArgs e) {
            var chargementReussi = await FormulaireHelpers.ChargerSitesAsync(_apiClient, cboSite, lblMessage);

            if (_connexion.Type == "SITE") {
                chkTousLesSites.Enabled = false;
                cboSite.Enabled = false;
                if (chargementReussi && _connexion.SiteId.HasValue)
                    cboSite.SelectedValue = _connexion.SiteId.Value;
            } else {
                chkTousLesSites.Checked = true;
            }

            _pretACharger = true;
            await ChargerAsync();
        }

        private async void chkTousLesSites_CheckedChanged(object sender, EventArgs e) {
            cboSite.Enabled = !chkTousLesSites.Checked && _connexion.Type == "GLOBAL";
            if (_pretACharger)
                await ChargerAsync();
        }

        private async void cboSite_SelectedIndexChanged(object sender, EventArgs e) {
            if (_pretACharger && !chkTousLesSites.Checked)
                await ChargerAsync();
        }

        private async void btnRafraichir_Click(object sender, EventArgs e) {
            await ChargerAsync();
        }

        private async Task ChargerAsync() {
            int? siteId = chkTousLesSites.Checked ? null : (int?)cboSite.SelectedValue;
            if (!chkTousLesSites.Checked && siteId == null)
                return;

            btnRafraichir.Enabled = false;
            lblMessage.Text = "Chargement en cours...";

            try {
                var statistiques = await _apiClient.ObtenirStatistiquesAsync(siteId, _connexion.Matricule);
                if (statistiques == null) {
                    lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
                    grdStatistiques.DataSource = null;
                    return;
                }

                // TauxOccupation est un ratio brut (0.0004 = 0.04%) : reformaté en % lisible pour
                // l'affichage plutôt que de montrer la décimale brute.
                grdStatistiques.DataSource = statistiques
                    .Select(s => new {
                        s.NomSite,
                        s.NombreMatchsPublics,
                        s.NombreMatchsPrives,
                        TauxOccupation = $"{s.TauxOccupation:0.####%}",
                        s.MembresActifs
                    })
                    .ToList();
                lblMessage.Text = "";
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnRafraichir.Enabled = true;
            }
        }
    }
}

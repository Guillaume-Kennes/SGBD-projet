using PadelManager.WinForms.Admin.Services;

namespace PadelManager.WinForms.Admin
{
    // Liste des membres (EF-bk-017) : admin de Site voit uniquement les membres SITE de son site
    // (Global/Libre n'apparaissent jamais dans une vue Site, n'étant rattachés à aucun site) ;
    // admin Global voit tous les membres, tous types. Indicateur dette/pénalité active par membre
    // (même règle de blocage qu'à la création d'un match, R-ACC-006/R-CALC-004).
    public partial class MembresForm : Form {

        private readonly ApiClient _apiClient = new();
        private readonly ConnexionResultat _connexion;

        private bool _pretACharger;

        public MembresForm(ConnexionResultat connexion) {
            InitializeComponent();
            _connexion = connexion;
        }

        private async void MembresForm_Load(object sender, EventArgs e) {
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
                var membres = await _apiClient.ObtenirMembresAsync(siteId, _connexion.Matricule);
                if (membres == null) {
                    lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
                    grdMembres.DataSource = null;
                    return;
                }

                grdMembres.DataSource = membres;
                lblMessage.Text = membres.Count == 0
                    ? "Aucun membre pour ce périmètre."
                    : $"{membres.Count} membre(s).";
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnRafraichir.Enabled = true;
            }
        }
    }
}

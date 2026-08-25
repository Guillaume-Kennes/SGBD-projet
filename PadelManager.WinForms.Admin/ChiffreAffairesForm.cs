using PadelManager.WinForms.Admin.Services;

namespace PadelManager.WinForms.Admin
{
    // Chiffre d'affaires (EF-bk-015, R-CALC-005) : somme de PAIEMENT.montantTotal (part standard +
    // dette reportée éventuelle, puisque les deux correspondent à de l'argent réellement encaissé),
    // rattachée au site du match où le paiement a eu lieu. Même portée que "État des matchs" :
    // admin Global voit tous les sites (ou filtre sur un site), admin de Site est verrouillé sur le
    // sien. Aucun filtre de période (non requis par le CDC) — un total global suffit.
    public partial class ChiffreAffairesForm : Form {

        private readonly ApiClient _apiClient = new();
        private readonly ConnexionResultat _connexion;

        private bool _pretACharger;

        public ChiffreAffairesForm(ConnexionResultat connexion) {
            InitializeComponent();
            _connexion = connexion;
        }

        private async void ChiffreAffairesForm_Load(object sender, EventArgs e) {
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
                var chiffreAffaires = await _apiClient.ObtenirChiffreAffairesAsync(siteId);
                if (chiffreAffaires == null) {
                    lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
                    grdChiffreAffaires.DataSource = null;
                    lblTotal.Text = "";
                    return;
                }

                grdChiffreAffaires.DataSource = chiffreAffaires;
                lblTotal.Text = $"Total : {chiffreAffaires.Sum(c => c.Montant):0.00}€";
                lblMessage.Text = "";
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnRafraichir.Enabled = true;
            }
        }
    }
}

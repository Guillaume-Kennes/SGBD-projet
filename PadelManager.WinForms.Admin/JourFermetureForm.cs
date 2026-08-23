using PadelManager.WinForms.Admin.Services;

namespace PadelManager.WinForms.Admin
{
    // Déclaration / annulation d'une fermeture ponctuelle (EF-bk-024, JOUR_FERMETURE).
    // Un admin de site est verrouillé sur son propre site ; un admin global peut choisir
    // n'importe quel site, ou cocher "tous les sites" pour une fermeture globale
    // (JOUR_FERMETURE.siteId = NULL). L'enregistrement/l'annulation déclenchent automatiquement,
    // côté API, la régénération des disponibilités concernées (EF-bk-022).
    public partial class JourFermetureForm : Form {

        private readonly ApiClient _apiClient = new();
        private readonly ConnexionResultat _connexion;
        private List<JourFermetureResultat> _fermetures = new();

        public JourFermetureForm(ConnexionResultat connexion) {
            InitializeComponent();
            _connexion = connexion;

            dtpDate.Value = DateTime.Today;
        }

        private async void JourFermetureForm_Load(object sender, EventArgs e) {
            var chargementReussi = await FormulaireHelpers.ChargerSitesAsync(_apiClient, cboSite, lblMessage);

            if (_connexion.Type == "SITE") {
                // Un admin de site ne peut déclarer que pour son propre site (EF-bk-024).
                chkTousLesSites.Enabled = false;
                cboSite.Enabled = false;
                if (chargementReussi && _connexion.SiteId.HasValue)
                    cboSite.SelectedValue = _connexion.SiteId.Value;
            }
        }

        private void chkTousLesSites_CheckedChanged(object sender, EventArgs e) {
            cboSite.Enabled = !chkTousLesSites.Checked;
        }

        private async void btnCharger_Click(object sender, EventArgs e) {
            if (cboSite.SelectedValue == null) {
                lblMessage.Text = "Veuillez sélectionner un site.";
                return;
            }

            int siteId = (int)cboSite.SelectedValue;
            int annee = (int)numAnnee.Value;

            lblMessage.Text = "Chargement en cours...";

            try {
                var fermetures = await _apiClient.ObtenirFermeturesPonctuellesAsync(siteId, annee);
                if (fermetures == null) {
                    lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
                    return;
                }

                _fermetures = fermetures.OrderBy(f => f.Date).ToList();
                lstFermetures.Items.Clear();
                foreach (var fermeture in _fermetures)
                    lstFermetures.Items.Add(fermeture.SiteId == null
                        ? $"{fermeture.Date:dd/MM/yyyy} — tous les sites"
                        : $"{fermeture.Date:dd/MM/yyyy} — site {fermeture.SiteId}");

                lblMessage.Text = $"{_fermetures.Count} fermeture(s) chargée(s).";
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            }
        }

        private async void btnDeclarer_Click(object sender, EventArgs e) {
            if (!chkTousLesSites.Checked && cboSite.SelectedValue == null) {
                lblMessage.Text = "Veuillez sélectionner un site.";
                return;
            }

            var requete = new JourFermetureRequete {
                SiteId = chkTousLesSites.Checked ? null : (int)cboSite.SelectedValue!,
                Date = DateOnly.FromDateTime(dtpDate.Value)
            };

            btnDeclarer.Enabled = false;
            lblMessage.Text = "Déclaration en cours...";

            try {
                var resultat = await _apiClient.DeclarerFermetureAsync(requete);

                lblMessage.Text = resultat.Succes
                    ? "Fermeture déclarée et disponibilités régénérées."
                    : $"Erreur : {resultat.Message}";

                if (resultat.Succes)
                    btnCharger_Click(sender, e);
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnDeclarer.Enabled = true;
            }
        }

        private async void btnSupprimer_Click(object sender, EventArgs e) {
            if (lstFermetures.SelectedIndex < 0) {
                lblMessage.Text = "Veuillez sélectionner une fermeture à annuler.";
                return;
            }

            var fermeture = _fermetures[lstFermetures.SelectedIndex];

            // EF-bk-024 : un admin de site n'agit que sur son propre site ; seul l'admin global
            // peut annuler une fermeture globale ("tous les sites", siteId NULL).
            if (fermeture.SiteId == null && _connexion.Type != "GLOBAL") {
                lblMessage.Text = "Seul un administrateur global peut annuler une fermeture globale (tous les sites).";
                return;
            }

            btnSupprimer.Enabled = false;
            lblMessage.Text = "Annulation en cours...";

            try {
                var succes = await _apiClient.SupprimerFermetureAsync(fermeture.Id);

                lblMessage.Text = succes
                    ? "Fermeture annulée et disponibilités régénérées."
                    : "Impossible d'annuler cette fermeture.";

                if (succes)
                    btnCharger_Click(sender, e);
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnSupprimer.Enabled = true;
            }
        }
    }
}

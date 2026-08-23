using PadelManager.WinForms.Admin.Services;

namespace PadelManager.WinForms.Admin
{
    // Paramétrage annuel du site (EF-bk-003) : jours d'ouverture + plage horaire de réservation.
    // L'enregistrement déclenche automatiquement, côté API, la régénération des disponibilités
    // (EF-bk-022). Accessible uniquement après connexion via AdminLoginForm.
    public partial class HoraireSiteForm : Form {

        private readonly ApiClient _apiClient = new();

        private readonly Dictionary<string, CheckBox> _checkBoxParJour;

        public HoraireSiteForm() {
            InitializeComponent();

            _checkBoxParJour = new Dictionary<string, CheckBox> {
                ["LUN"] = chkLun,
                ["MAR"] = chkMar,
                ["MER"] = chkMer,
                ["JEU"] = chkJeu,
                ["VEN"] = chkVen,
                ["SAM"] = chkSam,
                ["DIM"] = chkDim,
            };

            dtpHeureDebut.Value = DateTime.Today.AddHours(9);
            dtpHeureFin.Value = DateTime.Today.AddHours(21);
        }

        private async void HoraireSiteForm_Load(object sender, EventArgs e) {
            await FormulaireHelpers.ChargerSitesAsync(_apiClient, cboSite, lblMessage);
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
                var horaire = await _apiClient.ObtenirHoraireAsync(siteId, annee);
                if (horaire == null) {
                    lblMessage.Text = "Aucun horaire configuré pour ce site et cette année. Vous pouvez en créer un.";
                    return;
                }

                foreach (var (jour, checkBox) in _checkBoxParJour)
                    checkBox.Checked = horaire.JoursOuverture.Contains(jour);

                dtpHeureDebut.Value = DateTime.Today.Add(horaire.HeureDebutReservation.ToTimeSpan());
                dtpHeureFin.Value = DateTime.Today.Add(horaire.HeureFinReservation.ToTimeSpan());

                lblMessage.Text = "Horaire chargé.";
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            }
        }

        private async void btnEnregistrer_Click(object sender, EventArgs e) {
            if (cboSite.SelectedValue == null) {
                lblMessage.Text = "Veuillez sélectionner un site.";
                return;
            }

            int siteId = (int)cboSite.SelectedValue;
            int annee = (int)numAnnee.Value;

            var joursOuverture = _checkBoxParJour
                .Where(paire => paire.Value.Checked)
                .Select(paire => paire.Key)
                .ToList();

            var requete = new HoraireSiteRequete {
                JoursOuverture = joursOuverture,
                HeureDebutReservation = TimeOnly.FromDateTime(dtpHeureDebut.Value),
                HeureFinReservation = TimeOnly.FromDateTime(dtpHeureFin.Value)
            };

            btnEnregistrer.Enabled = false;
            lblMessage.Text = "Enregistrement en cours...";

            try {
                var resultat = await _apiClient.DefinirHoraireAsync(siteId, annee, requete);

                lblMessage.Text = resultat.Succes
                    ? "Horaire enregistré et disponibilités régénérées."
                    : $"Erreur : {resultat.Message}";
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnEnregistrer.Enabled = true;
            }
        }
    }
}

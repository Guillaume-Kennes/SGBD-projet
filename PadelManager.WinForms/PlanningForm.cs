using PadelManager.WinForms.Services;

namespace PadelManager.WinForms
{
    // Consultation du planning d'un site (EF-bk-002) : lecture des disponibilités générées
    // (EF-bk-022) sur une période donnée.
    public partial class PlanningForm : Form {

        private readonly ApiClient _apiClient = new();

        public PlanningForm() {
            InitializeComponent();

            dtpDu.Value = DateTime.Today;
            dtpAu.Value = DateTime.Today.AddDays(14);
        }

        private async void PlanningForm_Load(object sender, EventArgs e) {
            await FormulaireHelpers.ChargerSitesAsync(_apiClient, cboSite, lblMessage);
        }

        private async void btnRechercher_Click(object sender, EventArgs e) {
            if (cboSite.SelectedValue == null) {
                lblMessage.Text = "Veuillez sélectionner un site.";
                return;
            }

            int siteId = (int)cboSite.SelectedValue;
            var from = DateOnly.FromDateTime(dtpDu.Value);
            var to = DateOnly.FromDateTime(dtpAu.Value);

            btnRechercher.Enabled = false;
            lblMessage.Text = "Recherche en cours...";

            try {
                var disponibilites = await _apiClient.ConsulterPlanningAsync(siteId, from, to);
                if (disponibilites == null) {
                    lblMessage.Text = "Site introuvable.";
                    grdPlanning.DataSource = null;
                    return;
                }

                grdPlanning.DataSource = disponibilites;
                lblMessage.Text = disponibilites.Count == 0
                    ? "Aucune disponibilité sur cette période."
                    : $"{disponibilites.Count} créneau(x) trouvé(s).";
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnRechercher.Enabled = true;
            }
        }
    }
}

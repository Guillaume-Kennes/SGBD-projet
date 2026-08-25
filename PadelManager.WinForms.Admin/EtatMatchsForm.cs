using PadelManager.WinForms.Admin.Services;

namespace PadelManager.WinForms.Admin
{
    // État des matchs et terrains (EF-bk-014) : un admin Global consulte tous les sites (ou filtre
    // sur un site précis) ; un admin de Site est verrouillé sur son propre site. Contrairement aux
    // écrans Membre, les IDs (match, terrain) sont affichés pour faciliter le contrôle par l'admin.
    // Statut calculé dynamiquement côté Service (TERMINE tant que non scellé par le job, comme
    // EF-bk-013/021) — cet écran ne fait qu'afficher ce qu'on lui donne.
    public partial class EtatMatchsForm : Form {

        private readonly ApiClient _apiClient = new();
        private readonly ConnexionResultat _connexion;

        // Évite les rechargements en double : positionner chkTousLesSites/cboSite pendant
        // EtatMatchsForm_Load déclenche leurs évènements Changed respectifs avant même que le
        // premier chargement explicite n'ait eu lieu.
        private bool _pretACharger;

        public EtatMatchsForm(ConnexionResultat connexion) {
            InitializeComponent();
            _connexion = connexion;
        }

        private async void EtatMatchsForm_Load(object sender, EventArgs e) {
            var chargementReussi = await FormulaireHelpers.ChargerSitesAsync(_apiClient, cboSite, lblMessage);

            if (_connexion.Type == "SITE") {
                // Un admin de site ne consulte que son propre site (EF-bk-014).
                chkTousLesSites.Enabled = false;
                cboSite.Enabled = false;
                if (chargementReussi && _connexion.SiteId.HasValue)
                    cboSite.SelectedValue = _connexion.SiteId.Value;
            } else {
                // Vue d'ensemble par défaut pour l'admin global.
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
                return; // ComboBox pas encore chargée

            btnRafraichir.Enabled = false;
            lblMessage.Text = "Chargement en cours...";

            try {
                var matchs = await _apiClient.ObtenirEtatMatchsAsync(siteId);
                if (matchs == null) {
                    lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
                    grdMatchs.DataSource = null;
                    lblTerrains.Text = "";
                    return;
                }

                grdMatchs.DataSource = matchs;
                lblMessage.Text = matchs.Count == 0
                    ? "Aucun match pour ce périmètre."
                    : $"{matchs.Count} match(s).";

                var recapTerrains = await _apiClient.ObtenirRecapitulatifTerrainsAsync(siteId);
                lblTerrains.Text = recapTerrains == null ? "" : FormatterRecapTerrains(recapTerrains, siteId == null);
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnRafraichir.Enabled = true;
            }
        }

        // EF-bk-014 ("matchs et terrains") : un seul site -> liste complète ("Terrains : 5 (11, 12,
        // 13, 14, 15)") ; tous les sites -> une plage compressée par site, séparées par " · "
        // ("Site 1 : 5 (11-15) · Site 2 : 6 (21-26)").
        private static string FormatterRecapTerrains(List<TerrainRecapResultat> recap, bool tousLesSites) {
            if (recap.Count == 0)
                return "Terrains : aucun.";

            if (!tousLesSites) {
                var r = recap[0];
                return $"Terrains : {r.TerrainIds.Count} ({string.Join(", ", r.TerrainIds)})";
            }

            return string.Join(" · ", recap.Select(r => $"{r.NomSite} : {r.TerrainIds.Count} ({FormatterPlage(r.TerrainIds)})"));
        }

        private static string FormatterPlage(List<int> numeros) {
            if (numeros.Count == 0)
                return "aucun";

            // Compressé en plage min-max si les numéros sont réellement contigus (toujours le cas
            // dans ce projet, le nombre de terrains par site étant fixe) ; liste complète sinon,
            // en repli défensif.
            var min = numeros.Min();
            var max = numeros.Max();
            return max - min + 1 == numeros.Count ? $"{min}-{max}" : string.Join(", ", numeros);
        }
    }
}

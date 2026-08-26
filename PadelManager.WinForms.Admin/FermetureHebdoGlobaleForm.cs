using PadelManager.WinForms.Admin.Services;

namespace PadelManager.WinForms.Admin
{
    // Paramétrage de la fermeture hebdomadaire globale de l'année (EF-bk-023, FERMETURE_HEBDO_GLOBALE).
    // Réservé à l'administrateur global : accessible uniquement depuis AdminMenuForm quand
    // Type == "GLOBAL". L'enregistrement/la suppression déclenchent automatiquement, côté API,
    // la régénération des disponibilités de tous les sites (EF-bk-022).
    public partial class FermetureHebdoGlobaleForm : Form {

        private readonly ApiClient _apiClient = new();
        private readonly ConnexionResultat _connexion;

        private readonly Dictionary<string, CheckBox> _checkBoxParJour;

        public FermetureHebdoGlobaleForm(ConnexionResultat connexion) {
            InitializeComponent();
            _connexion = connexion;

            _checkBoxParJour = new Dictionary<string, CheckBox> {
                ["LUN"] = chkLun,
                ["MAR"] = chkMar,
                ["MER"] = chkMer,
                ["JEU"] = chkJeu,
                ["VEN"] = chkVen,
                ["SAM"] = chkSam,
                ["DIM"] = chkDim,
            };
        }

        private async void btnCharger_Click(object sender, EventArgs e) {
            int annee = (int)numAnnee.Value;

            lblMessage.Text = "Chargement en cours...";

            foreach (var checkBox in _checkBoxParJour.Values)
                checkBox.Checked = false;

            try {
                var fermeture = await _apiClient.ObtenirFermetureHebdoGlobaleAsync(annee, _connexion.Matricule);
                if (fermeture == null) {
                    lblMessage.Text = "Aucun jour fermé globalement pour cette année.";
                    return;
                }

                foreach (var (jour, checkBox) in _checkBoxParJour)
                    checkBox.Checked = fermeture.JoursFermes.Contains(jour);

                lblMessage.Text = "Fermeture hebdomadaire globale chargée.";
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            }
        }

        private async void btnEnregistrer_Click(object sender, EventArgs e) {
            int annee = (int)numAnnee.Value;

            var joursFermes = _checkBoxParJour
                .Where(paire => paire.Value.Checked)
                .Select(paire => paire.Key)
                .ToList();

            if (joursFermes.Count == 0) {
                lblMessage.Text = "Sélectionnez au moins un jour, ou utilisez \"Supprimer\" pour n'en fermer aucun.";
                return;
            }

            var requete = new FermetureHebdoGlobaleRequete { AdminMatricule = _connexion.Matricule, JoursFermes = joursFermes };

            btnEnregistrer.Enabled = false;
            lblMessage.Text = "Enregistrement en cours...";

            try {
                var resultat = await _apiClient.DefinirFermetureHebdoGlobaleAsync(annee, requete);

                lblMessage.Text = resultat.Succes
                    ? "Fermeture hebdomadaire globale enregistrée et disponibilités régénérées."
                    : $"Erreur : {resultat.Message}";
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnEnregistrer.Enabled = true;
            }
        }

        private async void btnSupprimer_Click(object sender, EventArgs e) {
            int annee = (int)numAnnee.Value;

            if (MessageBox.Show(
                    $"Repasser l'année {annee} à \"aucun jour fermé globalement\" ?",
                    "Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes) {
                return;
            }

            btnSupprimer.Enabled = false;
            lblMessage.Text = "Suppression en cours...";

            try {
                var succes = await _apiClient.SupprimerFermetureHebdoGlobaleAsync(annee, _connexion.Matricule);

                if (succes) {
                    foreach (var checkBox in _checkBoxParJour.Values)
                        checkBox.Checked = false;
                    lblMessage.Text = "Fermeture hebdomadaire globale supprimée et disponibilités régénérées.";
                } else {
                    lblMessage.Text = "Aucune fermeture hebdomadaire globale à supprimer pour cette année.";
                }
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnSupprimer.Enabled = true;
            }
        }
    }
}

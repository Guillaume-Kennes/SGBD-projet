using PadelManager.WinForms.Admin.Services;

namespace PadelManager.WinForms.Admin
{
    // Point d'entrée de l'application Admin. Même principe que LoginForm côté Membre (saisie du
    // matricule, vérification via /api/auth/connexion), mais rejette tout matricule qui n'est
    // pas de type "Administrateur". Ouvre HoraireSiteForm une fois la connexion validée.
    public partial class AdminLoginForm : Form {

        private readonly ApiClient _apiClient = new();

        public AdminLoginForm() {
            InitializeComponent();
        }

        private async void btnConnexion_Click(object sender, EventArgs e) {
            string matricule = txtMatricule.Text.Trim();

            if (string.IsNullOrWhiteSpace(matricule)) {
                lblMessage.Text = "Veuillez saisir un matricule.";
                return;
            }

            btnConnexion.Enabled = false;
            lblMessage.Text = "Connexion en cours...";

            try {
                var resultat = await _apiClient.ConnexionAsync(matricule);

                if (resultat == null) {
                    lblMessage.Text = "Matricule inconnu.";
                    return;
                }

                if (resultat.TypeUtilisateur != "Administrateur") {
                    lblMessage.Text = "Accès réservé aux administrateurs.";
                    return;
                }

                Hide();
                using var horaireSiteForm = new HoraireSiteForm();
                horaireSiteForm.ShowDialog();
                Close();
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                // HoraireSiteForm peut avoir fermé/disposé ce formulaire entre-temps (connexion
                // réussie) : dans ce cas il n'y a plus de bouton à réactiver.
                if (!IsDisposed)
                    btnConnexion.Enabled = true;
            }
        }
    }
}

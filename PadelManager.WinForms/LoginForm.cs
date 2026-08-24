using PadelManager.WinForms.Services;

namespace PadelManager.WinForms
{
    public partial class LoginForm : Form {

        private readonly ApiClient _apiClient = new();

        public LoginForm() {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e) {

        }

        private void label1_Click(object sender, EventArgs e) {

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

                if (resultat.TypeUtilisateur != "Membre") {
                    lblMessage.Text = "Accès réservé aux membres.";
                    return;
                }

                Hide();
                using var menuForm = new MembreMenuForm(resultat);
                menuForm.ShowDialog();
                Close();
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                // MembreMenuForm peut avoir fermé/disposé ce formulaire entre-temps (connexion
                // réussie) : dans ce cas il n'y a plus de bouton à réactiver.
                if (!IsDisposed)
                    btnConnexion.Enabled = true;
            }
        }
    }
}






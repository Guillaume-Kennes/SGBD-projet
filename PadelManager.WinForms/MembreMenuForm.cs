using PadelManager.WinForms.Services;

namespace PadelManager.WinForms
{
    // Hub ouvert après connexion réussie (LoginForm) : donne accès aux différents écrans
    // Membre, chacun ouvert en modal.
    public partial class MembreMenuForm : Form {

        private readonly ConnexionResultat _connexion;

        public MembreMenuForm(ConnexionResultat connexion) {
            InitializeComponent();
            _connexion = connexion;

            lblConnecte.Text = $"Connecté : {_connexion.Matricule} ({_connexion.Type})";
        }

        private void btnCreerMatch_Click(object sender, EventArgs e) {
            using var form = new CreerMatchForm(_connexion);
            form.ShowDialog();
        }
    }
}

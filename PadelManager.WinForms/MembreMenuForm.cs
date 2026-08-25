using PadelManager.WinForms.Services;

namespace PadelManager.WinForms
{
    // Hub ouvert après connexion réussie (LoginForm) : donne accès aux différents écrans
    // Membre, chacun ouvert en modal.
    public partial class MembreMenuForm : Form {

        private readonly ConnexionResultat _connexion;

        // Distingue une fermeture volontaire ("Se déconnecter") d'une fermeture de fenêtre : le
        // LoginForm appelant s'en sert pour savoir s'il doit se réafficher (nouvelle connexion)
        // ou fermer l'application (comportement historique, avant l'ajout du bouton).
        public bool Deconnexion { get; private set; }

        public MembreMenuForm(ConnexionResultat connexion) {
            InitializeComponent();
            _connexion = connexion;

            lblConnecte.Text = $"Connecté : {_connexion.Matricule} ({_connexion.Type})";
        }

        private void btnCreerMatch_Click(object sender, EventArgs e) {
            using var form = new CreerMatchForm(_connexion);
            form.ShowDialog();
        }

        private void btnCreerMatchPublic_Click(object sender, EventArgs e) {
            using var form = new CreerMatchPublicForm(_connexion);
            form.ShowDialog();
        }

        private void btnMatchsPublics_Click(object sender, EventArgs e) {
            using var form = new MatchsPublicsForm(_connexion);
            form.ShowDialog();
        }

        private void btnParticipationsEnAttente_Click(object sender, EventArgs e) {
            using var form = new ParticipationsEnAttenteForm(_connexion);
            form.ShowDialog();
        }

        private void btnReservations_Click(object sender, EventArgs e) {
            using var form = new ReservationsForm(_connexion);
            form.ShowDialog();
        }

        private void btnDeconnexion_Click(object sender, EventArgs e) {
            Deconnexion = true;
            Close();
        }
    }
}

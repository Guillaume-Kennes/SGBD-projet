using PadelManager.WinForms.Admin.Services;

namespace PadelManager.WinForms.Admin
{
    // Hub ouvert après connexion réussie (AdminLoginForm) : donne accès aux différents écrans de
    // paramétrage, chacun ouvert en modal. "Fermeture hebdomadaire globale" est réservé à
    // l'administrateur global (EF-bk-023) — restriction appliquée ici côté UI, l'API elle-même
    // ne vérifiant pas l'identité de l'appelant (comme le reste de ce projet).
    public partial class AdminMenuForm : Form {

        private readonly ConnexionResultat _connexion;

        public AdminMenuForm(ConnexionResultat connexion) {
            InitializeComponent();
            _connexion = connexion;

            lblConnecte.Text = $"Connecté : {_connexion.Matricule} ({_connexion.Type})";
            btnFermetureHebdoGlobale.Enabled = _connexion.Type == "GLOBAL";
        }

        private void btnHoraires_Click(object sender, EventArgs e) {
            using var form = new HoraireSiteForm(_connexion);
            form.ShowDialog();
        }

        private void btnFermeturesPonctuelles_Click(object sender, EventArgs e) {
            using var form = new JourFermetureForm(_connexion);
            form.ShowDialog();
        }

        private void btnFermetureHebdoGlobale_Click(object sender, EventArgs e) {
            using var form = new FermetureHebdoGlobaleForm();
            form.ShowDialog();
        }
    }
}

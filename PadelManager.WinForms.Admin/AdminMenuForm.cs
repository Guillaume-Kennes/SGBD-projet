using PadelManager.WinForms.Admin.Services;

namespace PadelManager.WinForms.Admin
{
    // Hub ouvert après connexion réussie (AdminLoginForm) : donne accès aux différents écrans de
    // paramétrage, chacun ouvert en modal. "Fermeture hebdomadaire globale" est réservé à
    // l'administrateur global (EF-bk-023) — restriction appliquée ici côté UI, l'API elle-même
    // ne vérifiant pas l'identité de l'appelant (comme le reste de ce projet).
    public partial class AdminMenuForm : Form {

        private readonly ConnexionResultat _connexion;

        // Distingue une fermeture volontaire ("Se déconnecter") d'une fermeture de fenêtre :
        // l'AdminLoginForm appelant s'en sert pour savoir s'il doit se réafficher (nouvelle
        // connexion) ou fermer l'application (comportement historique, avant ce bouton).
        public bool Deconnexion { get; private set; }

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
            using var form = new FermetureHebdoGlobaleForm(_connexion);
            form.ShowDialog();
        }

        private void btnEtatMatchs_Click(object sender, EventArgs e) {
            using var form = new EtatMatchsForm(_connexion);
            form.ShowDialog();
        }

        private void btnChiffreAffaires_Click(object sender, EventArgs e) {
            using var form = new ChiffreAffairesForm(_connexion);
            form.ShowDialog();
        }

        private void btnStatistiques_Click(object sender, EventArgs e) {
            using var form = new StatistiquesForm(_connexion);
            form.ShowDialog();
        }

        private void btnMembres_Click(object sender, EventArgs e) {
            using var form = new MembresForm(_connexion);
            form.ShowDialog();
        }

        private void btnDeconnexion_Click(object sender, EventArgs e) {
            Deconnexion = true;
            Close();
        }
    }
}

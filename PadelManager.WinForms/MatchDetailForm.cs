using PadelManager.WinForms.Services;

namespace PadelManager.WinForms
{
    // Détail d'un match (EF-bk-021) : site, terrain, date/heure, visibilité, statut, et la liste
    // des joueurs inscrits avec leur statut de paiement. Ouvert soit depuis "Mes réservations",
    // soit depuis "Matchs publics" (même écran, réutilisé tel quel — EF-bk-012 gère déjà côté
    // serveur qui a le droit de consulter quel match).
    public partial class MatchDetailForm : Form {

        private readonly ApiClient _apiClient = new();
        private readonly ConnexionResultat _connexion;
        private readonly int _matchId;

        public MatchDetailForm(ConnexionResultat connexion, int matchId) {
            InitializeComponent();
            _connexion = connexion;
            _matchId = matchId;
        }

        private async void MatchDetailForm_Load(object sender, EventArgs e) {
            await ChargerAsync();
        }

        private async Task ChargerAsync() {
            lblMessage.Text = "Chargement en cours...";
            lblInfo.Text = "";
            grdJoueurs.DataSource = null;

            try {
                var detail = await _apiClient.ObtenirDetailMatchAsync(_matchId, _connexion.Matricule);
                if (detail == null) {
                    lblMessage.Text = "Ce match est introuvable, ou vous n'êtes pas autorisé à le consulter.";
                    return;
                }

                lblMessage.Text = "";
                lblInfo.Text =
                    $"Site : {detail.NomSite}\n" +
                    $"Terrain : {detail.NumeroTerrain}\n" +
                    $"Date / heure : {detail.DateHeure:dd/MM/yyyy HH:mm}\n" +
                    $"Visibilité : {detail.Visibilite}\n" +
                    $"Statut : {detail.Statut}\n" +
                    $"Organisateur : {detail.OrganisateurMatricule}";

                grdJoueurs.DataSource = detail.Joueurs
                    .Select(j => new { j.MembreMatricule, Statut = j.Paye ? "Payé" : "En attente" })
                    .ToList();
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            }
        }
    }
}

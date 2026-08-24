using PadelManager.WinForms.Services;

namespace PadelManager.WinForms
{
    // Consultation des matchs publics (EF-bk-005) et inscription + paiement immédiat d'une
    // place libre (EF-bk-006/007). Le filtrage par portée/délai (site pour un membre Site, 5
    // jours pour un membre Libre, aucun pour un membre Global) est fait côté serveur
    // (MatchService.ObtenirMatchsPublicsAsync) : cet écran ne fait qu'afficher ce qu'on lui donne.
    public partial class MatchsPublicsForm : Form {

        private readonly ApiClient _apiClient = new();
        private readonly ConnexionResultat _connexion;
        private List<MatchPublicResultat> _matchs = new();

        public MatchsPublicsForm(ConnexionResultat connexion) {
            InitializeComponent();
            _connexion = connexion;
        }

        private async void MatchsPublicsForm_Load(object sender, EventArgs e) {
            await ChargerAsync();
        }

        private async void btnRafraichir_Click(object sender, EventArgs e) {
            await ChargerAsync();
        }

        private async Task ChargerAsync() {
            btnRafraichir.Enabled = false;
            lblMessage.Text = "Chargement en cours...";

            try {
                var matchs = await _apiClient.ObtenirMatchsPublicsAsync(_connexion.Matricule);
                if (matchs == null) {
                    lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
                    grdMatchs.DataSource = null;
                    return;
                }

                _matchs = matchs;
                grdMatchs.DataSource = _matchs;
                lblMessage.Text = _matchs.Count == 0
                    ? "Aucun match public disponible pour l'instant."
                    : $"{_matchs.Count} match(s) public(s) disponible(s).";
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnRafraichir.Enabled = true;
            }
        }

        private async void btnRejoindre_Click(object sender, EventArgs e) {
            if (grdMatchs.CurrentRow?.DataBoundItem is not MatchPublicResultat match) {
                lblMessage.Text = "Veuillez sélectionner un match dans la liste.";
                return;
            }

            btnRejoindre.Enabled = false;
            lblMessage.Text = "Inscription et paiement en cours...";

            try {
                var resultat = await _apiClient.RejoindreMatchPublicAsync(match.Id, _connexion.Matricule);

                if (resultat.Succes) {
                    // Rafraîchit la liste (la place n'est plus libre, ou le match a disparu s'il
                    // est complet) AVANT le message de confirmation, sinon ChargerAsync l'écrase.
                    await ChargerAsync();

                    var detteInfo = resultat.Data?.DetteReglee == true ? " (dette existante réglée par la même occasion)" : "";
                    lblMessage.Text = $"Inscription confirmée, {resultat.Data?.MontantPaye:0.00}€ payés{detteInfo}.";
                } else {
                    lblMessage.Text = $"Erreur : {resultat.Message}";
                }
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnRejoindre.Enabled = true;
            }
        }
    }
}

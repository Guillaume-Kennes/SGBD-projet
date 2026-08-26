using PadelManager.WinForms.Services;

namespace PadelManager.WinForms
{
    // Consultation des matchs publics (EF-bk-005) et inscription + paiement immédiat d'une
    // place libre (EF-bk-006/007). Le filtrage par portée (site pour un membre Site, aucun pour
    // un membre Libre ou Global — R-VAL-003, CDC v0.11 : le délai ne borne que la création,
    // jamais la consultation/l'inscription) est fait côté serveur
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

                await ChargerMontantAPayerAsync();
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnRafraichir.Enabled = true;
            }
        }

        // Montant affiché à côté du bouton "Rejoindre et payer 15€" : toujours recalculé côté
        // serveur à chaque chargement (jamais mis en cache depuis ConnexionResultat), une dette
        // pouvant être réglée entre-temps par une autre action. Une dette active ne bloque pas
        // l'inscription (R-ACC-006 ne bloque que la création), elle s'ajoute simplement au
        // montant à payer. Rien n'est affiché sans dette : le bouton annonce déjà les 15€, un
        // label répétant "15,00€" à côté n'apporterait rien.
        private async Task ChargerMontantAPayerAsync() {
            var montant = await _apiClient.ObtenirMontantAPayerAsync(_connexion.Matricule);
            lblMontant.Text = FormatterDette(montant);
        }

        internal static string FormatterDette(MontantAPayerResultat? montant) {
            if (montant?.MontantDette is not > 0)
                return "";

            return $"+ {montant.MontantDette:0.00}€ de dette à payer en plus (total : {montant.MontantTotal:0.00}€)";
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

        // EF-bk-021 : détail consultable même sans y avoir encore participé, tant que le match
        // reste dans le périmètre du membre — déjà garanti ici puisque cette liste elle-même est
        // filtrée par portée/délai côté serveur (EF-bk-005).
        private void btnVoirDetail_Click(object sender, EventArgs e) {
            if (grdMatchs.CurrentRow?.DataBoundItem is not MatchPublicResultat match) {
                lblMessage.Text = "Veuillez sélectionner un match dans la liste.";
                return;
            }

            using var form = new MatchDetailForm(_connexion, match.Id);
            form.ShowDialog();
        }
    }
}

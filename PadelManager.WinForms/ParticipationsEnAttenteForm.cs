using PadelManager.WinForms.Services;

namespace PadelManager.WinForms
{
    // Matchs privés où le membre connecté a été ajouté par l'organisateur : sa place n'est
    // confirmée qu'une fois payée (EF-bk-007). Même principe d'affichage que MatchsPublicsForm
    // (montant recalculé côté serveur à chaque chargement, dette active ajoutée le cas échéant).
    public partial class ParticipationsEnAttenteForm : Form {

        private readonly ApiClient _apiClient = new();
        private readonly ConnexionResultat _connexion;
        private List<ParticipationEnAttenteResultat> _participations = new();

        public ParticipationsEnAttenteForm(ConnexionResultat connexion) {
            InitializeComponent();
            _connexion = connexion;
        }

        private async void ParticipationsEnAttenteForm_Load(object sender, EventArgs e) {
            await ChargerAsync();
        }

        private async void btnRafraichir_Click(object sender, EventArgs e) {
            await ChargerAsync();
        }

        private async Task ChargerAsync() {
            btnRafraichir.Enabled = false;
            lblMessage.Text = "Chargement en cours...";

            try {
                var participations = await _apiClient.ObtenirParticipationsEnAttenteAsync(_connexion.Matricule);
                if (participations == null) {
                    lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
                    grdParticipations.DataSource = null;
                    return;
                }

                _participations = participations;
                grdParticipations.DataSource = _participations;
                lblMessage.Text = _participations.Count == 0
                    ? "Aucune participation en attente de paiement."
                    : $"{_participations.Count} participation(s) en attente de paiement.";

                var montant = await _apiClient.ObtenirMontantAPayerAsync(_connexion.Matricule);
                lblMontant.Text = MatchsPublicsForm.FormatterDette(montant);
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnRafraichir.Enabled = true;
            }
        }

        private async void btnPayer_Click(object sender, EventArgs e) {
            if (grdParticipations.CurrentRow?.DataBoundItem is not ParticipationEnAttenteResultat participation) {
                lblMessage.Text = "Veuillez sélectionner une participation dans la liste.";
                return;
            }

            btnPayer.Enabled = false;
            lblMessage.Text = "Paiement en cours...";

            try {
                var resultat = await _apiClient.PayerParticipationAsync(participation.ParticipationId, _connexion.Matricule);

                if (resultat.Succes) {
                    // Rafraîchit la liste (la participation payée disparaît) AVANT le message de
                    // confirmation, sinon ChargerAsync l'écrase.
                    await ChargerAsync();

                    var detteInfo = resultat.Data?.DetteReglee == true ? " (dette existante réglée par la même occasion)" : "";
                    lblMessage.Text = $"Paiement confirmé, {resultat.Data?.MontantPaye:0.00}€ payés{detteInfo}.";
                } else {
                    lblMessage.Text = $"Erreur : {resultat.Message}";
                }
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnPayer.Enabled = true;
            }
        }
    }
}

using PadelManager.WinForms.Services;

namespace PadelManager.WinForms
{
    // Consultation des réservations du membre connecté (EF-bk-013) : tous les matchs où il est
    // organisateur ou participant, passés ou à venir, quel que soit le statut ou la visibilité.
    // Le détail d'un match sélectionné s'ouvre via MatchDetailForm (EF-bk-021).
    public partial class ReservationsForm : Form {

        private readonly ApiClient _apiClient = new();
        private readonly ConnexionResultat _connexion;
        private List<ReservationResultat> _reservations = new();

        public ReservationsForm(ConnexionResultat connexion) {
            InitializeComponent();
            _connexion = connexion;
        }

        private async void ReservationsForm_Load(object sender, EventArgs e) {
            await ChargerAsync();
        }

        private async void btnRafraichir_Click(object sender, EventArgs e) {
            await ChargerAsync();
        }

        private async Task ChargerAsync() {
            btnRafraichir.Enabled = false;
            lblMessage.Text = "Chargement en cours...";

            try {
                var reservations = await _apiClient.ObtenirReservationsAsync(_connexion.Matricule);
                if (reservations == null) {
                    lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
                    grdReservations.DataSource = null;
                    return;
                }

                _reservations = reservations;
                grdReservations.DataSource = _reservations;
                lblMessage.Text = _reservations.Count == 0
                    ? "Aucune réservation pour l'instant."
                    : $"{_reservations.Count} réservation(s).";
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnRafraichir.Enabled = true;
            }
        }

        private void btnVoirDetail_Click(object sender, EventArgs e) {
            if (grdReservations.CurrentRow?.DataBoundItem is not ReservationResultat reservation) {
                lblMessage.Text = "Veuillez sélectionner une réservation dans la liste.";
                return;
            }

            using var form = new MatchDetailForm(_connexion, reservation.Id);
            form.ShowDialog();
        }
    }
}

using PadelManager.WinForms.Services;

namespace PadelManager.WinForms
{
    // Création d'un match privé (EF-bk-004) : recherche des créneaux réellement libres par
    // terrain (site + date), ajout de jusqu'à 3 joueurs par matricule, création + paiement
    // immédiat de la part de l'organisateur, en une seule opération (R-VAL-005).
    public partial class CreerMatchForm : Form {

        private readonly ApiClient _apiClient = new();
        private readonly ConnexionResultat _connexion;
        private List<CreneauMatchResultat> _creneaux = new();

        // Horaire + fermetures ponctuelles du site sélectionné, mis en cache par (siteId, année)
        // pour prévenir immédiatement un jour fermé sans attendre une recherche infructueuse.
        private int? _horaireSiteId;
        private int? _horaireAnnee;
        private HashSet<string> _joursOuverture = new();
        private HashSet<DateOnly> _joursFermes = new();

        private static readonly string[] CodesJourParDayOfWeek = { "DIM", "LUN", "MAR", "MER", "JEU", "VEN", "SAM" }; // index = (int)DayOfWeek

        public CreerMatchForm(ConnexionResultat connexion) {
            InitializeComponent();
            _connexion = connexion;

            dtpDate.Value = DateTime.Today;
            dtpDate.MinDate = DateTime.Today;

            // Fenêtre de réservation du membre (R-ACC-001/002/003) : on l'affiche et on empêche
            // de sélectionner une date hors fenêtre plutôt que de laisser l'utilisateur découvrir
            // le refus après une recherche infructueuse.
            if (_connexion.AnticipationMaxJours.HasValue) {
                dtpDate.MaxDate = DateTime.Today.AddDays(_connexion.AnticipationMaxJours.Value);
                lblFenetre.Text = $"Réservation possible jusqu'à {_connexion.AnticipationMaxJours} jour(s) à l'avance ({_connexion.Type}).";
            }
        }

        private async void CreerMatchForm_Load(object sender, EventArgs e) {
            var chargementReussi = await FormulaireHelpers.ChargerSitesAsync(_apiClient, cboSite, lblMessage);

            if (_connexion.Type == "SITE") {
                // Un membre de site ne peut organiser un match que sur son propre site (R-ACC-002).
                cboSite.Enabled = false;
                if (chargementReussi && _connexion.SiteId.HasValue)
                    cboSite.SelectedValue = _connexion.SiteId.Value;
            }

            await RafraichirEtatDateAsync();
        }

        private async void cboSite_SelectedIndexChanged(object sender, EventArgs e) {
            await RafraichirEtatDateAsync();
        }

        private async void dtpDate_ValueChanged(object sender, EventArgs e) {
            await RafraichirEtatDateAsync();
        }

        // Charge (si besoin) l'horaire et les fermetures ponctuelles du site pour l'année de la
        // date sélectionnée, puis prévient immédiatement si le jour choisi est fermé (jour de
        // semaine non ouvert, ou fermeture ponctuelle site/globale), plutôt que de laisser
        // l'utilisateur découvrir une liste vide après une recherche. WinForms ne permet pas de
        // désactiver des dates précises dans un DateTimePicker : on bloque donc la recherche et
        // on explique pourquoi, au lieu de retirer visuellement le jour du calendrier.
        private async Task RafraichirEtatDateAsync() {
            grdCreneaux.DataSource = null;
            _creneaux = new List<CreneauMatchResultat>();

            if (cboSite.SelectedValue == null)
                return;

            int siteId = (int)cboSite.SelectedValue;
            int annee = dtpDate.Value.Year;

            if (_horaireSiteId != siteId || _horaireAnnee != annee) {
                try {
                    var horaire = await _apiClient.ObtenirHoraireAsync(siteId, annee);
                    var fermetures = await _apiClient.ObtenirFermeturesPonctuellesAsync(siteId, annee);

                    _joursOuverture = horaire != null ? horaire.JoursOuverture.ToHashSet() : new HashSet<string>();
                    _joursFermes = fermetures?.Select(f => f.Date).ToHashSet() ?? new HashSet<DateOnly>();
                    _horaireSiteId = siteId;
                    _horaireAnnee = annee;
                } catch (HttpRequestException) {
                    // Pas bloquant : la recherche réelle fera foi si ce pré-contrôle échoue.
                    return;
                }
            }

            var date = DateOnly.FromDateTime(dtpDate.Value);
            var codeJour = CodesJourParDayOfWeek[(int)dtpDate.Value.DayOfWeek];

            if (_joursFermes.Contains(date)) {
                lblMessage.Text = "Ce site est fermé ce jour-là (fermeture ponctuelle).";
                btnRechercher.Enabled = false;
            } else if (!_joursOuverture.Contains(codeJour)) {
                lblMessage.Text = $"Ce site est fermé le {LibelleJour(dtpDate.Value.DayOfWeek)}.";
                btnRechercher.Enabled = false;
            } else {
                lblMessage.Text = string.Empty;
                btnRechercher.Enabled = true;
            }
        }

        private static string LibelleJour(DayOfWeek jour) => jour switch {
            DayOfWeek.Monday => "lundi",
            DayOfWeek.Tuesday => "mardi",
            DayOfWeek.Wednesday => "mercredi",
            DayOfWeek.Thursday => "jeudi",
            DayOfWeek.Friday => "vendredi",
            DayOfWeek.Saturday => "samedi",
            _ => "dimanche"
        };

        private async void btnRechercher_Click(object sender, EventArgs e) {
            await RechercherAsync();
        }

        private async Task RechercherAsync() {
            if (cboSite.SelectedValue == null) {
                lblMessage.Text = "Veuillez sélectionner un site.";
                return;
            }

            int siteId = (int)cboSite.SelectedValue;
            var date = DateOnly.FromDateTime(dtpDate.Value);

            btnRechercher.Enabled = false;
            lblMessage.Text = "Recherche en cours...";

            try {
                var creneaux = await _apiClient.ObtenirCreneauxDisponiblesAsync(siteId, date);
                if (creneaux == null) {
                    lblMessage.Text = "Site introuvable.";
                    grdCreneaux.DataSource = null;
                    return;
                }

                _creneaux = creneaux;
                grdCreneaux.DataSource = _creneaux;
                lblMessage.Text = _creneaux.Count == 0
                    ? "Aucun créneau disponible pour cette date."
                    : $"{_creneaux.Count} créneau(x) disponible(s).";
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnRechercher.Enabled = true;
            }
        }

        private async void btnCreer_Click(object sender, EventArgs e) {
            if (cboSite.SelectedValue == null) {
                lblMessage.Text = "Veuillez sélectionner un site.";
                return;
            }

            if (grdCreneaux.CurrentRow?.DataBoundItem is not CreneauMatchResultat creneau) {
                lblMessage.Text = "Veuillez sélectionner un créneau dans la liste.";
                return;
            }

            var joueurs = new[] { txtJoueur1.Text, txtJoueur2.Text, txtJoueur3.Text }
                .Select(m => m.Trim())
                .Where(m => m.Length > 0)
                .ToList();

            var requete = new CreerMatchPriveRequete {
                OrganisateurMatricule = _connexion.Matricule,
                SiteId = (int)cboSite.SelectedValue,
                TerrainId = creneau.TerrainId,
                Date = DateOnly.FromDateTime(dtpDate.Value),
                HeureDebut = creneau.HeureDebut,
                Joueurs = joueurs
            };

            btnCreer.Enabled = false;
            lblMessage.Text = "Création et paiement en cours...";

            try {
                var resultat = await _apiClient.CreerMatchPriveAsync(requete);

                if (resultat.Succes) {
                    txtJoueur1.Clear();
                    txtJoueur2.Clear();
                    txtJoueur3.Clear();
                    // Rafraîchit la liste (le créneau n'est plus libre) AVANT le message de
                    // confirmation, sinon RechercherAsync l'écrase aussitôt (bug précédent).
                    await RechercherAsync();
                    lblMessage.Text = $"Match créé (terrain {creneau.NumeroTerrain}, {requete.Date:dd/MM/yyyy} {requete.HeureDebut}) et votre part de 15€ payée.";
                } else {
                    lblMessage.Text = $"Erreur : {resultat.Message}";
                }
            } catch (HttpRequestException) {
                lblMessage.Text = "Impossible de contacter le serveur. Vérifiez que l'API est lancée.";
            } finally {
                btnCreer.Enabled = true;
            }
        }
    }
}

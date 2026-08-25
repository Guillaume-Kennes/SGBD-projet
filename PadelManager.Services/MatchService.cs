using PadelManager.Interfaces;
using PadelManager.Models;
using PadelManager.Models.Dtos;

namespace PadelManager.Services;

public class MatchService : IMatchService {
    private const decimal MontantParticipation = 15.00m;

    // R-STR-002 : un match compte toujours exactement 4 participants (organisateur inclus) —
    // ni moins (place vide non prévue par le CDC pour un match privé), ni plus.
    private const int NombreJoueursAjoutesRequis = 3;

    private readonly ISiteRepository _siteRepository;
    private readonly ITerrainRepository _terrainRepository;
    private readonly IMembreRepository _membreRepository;
    private readonly IDetteRepository _detteRepository;
    private readonly IPenaliteRepository _penaliteRepository;
    private readonly IDisponibiliteRepository _disponibiliteRepository;
    private readonly IMatchRepository _matchRepository;

    public MatchService(
        ISiteRepository siteRepository,
        ITerrainRepository terrainRepository,
        IMembreRepository membreRepository,
        IDetteRepository detteRepository,
        IPenaliteRepository penaliteRepository,
        IDisponibiliteRepository disponibiliteRepository,
        IMatchRepository matchRepository) {
        _siteRepository = siteRepository;
        _terrainRepository = terrainRepository;
        _membreRepository = membreRepository;
        _detteRepository = detteRepository;
        _penaliteRepository = penaliteRepository;
        _disponibiliteRepository = disponibiliteRepository;
        _matchRepository = matchRepository;
    }

    public async Task<List<CreneauMatchDto>?> ObtenirCreneauxDisponiblesAsync(int siteId, DateOnly date) {
        if (await _siteRepository.GetByIdAsync(siteId) == null)
            return null;

        var disponibilites = await _disponibiliteRepository.GetBySiteAndPeriodeAsync(siteId, date, date);
        if (disponibilites.Count == 0)
            return new List<CreneauMatchDto>();

        var terrains = await _terrainRepository.GetBySiteIdAsync(siteId);
        var matchs = await _matchRepository.GetForSiteAndDateAsync(siteId, date);
        var creneauxPris = matchs.Select(m => (m.TerrainId, m.DateHeure)).ToHashSet();

        var creneaux = new List<CreneauMatchDto>();
        foreach (var disponibilite in disponibilites) {
            var dateHeure = date.ToDateTime(disponibilite.HeureDebut);
            foreach (var terrain in terrains) {
                if (creneauxPris.Contains((terrain.Id, dateHeure)))
                    continue;

                creneaux.Add(new CreneauMatchDto {
                    TerrainId = terrain.Id,
                    NumeroTerrain = terrain.Numero,
                    HeureDebut = disponibilite.HeureDebut,
                    HeureFin = disponibilite.HeureFin
                });
            }
        }

        return creneaux
            .OrderBy(c => c.HeureDebut)
            .ThenBy(c => c.NumeroTerrain)
            .ToList();
    }

    public async Task<CreerMatchResultatDto> CreerMatchPriveAsync(CreerMatchPriveRequestDto requete) {
        var (organisateur, dateHeure, erreur) = await ValiderCreationAsync(
            requete.OrganisateurMatricule, requete.SiteId, requete.TerrainId, requete.Date, requete.HeureDebut);
        if (erreur != null)
            return Echec(erreur);

        var erreurJoueurs = await ValiderJoueursAsync(requete.Joueurs, organisateur!.Matricule);
        if (erreurJoueurs != null)
            return Echec(erreurJoueurs);

        var match = ConstruireMatch(organisateur, requete.SiteId, requete.TerrainId, dateHeure, "PRIVE");

        // Les joueurs ajoutés par l'organisateur restent en attente de leur propre paiement
        // (R-VAL-005).
        var maintenant = DateTime.Now;
        foreach (var joueur in requete.Joueurs) {
            match.Participations.Add(new Participation {
                MembreMatricule = joueur,
                DateInscription = maintenant
            });
        }

        return await EnregistrerAsync(match);
    }

    public async Task<CreerMatchResultatDto> CreerMatchPublicAsync(CreerMatchPublicRequestDto requete) {
        var (organisateur, dateHeure, erreur) = await ValiderCreationAsync(
            requete.OrganisateurMatricule, requete.SiteId, requete.TerrainId, requete.Date, requete.HeureDebut);
        if (erreur != null)
            return Echec(erreur);

        // R-ACC-005 : aucun joueur ajouté à la création d'un match public ; les 3 places
        // restantes sont ouvertes à l'inscription individuelle (EF-bk-006).
        var match = ConstruireMatch(organisateur!, requete.SiteId, requete.TerrainId, dateHeure, "PUBLIC");

        return await EnregistrerAsync(match);
    }

    public async Task<List<MatchPublicDto>?> ObtenirMatchsPublicsAsync(string membreMatricule) {
        var membre = await _membreRepository.GetByMatriculeAsync(membreMatricule);
        if (membre == null)
            return null;

        var matchs = await _matchRepository.GetPublicsIncompletsAsync(DateTime.Now);

        // Un membre déjà inscrit (y compris comme organisateur) n'a plus de place à prendre sur
        // ce match ; tenter de le rejoindre échouerait de toute façon (DejaInscritException).
        IEnumerable<Match> visibles = matchs.Where(m => m.Participations.All(p => p.MembreMatricule != membreMatricule));

        // R-ACC-002 : un membre de site ne voit que les matchs publics de son site. Global et
        // Libre voient tous les sites, sans aucune restriction de délai (R-VAL-003 : l'anticipation
        // maximum par type de membre ne borne que la création d'un match, jamais la consultation).
        if (membre.TypeMembre == "SITE") {
            visibles = visibles.Where(m => m.SiteId == membre.SiteId);
        }

        return visibles
            .OrderBy(m => m.DateHeure)
            .Select(VersMatchPublicDto)
            .ToList();
    }

    public async Task<InscriptionResultatDto> RejoindreMatchPublicAsync(int matchId, string membreMatricule) {
        var membre = await _membreRepository.GetByMatriculeAsync(membreMatricule);
        if (membre == null)
            return EchecInscription("Membre introuvable.");

        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            return EchecInscription("Match introuvable.");

        if (match.Visibilite != "PUBLIC")
            return EchecInscription("Ce match n'est pas public.");

        if (match.DateHeure <= DateTime.Now)
            return EchecInscription("Ce match a déjà commencé.");

        // R-ACC-002 : un membre de site ne peut rejoindre que les matchs publics de son site.
        // Aucune restriction de délai pour Libre (R-VAL-003 : l'anticipation maximum par type de
        // membre ne borne que la création d'un match, jamais l'inscription à une place libre).
        if (membre.TypeMembre == "SITE" && membre.SiteId != match.SiteId)
            return EchecInscription("Un membre de site ne peut rejoindre un match public que sur son site de rattachement.");

        // R-ACC-006 : contrairement à la création, une dette non soldée ne bloque pas
        // l'inscription — elle est au contraire automatiquement réglée par ce paiement
        // (EF-bk-018). Idem pour une éventuelle pénalité (R-CALC-004) : elle ne bloque que la
        // création d'un nouveau match, jamais le fait de rejoindre un match existant.
        var dette = await _detteRepository.GetNonSoldeeAsync(membre.Matricule);

        try {
            await _matchRepository.InscrireEtPayerAsync(matchId, membre.Matricule, dette);
        } catch (MatchCompletException) {
            return EchecInscription("Ce match est déjà complet ; veuillez réessayer ou choisir un autre match.");
        } catch (DejaInscritException) {
            return EchecInscription("Vous êtes déjà inscrit à ce match.");
        }

        return new InscriptionResultatDto {
            Succes = true,
            MontantPaye = MontantParticipation + (dette?.Montant ?? 0.00m),
            DetteReglee = dette != null
        };
    }

    public async Task<MontantAPayerDto> ObtenirMontantAPayerAsync(string membreMatricule) {
        var dette = await _detteRepository.GetNonSoldeeAsync(membreMatricule);
        return new MontantAPayerDto {
            MontantParticipation = MontantParticipation,
            MontantDette = dette?.Montant,
            MontantTotal = MontantParticipation + (dette?.Montant ?? 0.00m)
        };
    }

    public async Task<InscriptionResultatDto> PayerParticipationAsync(int participationId, string membreMatricule) {
        var membre = await _membreRepository.GetByMatriculeAsync(membreMatricule);
        if (membre == null)
            return EchecInscription("Membre introuvable.");

        var participation = await _matchRepository.GetParticipationByIdAsync(participationId);
        if (participation == null)
            return EchecInscription("Participation introuvable.");

        if (participation.MembreMatricule != membre.Matricule)
            return EchecInscription("Vous ne pouvez payer que votre propre participation.");

        if (participation.Paiement != null)
            return EchecInscription("Cette participation est déjà payée.");

        // R-ACC-006 / R-CALC-004 : ni la dette ni la pénalité ne bloquent le paiement d'une
        // participation déjà existante — elles ne bloquent que la création d'une nouvelle
        // réservation. Une dette active est au contraire automatiquement réglée (EF-bk-018),
        // exactement comme pour l'inscription à un match public. Pas de vérification "match déjà
        // commencé" non plus : une place non payée reste payable jusqu'au job de bascule
        // (EF-bk-009), qui seul la libère. Idem pour l'anticipation maximum par type de membre
        // (R-VAL-003) : elle ne borne que la création, jamais le paiement d'une place existante —
        // aucune vérification de délai ici, ni de portée site (déjà membre du match).
        var dette = await _detteRepository.GetNonSoldeeAsync(membre.Matricule);

        try {
            await _matchRepository.PayerParticipationAsync(participation, dette);
        } catch (ParticipationDejaPayeeException) {
            return EchecInscription("Cette participation est déjà payée.");
        }

        return new InscriptionResultatDto {
            Succes = true,
            MontantPaye = MontantParticipation + (dette?.Montant ?? 0.00m),
            DetteReglee = dette != null
        };
    }

    public async Task<List<ParticipationEnAttenteDto>?> ObtenirParticipationsEnAttenteAsync(string membreMatricule) {
        var membre = await _membreRepository.GetByMatriculeAsync(membreMatricule);
        if (membre == null)
            return null;

        var participations = await _matchRepository.GetParticipationsEnAttenteAsync(membreMatricule);

        return participations
            .OrderBy(p => p.Match.DateHeure)
            .Select(p => new ParticipationEnAttenteDto {
                ParticipationId = p.Id,
                MatchId = p.MatchId,
                SiteId = p.Match.SiteId,
                NomSite = p.Match.Site.Nom,
                TerrainId = p.Match.TerrainId,
                NumeroTerrain = p.Match.Terrain.Numero,
                DateHeure = p.Match.DateHeure,
                OrganisateurMatricule = p.Match.OrganisateurMatricule
            })
            .ToList();
    }

    public async Task<List<ReservationDto>?> ObtenirReservationsAsync(string membreMatricule) {
        var membre = await _membreRepository.GetByMatriculeAsync(membreMatricule);
        if (membre == null)
            return null;

        var matchs = await _matchRepository.GetReservationsAsync(membreMatricule);

        // Les plus récentes/imminentes en premier (mélange volontaire de passé et de futur,
        // EF-bk-013 ne prescrit aucun tri) : la date la plus proche du présent, dans un sens ou
        // l'autre, est en général ce qui intéresse le plus le membre en premier lieu.
        return matchs
            .OrderByDescending(m => m.DateHeure)
            .Select(m => new ReservationDto {
                Id = m.Id,
                SiteId = m.SiteId,
                NomSite = m.Site.Nom,
                TerrainId = m.TerrainId,
                NumeroTerrain = m.Terrain.Numero,
                DateHeure = m.DateHeure,
                Visibilite = m.Visibilite,
                Statut = CalculerStatutAffiche(m),
                EstOrganisateur = m.OrganisateurMatricule == membreMatricule
            })
            .ToList();
    }

    public async Task<MatchDetailDto?> ObtenirDetailAsync(int matchId, string membreMatricule) {
        var membre = await _membreRepository.GetByMatriculeAsync(membreMatricule);
        if (membre == null)
            return null;

        var match = await _matchRepository.GetDetailAsync(matchId);
        if (match == null || !PeutConsulter(match, membre))
            return null;

        return new MatchDetailDto {
            Id = match.Id,
            SiteId = match.SiteId,
            NomSite = match.Site.Nom,
            TerrainId = match.TerrainId,
            NumeroTerrain = match.Terrain.Numero,
            DateHeure = match.DateHeure,
            Visibilite = match.Visibilite,
            Statut = CalculerStatutAffiche(match),
            OrganisateurMatricule = match.OrganisateurMatricule,
            Joueurs = match.Participations
                .Select(p => new JoueurDetailDto { MembreMatricule = p.MembreMatricule, Paye = p.Paiement != null })
                .ToList()
        };
    }

    public async Task<List<AdminMatchDto>> ObtenirEtatMatchsAsync(int? siteId) {
        var matchs = await _matchRepository.GetTousLesMatchsAsync(siteId);

        return matchs
            .OrderBy(m => m.DateHeure)
            .Select(m => new AdminMatchDto {
                Id = m.Id,
                SiteId = m.SiteId,
                NomSite = m.Site.Nom,
                TerrainId = m.TerrainId,
                NumeroTerrain = m.Terrain.Numero,
                DateHeure = m.DateHeure,
                Visibilite = m.Visibilite,
                Statut = CalculerStatutAffiche(m)
            })
            .ToList();
    }

    public async Task<List<TerrainRecapDto>> ObtenirRecapitulatifTerrainsAsync(int? siteId) {
        List<Site> sites;
        if (siteId.HasValue) {
            var site = await _siteRepository.GetByIdAsync(siteId.Value);
            sites = site != null ? new List<Site> { site } : new List<Site>();
        } else {
            sites = await _siteRepository.GetAllAsync();
        }

        var recap = new List<TerrainRecapDto>();
        foreach (var site in sites) {
            var terrains = await _terrainRepository.GetBySiteIdAsync(site.Id);
            recap.Add(new TerrainRecapDto {
                SiteId = site.Id,
                NomSite = site.Nom,
                TerrainIds = terrains.Select(t => t.Id).OrderBy(id => id).ToList()
            });
        }

        return recap.OrderBy(r => r.SiteId).ToList();
    }

    // "Statut TERMINE d'un match" (calcul hybride, CDC) : tant que le job de clôture quotidien
    // (padel_job, EF-bk-008/issue #10) n'a pas encore scellé le match en base, son statut réel
    // reste INCOMPLET/COMPLET même après l'heure du match — rien ne le met à jour en temps réel.
    // On l'affiche donc calculé à la lecture dès que l'heure courante dépasse la fin du créneau
    // (dateHeure + 1h30, R-STR-004), sans jamais écrire cette valeur en base ; un match déjà
    // scellé TERMINE reste tel quel. Uniquement pour l'affichage (EF-bk-013/021/014) — la
    // création d'un match n'est pas concernée.
    private static string CalculerStatutAffiche(Match match) {
        if (match.Statut == "TERMINE")
            return "TERMINE";

        return DateTime.Now > match.DateHeure.AddMinutes(90) ? "TERMINE" : match.Statut;
    }

    // EF-bk-021 : consultable si organisateur/participant, quel que soit le site ou la visibilité
    // (un joueur invité à un match privé sur un autre site — R-ACC-005 — doit pouvoir en voir le
    // détail) ; sinon, uniquement si le match est public et dans le périmètre du membre (EF-bk-012,
    // même règle de portée site que pour rejoindre — R-ACC-002). Aucune vérification de délai
    // (R-VAL-003 : l'anticipation maximum par type de membre ne borne que la création d'un match,
    // jamais la consultation).
    private static bool PeutConsulter(Match match, Membre membre) {
        var estImplique = match.OrganisateurMatricule == membre.Matricule
            || match.Participations.Any(p => p.MembreMatricule == membre.Matricule);
        if (estImplique)
            return true;

        if (match.Visibilite != "PUBLIC")
            return false;

        return membre.TypeMembre != "SITE" || membre.SiteId == match.SiteId;
    }

    private static InscriptionResultatDto EchecInscription(string message) => new() { Succes = false, MessageErreur = message };

    private static MatchPublicDto VersMatchPublicDto(Match match) => new() {
        Id = match.Id,
        SiteId = match.SiteId,
        NomSite = match.Site.Nom,
        TerrainId = match.TerrainId,
        NumeroTerrain = match.Terrain.Numero,
        DateHeure = match.DateHeure,
        PlacesRestantes = 4 - match.Participations.Count
    };

    // Validations communes à la création d'un match privé (EF-bk-004) et public (EF-bk-002) :
    // seul l'ajout de joueurs diffère entre les deux (R-ACC-005).
    private async Task<(Membre? Organisateur, DateTime DateHeure, string? Erreur)> ValiderCreationAsync(
            string organisateurMatricule, int siteId, int terrainId, DateOnly date, TimeOnly heureDebut) {
        var organisateur = await _membreRepository.GetByMatriculeAsync(organisateurMatricule);
        if (organisateur == null)
            return (null, default, "Organisateur introuvable.");

        if (await _siteRepository.GetByIdAsync(siteId) == null)
            return (null, default, "Site introuvable.");

        var terrain = await _terrainRepository.GetByIdAsync(terrainId);
        if (terrain == null || terrain.SiteId != siteId)
            return (null, default, "Terrain introuvable pour ce site.");

        // R-ACC-002 / EF-bk-012 : un membre de site ne peut organiser que sur son propre site.
        if (organisateur.TypeMembre == "SITE" && organisateur.SiteId != siteId)
            return (null, default, "Un membre de site ne peut organiser un match que sur son site de rattachement.");

        // R-ACC-006 : solde impayé -> aucune nouvelle réservation.
        if (await _detteRepository.ExisteDetteNonSoldeeAsync(organisateur.Matricule))
            return (null, default, "Vous avez un solde impayé : réglez votre dette avant de créer une nouvelle réservation.");

        var aujourdHui = DateOnly.FromDateTime(DateTime.Today);

        // R-CALC-004 : pénalité active -> blocage total jusqu'à la date, aucune fenêtre réduite.
        var penalite = await _penaliteRepository.GetPlusRecenteAsync(organisateur.Matricule);
        if (penalite != null && penalite.DelaiJusquAu > aujourdHui)
            return (null, default, $"Vous êtes pénalisé suite à un match resté incomplet : aucune nouvelle réservation possible avant le {penalite.DelaiJusquAu:dd/MM/yyyy}.");

        // R-VAL-001, R-ACC-001/002/003 : fenêtre de réservation selon le type de membre,
        // ouverte jusqu'au jour même (anticipationMaxJours lu en base, jamais en dur).
        var anticipationMaxJours = organisateur.TypeMembreNavigation.AnticipationMaxJours;
        var ecartJours = date.DayNumber - aujourdHui.DayNumber;
        if (ecartJours < 0 || ecartJours > anticipationMaxJours)
            return (null, default, $"La date du match doit être comprise entre aujourd'hui et {anticipationMaxJours} jour(s) à l'avance pour votre type de membre.");

        // EF-bk-020 : le créneau doit provenir des disponibilités déjà générées.
        if (!await _disponibiliteRepository.ExisteAsync(siteId, date, heureDebut))
            return (null, default, "Ce créneau n'est pas disponible.");

        var dateHeure = date.ToDateTime(heureDebut);

        // EF-bk-019 : revérification explicite, juste avant l'enregistrement, qu'aucun autre
        // match n'a été créé entre-temps sur ce même terrain pour ce même créneau.
        if (await _matchRepository.ExisteAsync(terrainId, dateHeure))
            return (null, default, "Ce terrain est déjà réservé pour ce créneau ; veuillez choisir un autre créneau.");

        return (organisateur, dateHeure, null);
    }

    // Match + participation/paiement de l'organisateur (R-VAL-005, R-CALC-002 : part standard
    // 15€), commun aux deux visibilités.
    private static Match ConstruireMatch(Membre organisateur, int siteId, int terrainId, DateTime dateHeure, string visibilite) {
        var match = new Match {
            SiteId = siteId,
            TerrainId = terrainId,
            DateHeure = dateHeure,
            Visibilite = visibilite,
            OrganisateurMatricule = organisateur.Matricule,
            Statut = "INCOMPLET"
        };

        var maintenant = DateTime.Now;
        match.Participations.Add(new Participation {
            MembreMatricule = organisateur.Matricule,
            DateInscription = maintenant,
            Paiement = new Paiement {
                MontantParticipation = MontantParticipation,
                MontantDetteReportee = 0.00m,
                DatePaiement = maintenant
            }
        });

        return match;
    }

    private async Task<CreerMatchResultatDto> EnregistrerAsync(Match match) {
        try {
            await _matchRepository.AddAsync(match);
        } catch (CreneauIndisponibleException) {
            return Echec("Ce terrain vient d'être réservé pour ce créneau ; veuillez choisir un autre créneau.");
        }

        return new CreerMatchResultatDto { Succes = true, Match = VersDto(match) };
    }

    private async Task<string?> ValiderJoueursAsync(List<string> joueurs, string organisateurMatricule) {
        if (joueurs.Count != NombreJoueursAjoutesRequis)
            return $"Un match privé doit compter exactement {NombreJoueursAjoutesRequis} joueurs ajoutés par l'organisateur (reçu : {joueurs.Count}).";

        if (joueurs.Distinct().Count() != joueurs.Count)
            return "Un joueur est ajouté plusieurs fois.";

        if (joueurs.Contains(organisateurMatricule))
            return "L'organisateur ne peut pas s'ajouter lui-même comme joueur.";

        foreach (var joueur in joueurs) {
            if (await _membreRepository.GetByMatriculeAsync(joueur) == null)
                return $"Le joueur {joueur} est introuvable.";
        }

        return null;
    }

    private static CreerMatchResultatDto Echec(string message) => new() { Succes = false, MessageErreur = message };

    private static MatchDto VersDto(Match match) => new() {
        Id = match.Id,
        SiteId = match.SiteId,
        TerrainId = match.TerrainId,
        DateHeure = match.DateHeure,
        Visibilite = match.Visibilite,
        OrganisateurMatricule = match.OrganisateurMatricule,
        Statut = match.Statut,
        Joueurs = match.Participations.Select(p => p.MembreMatricule).ToList()
    };
}

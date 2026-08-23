using PadelManager.Interfaces;
using PadelManager.Models;

namespace PadelManager.Services;

public class DisponibiliteGenerationService : IDisponibiliteGenerationService {
    private const int DureeMatchMinutes = 90;
    private const int PauseMinutes = 15;

    private readonly IHoraireSiteRepository _horaireSiteRepository;
    private readonly IJourFermetureRepository _jourFermetureRepository;
    private readonly IFermetureHebdoGlobaleRepository _fermetureHebdoGlobaleRepository;
    private readonly IDisponibiliteRepository _disponibiliteRepository;

    public DisponibiliteGenerationService(
        IHoraireSiteRepository horaireSiteRepository,
        IJourFermetureRepository jourFermetureRepository,
        IFermetureHebdoGlobaleRepository fermetureHebdoGlobaleRepository,
        IDisponibiliteRepository disponibiliteRepository) {
        _horaireSiteRepository = horaireSiteRepository;
        _jourFermetureRepository = jourFermetureRepository;
        _fermetureHebdoGlobaleRepository = fermetureHebdoGlobaleRepository;
        _disponibiliteRepository = disponibiliteRepository;
    }

    public async Task<int?> GenererPourSiteEtAnneeAsync(int siteId, short annee) {
        var horaire = await _horaireSiteRepository.GetBySiteAndAnneeAsync(siteId, annee);
        if (horaire == null)
            return null;

        var joursFermeture = await _jourFermetureRepository.GetForSiteAndAnneeAsync(siteId, annee);
        var datesFermees = joursFermeture.Select(j => j.Date).ToHashSet();
        var joursOuverture = horaire.JoursOuverture.Split(',').ToHashSet();

        // Filet de sécurité R-STR-006 : HoraireSiteService vérifie déjà, au moment du
        // paramétrage, qu'aucun jour d'ouverture ne coïncide avec une fermeture hebdomadaire
        // globale. Mais FERMETURE_HEBDO_GLOBALE peut être ajoutée après coup pour cette année
        // (son CRUD n'est pas géré par cette issue) ; on l'exclut donc aussi ici pour que la
        // (ré)génération manuelle reste toujours cohérente avec la règle.
        var fermetureGlobale = await _fermetureHebdoGlobaleRepository.GetByAnneeAsync(annee);
        if (fermetureGlobale != null)
            joursOuverture.ExceptWith(fermetureGlobale.JoursFermes.Split(','));

        var disponibilites = new List<Disponibilite>();

        for (var date = new DateOnly(annee, 1, 1); date.Year == annee; date = date.AddDays(1)) {
            if (datesFermees.Contains(date))
                continue;

            if (!joursOuverture.Contains(JourSemaineMapper.CodePour(date.DayOfWeek)))
                continue;

            disponibilites.AddRange(GenererCreneauxDuJour(siteId, date, horaire.HeureDebutReservation, horaire.HeureFinReservation));
        }

        await _disponibiliteRepository.RemplacerPourSiteEtAnneeAsync(siteId, annee, disponibilites);

        return disponibilites.Count;
    }

    // Créneaux d'1h30 de match espacés de 15 min de battement, tant que le créneau tient
    // entre l'heure de début et l'heure de fin de réservation (cf. horaires réels par site).
    private static IEnumerable<Disponibilite> GenererCreneauxDuJour(int siteId, DateOnly date, TimeOnly heureDebut, TimeOnly heureFin) {
        var debutCreneau = heureDebut;

        while (true) {
            var finCreneau = debutCreneau.AddMinutes(DureeMatchMinutes, out int joursDecalesFin);
            if (joursDecalesFin != 0 || finCreneau > heureFin)
                yield break;

            yield return new Disponibilite {
                SiteId = siteId,
                Date = date,
                HeureDebut = debutCreneau,
                HeureFin = finCreneau
            };

            debutCreneau = finCreneau.AddMinutes(PauseMinutes, out int joursDecalesSuivant);
            if (joursDecalesSuivant != 0)
                yield break;
        }
    }
}

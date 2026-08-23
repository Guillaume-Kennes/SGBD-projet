using PadelManager.Interfaces;
using PadelManager.Models;
using PadelManager.Models.Dtos;

namespace PadelManager.Services;

public class HoraireSiteService : IHoraireSiteService {
    private const short AnneeMin = 2000;
    private const short AnneeMax = 2100;

    private readonly ISiteRepository _siteRepository;
    private readonly IHoraireSiteRepository _horaireSiteRepository;
    private readonly IFermetureHebdoGlobaleRepository _fermetureHebdoGlobaleRepository;
    private readonly IDisponibiliteGenerationService _disponibiliteGenerationService;

    public HoraireSiteService(
        ISiteRepository siteRepository,
        IHoraireSiteRepository horaireSiteRepository,
        IFermetureHebdoGlobaleRepository fermetureHebdoGlobaleRepository,
        IDisponibiliteGenerationService disponibiliteGenerationService) {
        _siteRepository = siteRepository;
        _horaireSiteRepository = horaireSiteRepository;
        _fermetureHebdoGlobaleRepository = fermetureHebdoGlobaleRepository;
        _disponibiliteGenerationService = disponibiliteGenerationService;
    }

    public async Task<HoraireSiteDto?> ObtenirHoraireAsync(int siteId, short annee) {
        var horaire = await _horaireSiteRepository.GetBySiteAndAnneeAsync(siteId, annee);
        return horaire == null ? null : VersDto(horaire);
    }

    public async Task<DefinirHoraireResultatDto> DefinirHoraireAsync(int siteId, short annee, HoraireSiteRequestDto requete) {
        var erreur = await ValiderAsync(siteId, annee, requete);
        if (erreur != null) {
            return new DefinirHoraireResultatDto { Succes = false, MessageErreur = erreur };
        }

        var joursOrdonnes = JourSemaineMapper.CodesValides
            .Where(c => requete.JoursOuverture.Contains(c))
            .ToList();

        var horaire = new HoraireSite {
            SiteId = siteId,
            Annee = annee,
            JoursOuverture = string.Join(",", joursOrdonnes),
            HeureDebutReservation = requete.HeureDebutReservation,
            HeureFinReservation = requete.HeureFinReservation
        };

        await _horaireSiteRepository.UpsertAsync(horaire);
        await _disponibiliteGenerationService.GenererPourSiteEtAnneeAsync(siteId, annee);

        return new DefinirHoraireResultatDto {
            Succes = true,
            Horaire = new HoraireSiteDto {
                SiteId = siteId,
                Annee = annee,
                JoursOuverture = joursOrdonnes,
                HeureDebutReservation = requete.HeureDebutReservation,
                HeureFinReservation = requete.HeureFinReservation
            }
        };
    }

    private async Task<string?> ValiderAsync(int siteId, short annee, HoraireSiteRequestDto requete) {
        if (await _siteRepository.GetByIdAsync(siteId) == null)
            return "Site introuvable.";

        if (annee < AnneeMin || annee > AnneeMax)
            return $"Année hors bornes ({AnneeMin}-{AnneeMax}).";

        if (requete.JoursOuverture == null || requete.JoursOuverture.Count == 0)
            return "Veuillez sélectionner au moins un jour d'ouverture.";

        if (requete.JoursOuverture.Distinct().Count() != requete.JoursOuverture.Count)
            return "Un jour d'ouverture est dupliqué.";

        if (requete.JoursOuverture.Any(c => !JourSemaineMapper.EstCodeValide(c)))
            return "Un des jours d'ouverture n'est pas valide.";

        if (requete.HeureDebutReservation >= requete.HeureFinReservation)
            return "L'heure de début doit précéder l'heure de fin.";

        // R-STR-006 : un jour d'ouverture ne peut jamais coïncider avec une fermeture
        // hebdomadaire globale de la même année.
        var fermetureGlobale = await _fermetureHebdoGlobaleRepository.GetByAnneeAsync(annee);
        if (fermetureGlobale != null) {
            var joursFermes = fermetureGlobale.JoursFermes.Split(',');
            var conflit = requete.JoursOuverture.FirstOrDefault(joursFermes.Contains);
            if (conflit != null)
                return $"Le jour {conflit} est fermé globalement pour l'année {annee}.";
        }

        return null;
    }

    private static HoraireSiteDto VersDto(HoraireSite horaire) => new() {
        SiteId = horaire.SiteId,
        Annee = horaire.Annee,
        JoursOuverture = horaire.JoursOuverture.Split(',').ToList(),
        HeureDebutReservation = horaire.HeureDebutReservation,
        HeureFinReservation = horaire.HeureFinReservation
    };
}

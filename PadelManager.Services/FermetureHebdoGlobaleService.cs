using PadelManager.Interfaces;
using PadelManager.Models;
using PadelManager.Models.Dtos;

namespace PadelManager.Services;

public class FermetureHebdoGlobaleService : IFermetureHebdoGlobaleService {
    private readonly IHoraireSiteRepository _horaireSiteRepository;
    private readonly IFermetureHebdoGlobaleRepository _fermetureHebdoGlobaleRepository;
    private readonly IDisponibiliteGenerationService _disponibiliteGenerationService;

    public FermetureHebdoGlobaleService(
        IHoraireSiteRepository horaireSiteRepository,
        IFermetureHebdoGlobaleRepository fermetureHebdoGlobaleRepository,
        IDisponibiliteGenerationService disponibiliteGenerationService) {
        _horaireSiteRepository = horaireSiteRepository;
        _fermetureHebdoGlobaleRepository = fermetureHebdoGlobaleRepository;
        _disponibiliteGenerationService = disponibiliteGenerationService;
    }

    public async Task<FermetureHebdoGlobaleDto?> ObtenirAsync(short annee) {
        var fermeture = await _fermetureHebdoGlobaleRepository.GetByAnneeAsync(annee);
        return fermeture == null ? null : VersDto(fermeture);
    }

    public async Task<DefinirFermetureHebdoGlobaleResultatDto> DefinirAsync(short annee, FermetureHebdoGlobaleRequestDto requete) {
        var erreur = Valider(annee, requete);
        if (erreur != null) {
            return new DefinirFermetureHebdoGlobaleResultatDto { Succes = false, MessageErreur = erreur };
        }

        var joursOrdonnes = JourSemaineMapper.Ordonner(requete.JoursFermes);

        await _fermetureHebdoGlobaleRepository.UpsertAsync(new FermetureHebdoGlobale {
            Annee = annee,
            JoursFermes = string.Join(",", joursOrdonnes)
        });

        // R-STR-006 : la fermeture hebdomadaire globale prime sur le paramétrage local (CDC).
        // Contrairement au sens inverse (HoraireSiteService rejette un jour d'ouverture déjà
        // fermé globalement), ici on ne rejette pas : on retire automatiquement le(s) jour(s)
        // désormais fermé(s) globalement de tous les HORAIRE_SITE de l'année concernés, pour que
        // joursOuverture reste le reflet fidèle de ce qui est réellement réservable.
        var sitesModifies = await RetirerJoursFermesDesHorairesAsync(annee, joursOrdonnes);

        // Seuls les sites dont l'horaire vient réellement de changer ont un DISPONIBILITE à
        // régénérer (un site jamais ouvert un jour désormais fermé produit exactement les mêmes
        // créneaux qu'avant) : on évite ainsi de recalculer et réécrire l'année complète de tous
        // les sites à chaque fermeture hebdo globale, ce qui peut être coûteux en base.
        foreach (var siteId in sitesModifies)
            await _disponibiliteGenerationService.GenererPourSiteEtAnneeAsync(siteId, annee);

        return new DefinirFermetureHebdoGlobaleResultatDto {
            Succes = true,
            Fermeture = new FermetureHebdoGlobaleDto { Annee = annee, JoursFermes = joursOrdonnes }
        };
    }

    public async Task<bool> SupprimerAsync(short annee) {
        if (await _fermetureHebdoGlobaleRepository.GetByAnneeAsync(annee) == null)
            return false;

        // Note : les jours retirés des HORAIRE_SITE par un DefinirAsync antérieur ne sont pas
        // restaurés automatiquement (aucune trace de ce qui a été retiré où) ; un site qui
        // souhaite rouvrir ce jour doit reparamétrer son horaire (EF-bk-003). Par conséquent,
        // supprimer la fermeture hebdo globale ne modifie aucun HORAIRE_SITE : aucun site n'a de
        // DISPONIBILITE à régénérer ici.
        await _fermetureHebdoGlobaleRepository.DeleteAsync(annee);

        return true;
    }

    // Retire, pour chaque HORAIRE_SITE de l'année, les jours désormais fermés globalement.
    // Retourne les siteId réellement modifiés (pour ne régénérer que ceux-là).
    private async Task<List<int>> RetirerJoursFermesDesHorairesAsync(short annee, List<string> joursFermes) {
        var horaires = await _horaireSiteRepository.GetAllForAnneeAsync(annee);
        var sitesModifies = new List<int>();

        foreach (var horaire in horaires) {
            var joursOuverture = JourSemaineMapper.ParseCsv(horaire.JoursOuverture);
            var joursRestants = joursOuverture.Where(j => !joursFermes.Contains(j)).ToList();

            if (joursRestants.Count == joursOuverture.Count)
                continue; // aucun jour de ce site n'est concerné

            horaire.JoursOuverture = string.Join(",", joursRestants);
            await _horaireSiteRepository.UpsertAsync(horaire);
            sitesModifies.Add(horaire.SiteId);
        }

        return sitesModifies;
    }

    private static string? Valider(short annee, FermetureHebdoGlobaleRequestDto requete) {
        var erreurAnnee = AnneeValidation.Valider(annee);
        if (erreurAnnee != null)
            return erreurAnnee;

        if (requete.JoursFermes == null || requete.JoursFermes.Count == 0)
            return "Veuillez sélectionner au moins un jour fermé (ou utiliser la suppression pour n'en fermer aucun).";

        return JourSemaineMapper.ValiderListe(requete.JoursFermes, "jour fermé", "jours fermés");
    }

    private static FermetureHebdoGlobaleDto VersDto(FermetureHebdoGlobale fermeture) => new() {
        Annee = fermeture.Annee,
        JoursFermes = fermeture.JoursFermes.Split(',').ToList()
    };
}

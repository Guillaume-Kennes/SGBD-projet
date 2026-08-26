using PadelManager.Interfaces;
using PadelManager.Models;
using PadelManager.Models.Dtos;

namespace PadelManager.Services;

public class JourFermetureService : IJourFermetureService {
    private readonly ISiteRepository _siteRepository;
    private readonly IJourFermetureRepository _jourFermetureRepository;
    private readonly IDisponibiliteGenerationService _disponibiliteGenerationService;

    public JourFermetureService(
        ISiteRepository siteRepository,
        IJourFermetureRepository jourFermetureRepository,
        IDisponibiliteGenerationService disponibiliteGenerationService) {
        _siteRepository = siteRepository;
        _jourFermetureRepository = jourFermetureRepository;
        _disponibiliteGenerationService = disponibiliteGenerationService;
    }

    public async Task<List<JourFermetureDto>> ObtenirPourSiteEtAnneeAsync(int siteId, short annee) {
        var fermetures = await _jourFermetureRepository.GetForSiteAndAnneeAsync(siteId, annee);
        return fermetures.Select(VersDto).ToList();
    }

    public async Task<JourFermetureDto?> ObtenirParIdAsync(int id) {
        var jour = await _jourFermetureRepository.GetByIdAsync(id);
        return jour == null ? null : VersDto(jour);
    }

    public async Task<DeclarerFermetureResultatDto> DeclarerAsync(JourFermetureRequestDto requete) {
        var erreur = await ValiderAsync(requete);
        if (erreur != null) {
            return new DeclarerFermetureResultatDto { Succes = false, MessageErreur = erreur };
        }

        var jour = await _jourFermetureRepository.AddAsync(new JourFermeture {
            SiteId = requete.SiteId,
            Date = requete.Date
        });

        await RegenererAsync(requete.SiteId, requete.Date);

        return new DeclarerFermetureResultatDto { Succes = true, Fermeture = VersDto(jour) };
    }

    public async Task<bool> SupprimerAsync(int id) {
        var jour = await _jourFermetureRepository.GetByIdAsync(id);
        if (jour == null)
            return false;

        await _jourFermetureRepository.DeleteAsync(id);
        await RegenererAsync(jour.SiteId, jour.Date);

        return true;
    }

    private async Task<string?> ValiderAsync(JourFermetureRequestDto requete) {
        if (requete.SiteId.HasValue && await _siteRepository.GetByIdAsync(requete.SiteId.Value) == null)
            return "Site introuvable.";

        if (await _jourFermetureRepository.ExisteAsync(requete.SiteId, requete.Date))
            return "Cette fermeture est déjà déclarée.";

        return null;
    }

    // Une fermeture propre à un site n'affecte que ses disponibilités ; une fermeture globale
    // (siteId NULL) peut concerner tous les sites (EF-bk-024).
    private async Task RegenererAsync(int? siteId, DateOnly date) {
        var annee = (short)date.Year;

        if (siteId.HasValue) {
            await _disponibiliteGenerationService.GenererPourSiteEtAnneeAsync(siteId.Value, annee);
        } else {
            await _disponibiliteGenerationService.GenererPourTousLesSitesEtAnneeAsync(annee);
        }
    }

    private static JourFermetureDto VersDto(JourFermeture jour) => new() {
        Id = jour.Id,
        SiteId = jour.SiteId,
        Date = jour.Date
    };
}

using Microsoft.AspNetCore.Mvc;
using PadelManager.Interfaces;

namespace PadelManager.Api.Controllers;

[ApiController]
[Route("api/sites/{siteId:int}/disponibilites")]
public class DisponibiliteController : ControllerBase {
    private readonly ISiteService _siteService;
    private readonly IDisponibiliteService _disponibiliteService;
    private readonly IDisponibiliteGenerationService _disponibiliteGenerationService;

    public DisponibiliteController(
        ISiteService siteService,
        IDisponibiliteService disponibiliteService,
        IDisponibiliteGenerationService disponibiliteGenerationService) {
        _siteService = siteService;
        _disponibiliteService = disponibiliteService;
        _disponibiliteGenerationService = disponibiliteGenerationService;
    }

    [HttpGet]
    public async Task<IActionResult> ConsulterPlanning(int siteId, [FromQuery] DateOnly from, [FromQuery] DateOnly to) {
        var disponibilites = await _disponibiliteService.ConsulterPlanningAsync(siteId, from, to);
        if (disponibilites == null)
            return NotFound(new { message = "Site introuvable." });

        return Ok(disponibilites);
    }

    // Déclenchement manuel de la génération (EF-bk-022). Utile par ex. après ajout de
    // fermetures ponctuelles (JOUR_FERMETURE) postérieur au paramétrage de l'horaire, celui-ci
    // régénérant déjà automatiquement à chaque PUT /horaires/{annee}.
    [HttpPost("generation")]
    public async Task<IActionResult> Generer(int siteId, [FromQuery] short annee) {
        if (await _siteService.ObtenirParIdAsync(siteId) == null)
            return NotFound(new { message = "Site introuvable." });

        var nombreGenere = await _disponibiliteGenerationService.GenererPourSiteEtAnneeAsync(siteId, annee);
        if (nombreGenere == null)
            return NotFound(new { message = "Aucun horaire configuré pour ce site et cette année." });

        return Ok(new { count = nombreGenere });
    }
}

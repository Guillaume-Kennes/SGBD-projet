using Microsoft.AspNetCore.Mvc;
using PadelManager.Interfaces;

namespace PadelManager.Api.Controllers;

[ApiController]
[Route("api/statistiques")]
public class StatistiqueController : ControllerBase {
    private readonly IStatistiqueService _statistiqueService;
    private readonly IAdminPorteeService _adminPorteeService;

    public StatistiqueController(IStatistiqueService statistiqueService, IAdminPorteeService adminPorteeService) {
        _statistiqueService = statistiqueService;
        _adminPorteeService = adminPorteeService;
    }

    // siteId omis -> tous les sites (admin Global), fourni -> ce seul site (admin de Site) —
    // même convention que GET api/matchs/etat. Portée vérifiée côté serveur (issue #13).
    [HttpGet("chiffre-affaires")]
    public async Task<IActionResult> ObtenirChiffreAffaires([FromQuery] int? siteId, [FromQuery] string adminMatricule) {
        var portee = await _adminPorteeService.VerifierPorteeSiteAsync(adminMatricule, siteId);
        if (!portee.Autorise)
            return StatusCode(403, new { message = portee.MessageErreur });

        var chiffreAffaires = await _statistiqueService.ObtenirChiffreAffairesAsync(siteId);
        return Ok(chiffreAffaires);
    }

    // EF-bk-016 : matchs publics/privés, taux d'occupation, membres actifs.
    [HttpGet]
    public async Task<IActionResult> ObtenirStatistiques([FromQuery] int? siteId, [FromQuery] string adminMatricule) {
        var portee = await _adminPorteeService.VerifierPorteeSiteAsync(adminMatricule, siteId);
        if (!portee.Autorise)
            return StatusCode(403, new { message = portee.MessageErreur });

        var statistiques = await _statistiqueService.ObtenirStatistiquesAsync(siteId);
        return Ok(statistiques);
    }
}

using Microsoft.AspNetCore.Mvc;
using PadelManager.Interfaces;

namespace PadelManager.Api.Controllers;

[ApiController]
[Route("api/statistiques")]
public class StatistiqueController : ControllerBase {
    private readonly IStatistiqueService _statistiqueService;

    public StatistiqueController(IStatistiqueService statistiqueService) {
        _statistiqueService = statistiqueService;
    }

    // siteId omis -> tous les sites (admin Global), fourni -> ce seul site (admin de Site) —
    // même convention que GET api/matchs/etat.
    [HttpGet("chiffre-affaires")]
    public async Task<IActionResult> ObtenirChiffreAffaires([FromQuery] int? siteId) {
        var chiffreAffaires = await _statistiqueService.ObtenirChiffreAffairesAsync(siteId);
        return Ok(chiffreAffaires);
    }
}

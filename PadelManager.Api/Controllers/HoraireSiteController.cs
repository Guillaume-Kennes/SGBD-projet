using Microsoft.AspNetCore.Mvc;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;

namespace PadelManager.Api.Controllers;

[ApiController]
[Route("api/sites/{siteId:int}/horaires")]
public class HoraireSiteController : ControllerBase {
    private readonly IHoraireSiteService _horaireSiteService;

    public HoraireSiteController(IHoraireSiteService horaireSiteService) {
        _horaireSiteService = horaireSiteService;
    }

    [HttpGet("{annee:int}")]
    public async Task<IActionResult> Obtenir(int siteId, short annee) {
        var horaire = await _horaireSiteService.ObtenirHoraireAsync(siteId, annee);
        if (horaire == null)
            return NotFound(new { message = "Aucun horaire configuré pour ce site et cette année." });

        return Ok(horaire);
    }

    [HttpPut("{annee:int}")]
    public async Task<IActionResult> Definir(int siteId, short annee, [FromBody] HoraireSiteRequestDto requete) {
        var resultat = await _horaireSiteService.DefinirHoraireAsync(siteId, annee, requete);
        if (!resultat.Succes)
            return BadRequest(new { message = resultat.MessageErreur });

        return Ok(resultat.Horaire);
    }
}

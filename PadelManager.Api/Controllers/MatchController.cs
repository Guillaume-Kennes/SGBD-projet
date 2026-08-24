using Microsoft.AspNetCore.Mvc;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;

namespace PadelManager.Api.Controllers;

[ApiController]
[Route("api/matchs")]
public class MatchController : ControllerBase {
    private readonly IMatchService _matchService;

    public MatchController(IMatchService matchService) {
        _matchService = matchService;
    }

    // Route absolue (comme JourFermetureController) : la consultation des créneaux réellement
    // libres est toujours relative à un site.
    [HttpGet("/api/sites/{siteId:int}/creneaux-disponibles")]
    public async Task<IActionResult> ObtenirCreneauxDisponibles(int siteId, [FromQuery] DateOnly date) {
        var creneaux = await _matchService.ObtenirCreneauxDisponiblesAsync(siteId, date);
        if (creneaux == null)
            return NotFound(new { message = "Site introuvable." });

        return Ok(creneaux);
    }

    [HttpPost]
    public async Task<IActionResult> CreerPrive([FromBody] CreerMatchPriveRequestDto requete) {
        var resultat = await _matchService.CreerMatchPriveAsync(requete);
        if (!resultat.Succes)
            return BadRequest(new { message = resultat.MessageErreur });

        return Ok(resultat.Match);
    }

    [HttpPost("publics")]
    public async Task<IActionResult> CreerPublic([FromBody] CreerMatchPublicRequestDto requete) {
        var resultat = await _matchService.CreerMatchPublicAsync(requete);
        if (!resultat.Succes)
            return BadRequest(new { message = resultat.MessageErreur });

        return Ok(resultat.Match);
    }

    [HttpGet("publics")]
    public async Task<IActionResult> ObtenirPublics([FromQuery] string membreMatricule) {
        var matchs = await _matchService.ObtenirMatchsPublicsAsync(membreMatricule);
        if (matchs == null)
            return NotFound(new { message = "Membre introuvable." });

        return Ok(matchs);
    }

    [HttpPost("{id:int}/inscription")]
    public async Task<IActionResult> Rejoindre(int id, [FromBody] RejoindreMatchRequestDto requete) {
        var resultat = await _matchService.RejoindreMatchPublicAsync(id, requete.MembreMatricule);
        if (!resultat.Succes)
            return BadRequest(new { message = resultat.MessageErreur });

        return Ok(resultat);
    }
}

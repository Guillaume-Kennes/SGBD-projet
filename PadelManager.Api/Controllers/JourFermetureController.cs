using Microsoft.AspNetCore.Mvc;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;

namespace PadelManager.Api.Controllers;

[ApiController]
[Route("api/fermetures-ponctuelles")]
public class JourFermetureController : ControllerBase {
    private readonly IJourFermetureService _jourFermetureService;

    public JourFermetureController(IJourFermetureService jourFermetureService) {
        _jourFermetureService = jourFermetureService;
    }

    // Route absolue (plutôt que imbriquée sous ce contrôleur) car la consultation, contrairement
    // à la déclaration, est toujours relative à un site donné (JOUR_FERMETURE.siteId NULL y
    // apparaît fusionné, cf. IJourFermetureRepository.GetForSiteAndAnneeAsync).
    [HttpGet("/api/sites/{siteId:int}/fermetures-ponctuelles")]
    public async Task<IActionResult> ObtenirPourSite(int siteId, [FromQuery] short annee) {
        var fermetures = await _jourFermetureService.ObtenirPourSiteEtAnneeAsync(siteId, annee);
        return Ok(fermetures);
    }

    // requete.SiteId == null -> fermeture ponctuelle globale, tous les sites (EF-bk-024).
    [HttpPost]
    public async Task<IActionResult> Declarer([FromBody] JourFermetureRequestDto requete) {
        var resultat = await _jourFermetureService.DeclarerAsync(requete);
        if (!resultat.Succes)
            return BadRequest(new { message = resultat.MessageErreur });

        return Ok(resultat.Fermeture);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Supprimer(int id) {
        var supprime = await _jourFermetureService.SupprimerAsync(id);
        if (!supprime)
            return NotFound(new { message = "Fermeture ponctuelle introuvable." });

        return NoContent();
    }
}

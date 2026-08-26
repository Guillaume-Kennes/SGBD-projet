using Microsoft.AspNetCore.Mvc;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;

namespace PadelManager.Api.Controllers;

[ApiController]
[Route("api/fermetures-hebdo-globales")]
public class FermetureHebdoGlobaleController : ControllerBase {
    private readonly IFermetureHebdoGlobaleService _fermetureHebdoGlobaleService;
    private readonly IAdminPorteeService _adminPorteeService;

    public FermetureHebdoGlobaleController(IFermetureHebdoGlobaleService fermetureHebdoGlobaleService, IAdminPorteeService adminPorteeService) {
        _fermetureHebdoGlobaleService = fermetureHebdoGlobaleService;
        _adminPorteeService = adminPorteeService;
    }

    [HttpGet("{annee:int}")]
    public async Task<IActionResult> Obtenir(short annee, [FromQuery] string adminMatricule) {
        var portee = await _adminPorteeService.VerifierAdminGlobalAsync(adminMatricule);
        if (!portee.Autorise)
            return StatusCode(403, new { message = portee.MessageErreur });

        var fermeture = await _fermetureHebdoGlobaleService.ObtenirAsync(annee);
        if (fermeture == null)
            return NotFound(new { message = "Aucune fermeture hebdomadaire globale définie pour cette année." });

        return Ok(fermeture);
    }

    // Réservé à l'administrateur global (EF-bk-023) : désormais vérifié aussi côté serveur
    // (issue #13), rejette tout admin de site sans exception, quel que soit le paramètre envoyé.
    [HttpPut("{annee:int}")]
    public async Task<IActionResult> Definir(short annee, [FromBody] FermetureHebdoGlobaleRequestDto requete) {
        var portee = await _adminPorteeService.VerifierAdminGlobalAsync(requete.AdminMatricule);
        if (!portee.Autorise)
            return StatusCode(403, new { message = portee.MessageErreur });

        var resultat = await _fermetureHebdoGlobaleService.DefinirAsync(annee, requete);
        if (!resultat.Succes)
            return BadRequest(new { message = resultat.MessageErreur });

        return Ok(resultat.Fermeture);
    }

    [HttpDelete("{annee:int}")]
    public async Task<IActionResult> Supprimer(short annee, [FromQuery] string adminMatricule) {
        var portee = await _adminPorteeService.VerifierAdminGlobalAsync(adminMatricule);
        if (!portee.Autorise)
            return StatusCode(403, new { message = portee.MessageErreur });

        var supprime = await _fermetureHebdoGlobaleService.SupprimerAsync(annee);
        if (!supprime)
            return NotFound(new { message = "Aucune fermeture hebdomadaire globale définie pour cette année." });

        return NoContent();
    }
}

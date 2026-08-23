using Microsoft.AspNetCore.Mvc;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;

namespace PadelManager.Api.Controllers;

[ApiController]
[Route("api/fermetures-hebdo-globales")]
public class FermetureHebdoGlobaleController : ControllerBase {
    private readonly IFermetureHebdoGlobaleService _fermetureHebdoGlobaleService;

    public FermetureHebdoGlobaleController(IFermetureHebdoGlobaleService fermetureHebdoGlobaleService) {
        _fermetureHebdoGlobaleService = fermetureHebdoGlobaleService;
    }

    [HttpGet("{annee:int}")]
    public async Task<IActionResult> Obtenir(short annee) {
        var fermeture = await _fermetureHebdoGlobaleService.ObtenirAsync(annee);
        if (fermeture == null)
            return NotFound(new { message = "Aucune fermeture hebdomadaire globale définie pour cette année." });

        return Ok(fermeture);
    }

    // Réservé à l'administrateur global (EF-bk-023) : restriction appliquée côté UI WinForms,
    // ce projet ne comportant pas de middleware d'autorisation basé sur l'identité de l'appelant.
    [HttpPut("{annee:int}")]
    public async Task<IActionResult> Definir(short annee, [FromBody] FermetureHebdoGlobaleRequestDto requete) {
        var resultat = await _fermetureHebdoGlobaleService.DefinirAsync(annee, requete);
        if (!resultat.Succes)
            return BadRequest(new { message = resultat.MessageErreur });

        return Ok(resultat.Fermeture);
    }

    [HttpDelete("{annee:int}")]
    public async Task<IActionResult> Supprimer(short annee) {
        var supprime = await _fermetureHebdoGlobaleService.SupprimerAsync(annee);
        if (!supprime)
            return NotFound(new { message = "Aucune fermeture hebdomadaire globale définie pour cette année." });

        return NoContent();
    }
}

using Microsoft.AspNetCore.Mvc;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;

namespace PadelManager.Api.Controllers;

[ApiController]
[Route("api/sites/{siteId:int}/horaires")]
public class HoraireSiteController : ControllerBase {
    private readonly IHoraireSiteService _horaireSiteService;
    private readonly IAdminPorteeService _adminPorteeService;

    public HoraireSiteController(IHoraireSiteService horaireSiteService, IAdminPorteeService adminPorteeService) {
        _horaireSiteService = horaireSiteService;
        _adminPorteeService = adminPorteeService;
    }

    // Lecture seule, non soumise au contrôle de portée admin (issue #13/#14) : cette route est
    // aussi consommée par l'application Membre (CreerMatchForm/CreerMatchPublicForm), pour savoir
    // à l'avance quels jours un site est ouvert avant de rechercher un créneau — n'importe quel
    // membre doit pouvoir la lire, ce n'est pas une donnée réservée aux administrateurs.
    [HttpGet("{annee:int}")]
    public async Task<IActionResult> Obtenir(int siteId, short annee) {
        var horaire = await _horaireSiteService.ObtenirHoraireAsync(siteId, annee);
        if (horaire == null)
            return NotFound(new { message = "Aucun horaire configuré pour ce site et cette année." });

        return Ok(horaire);
    }

    [HttpPut("{annee:int}")]
    public async Task<IActionResult> Definir(int siteId, short annee, [FromBody] HoraireSiteRequestDto requete) {
        var portee = await _adminPorteeService.VerifierPorteeSiteAsync(requete.AdminMatricule, siteId);
        if (!portee.Autorise)
            return StatusCode(403, new { message = portee.MessageErreur });

        var resultat = await _horaireSiteService.DefinirHoraireAsync(siteId, annee, requete);
        if (!resultat.Succes)
            return BadRequest(new { message = resultat.MessageErreur });

        return Ok(resultat.Horaire);
    }
}

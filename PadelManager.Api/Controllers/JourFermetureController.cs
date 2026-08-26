using Microsoft.AspNetCore.Mvc;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;

namespace PadelManager.Api.Controllers;

[ApiController]
[Route("api/fermetures-ponctuelles")]
public class JourFermetureController : ControllerBase {
    private readonly IJourFermetureService _jourFermetureService;
    private readonly IAdminPorteeService _adminPorteeService;

    public JourFermetureController(IJourFermetureService jourFermetureService, IAdminPorteeService adminPorteeService) {
        _jourFermetureService = jourFermetureService;
        _adminPorteeService = adminPorteeService;
    }

    // Route absolue (plutôt que imbriquée sous ce contrôleur) car la consultation, contrairement
    // à la déclaration, est toujours relative à un site donné (JOUR_FERMETURE.siteId NULL y
    // apparaît fusionné, cf. IJourFermetureRepository.GetForSiteAndAnneeAsync).
    // Lecture seule, non soumise au contrôle de portée admin (issue #13/#14) : cette route est
    // aussi consommée par l'application Membre (CreerMatchForm/CreerMatchPublicForm), pour savoir
    // à l'avance si un jour est ponctuellement fermé avant de rechercher un créneau — n'importe
    // quel membre doit pouvoir la lire, ce n'est pas une donnée réservée aux administrateurs.
    [HttpGet("/api/sites/{siteId:int}/fermetures-ponctuelles")]
    public async Task<IActionResult> ObtenirPourSite(int siteId, [FromQuery] short annee) {
        var fermetures = await _jourFermetureService.ObtenirPourSiteEtAnneeAsync(siteId, annee);
        return Ok(fermetures);
    }

    // requete.SiteId == null -> fermeture ponctuelle globale, tous les sites (EF-bk-024) — réservé
    // à l'admin global, comme n'importe quel autre siteId != son propre site pour un admin de site.
    [HttpPost]
    public async Task<IActionResult> Declarer([FromBody] JourFermetureRequestDto requete) {
        var portee = await _adminPorteeService.VerifierPorteeSiteAsync(requete.AdminMatricule, requete.SiteId);
        if (!portee.Autorise)
            return StatusCode(403, new { message = portee.MessageErreur });

        var resultat = await _jourFermetureService.DeclarerAsync(requete);
        if (!resultat.Succes)
            return BadRequest(new { message = resultat.MessageErreur });

        return Ok(resultat.Fermeture);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Supprimer(int id, [FromQuery] string adminMatricule) {
        // Le siteId ciblé n'est pas fourni par le client sur une suppression par id : on le lit
        // depuis la fermeture elle-même avant de vérifier la portée (jamais faire confiance à un
        // siteId que le client n'a même pas à envoyer ici).
        var fermeture = await _jourFermetureService.ObtenirParIdAsync(id);
        if (fermeture == null)
            return NotFound(new { message = "Fermeture ponctuelle introuvable." });

        var portee = await _adminPorteeService.VerifierPorteeSiteAsync(adminMatricule, fermeture.SiteId);
        if (!portee.Autorise)
            return StatusCode(403, new { message = portee.MessageErreur });

        var supprime = await _jourFermetureService.SupprimerAsync(id);
        if (!supprime)
            return NotFound(new { message = "Fermeture ponctuelle introuvable." });

        return NoContent();
    }
}

using Microsoft.AspNetCore.Mvc;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;

namespace PadelManager.Api.Controllers;

[ApiController]
[Route("api/matchs")]
public class MatchController : ControllerBase {
    private readonly IMatchService _matchService;
    private readonly IAdminPorteeService _adminPorteeService;

    public MatchController(IMatchService matchService, IAdminPorteeService adminPorteeService) {
        _matchService = matchService;
        _adminPorteeService = adminPorteeService;
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

    [HttpGet("montant-a-payer")]
    public async Task<IActionResult> ObtenirMontantAPayer([FromQuery] string membreMatricule) {
        var montant = await _matchService.ObtenirMontantAPayerAsync(membreMatricule);
        return Ok(montant);
    }

    // Route absolue : le paiement porte sur une PARTICIPATION, pas un match (EF-bk-007, joueur
    // ajouté à un match privé qui paie sa part en attente).
    [HttpPost("/api/participations/{id:int}/paiement")]
    public async Task<IActionResult> PayerParticipation(int id, [FromBody] PayerParticipationRequestDto requete) {
        var resultat = await _matchService.PayerParticipationAsync(id, requete.MembreMatricule);
        if (!resultat.Succes)
            return BadRequest(new { message = resultat.MessageErreur });

        return Ok(resultat);
    }

    // Route absolue, comme la précédente : porte sur des PARTICIPATION, pas des MATCH.
    [HttpGet("/api/participations/en-attente")]
    public async Task<IActionResult> ObtenirParticipationsEnAttente([FromQuery] string membreMatricule) {
        var participations = await _matchService.ObtenirParticipationsEnAttenteAsync(membreMatricule);
        if (participations == null)
            return NotFound(new { message = "Membre introuvable." });

        return Ok(participations);
    }

    [HttpGet("mes-reservations")]
    public async Task<IActionResult> ObtenirReservations([FromQuery] string membreMatricule) {
        var reservations = await _matchService.ObtenirReservationsAsync(membreMatricule);
        if (reservations == null)
            return NotFound(new { message = "Membre introuvable." });

        return Ok(reservations);
    }

    // membreMatricule en query, comme les autres GET de ce contrôleur : nécessaire ici pour
    // déterminer si le membre est autorisé à consulter ce match précis (EF-bk-021/EF-bk-012).
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenirDetail(int id, [FromQuery] string membreMatricule) {
        var detail = await _matchService.ObtenirDetailAsync(id, membreMatricule);
        if (detail == null)
            return NotFound(new { message = "Match introuvable." });

        return Ok(detail);
    }

    // Vue administrateur (EF-bk-014) : siteId omis -> tous les sites (admin Global), fourni ->
    // filtré à ce site (admin de Site). Portée vérifiée côté serveur (issue #13).
    [HttpGet("etat")]
    public async Task<IActionResult> ObtenirEtat([FromQuery] int? siteId, [FromQuery] string adminMatricule) {
        var portee = await _adminPorteeService.VerifierPorteeSiteAsync(adminMatricule, siteId);
        if (!portee.Autorise)
            return StatusCode(403, new { message = portee.MessageErreur });

        var matchs = await _matchService.ObtenirEtatMatchsAsync(siteId);
        return Ok(matchs);
    }

    // Récapitulatif des terrains (EF-bk-014, "matchs et terrains"), même convention de portée.
    [HttpGet("terrains")]
    public async Task<IActionResult> ObtenirRecapitulatifTerrains([FromQuery] int? siteId, [FromQuery] string adminMatricule) {
        var portee = await _adminPorteeService.VerifierPorteeSiteAsync(adminMatricule, siteId);
        if (!portee.Autorise)
            return StatusCode(403, new { message = portee.MessageErreur });

        var recap = await _matchService.ObtenirRecapitulatifTerrainsAsync(siteId);
        return Ok(recap);
    }
}

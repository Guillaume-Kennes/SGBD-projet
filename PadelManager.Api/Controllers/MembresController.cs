using Microsoft.AspNetCore.Mvc;
using PadelManager.Interfaces;

namespace PadelManager.Api.Controllers;

[ApiController]
[Route("api/membres")]
public class MembresController : ControllerBase {
    private readonly IMembreService _membreService;
    private readonly IAdminPorteeService _adminPorteeService;

    public MembresController(IMembreService membreService, IAdminPorteeService adminPorteeService) {
        _membreService = membreService;
        _adminPorteeService = adminPorteeService;
    }

    // EF-bk-017 : siteId omis -> tous les membres, tous types (admin Global) ; fourni -> seulement
    // les membres SITE de ce site (admin de Site — cf. IMembreRepository.GetTousAsync). Portée
    // vérifiée côté serveur (issue #13).
    [HttpGet]
    public async Task<IActionResult> ObtenirMembres([FromQuery] int? siteId, [FromQuery] string adminMatricule) {
        var portee = await _adminPorteeService.VerifierPorteeSiteAsync(adminMatricule, siteId);
        if (!portee.Autorise)
            return StatusCode(403, new { message = portee.MessageErreur });

        var membres = await _membreService.ObtenirMembresAsync(siteId);
        return Ok(membres);
    }
}

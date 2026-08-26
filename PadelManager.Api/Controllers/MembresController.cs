using Microsoft.AspNetCore.Mvc;
using PadelManager.Interfaces;

namespace PadelManager.Api.Controllers;

[ApiController]
[Route("api/membres")]
public class MembresController : ControllerBase {
    private readonly IMembreService _membreService;

    public MembresController(IMembreService membreService) {
        _membreService = membreService;
    }

    // EF-bk-017 : siteId omis -> tous les membres, tous types (admin Global) ; fourni -> seulement
    // les membres SITE de ce site (admin de Site — cf. IMembreRepository.GetTousAsync).
    [HttpGet]
    public async Task<IActionResult> ObtenirMembres([FromQuery] int? siteId) {
        var membres = await _membreService.ObtenirMembresAsync(siteId);
        return Ok(membres);
    }
}

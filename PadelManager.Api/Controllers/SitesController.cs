using Microsoft.AspNetCore.Mvc;
using PadelManager.Interfaces;

namespace PadelManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SitesController : ControllerBase {
    private readonly ISiteService _siteService;

    public SitesController(ISiteService siteService) {
        _siteService = siteService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenirTous() {
        var sites = await _siteService.ObtenirTousAsync();
        return Ok(sites);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenirParId(int id) {
        var site = await _siteService.ObtenirParIdAsync(id);
        if (site == null)
            return NotFound(new { message = "Site introuvable." });

        return Ok(site);
    }
}

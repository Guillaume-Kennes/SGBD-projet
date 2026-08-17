using Microsoft.AspNetCore.Mvc;
using PadelManager.Interfaces;

namespace PadelManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase {
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) {
        _authService = authService;
    }

    [HttpPost("connexion")]
    public async Task<IActionResult> Connexion([FromBody] string matricule) {
        var resultat = await _authService.SeConnecterAsync(matricule);
        if (resultat == null)
            return Unauthorized(new { message = "Matricule inconnu." });

        return Ok(resultat);
    }
}


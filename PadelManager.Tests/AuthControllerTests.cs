using Microsoft.AspNetCore.Mvc;
using Moq;
using PadelManager.Api.Controllers;
using PadelManager.Interfaces;
using PadelManager.Models.Dtos;
using Xunit;

namespace PadelManager.Tests;

public class AuthControllerTests {
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests() {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Connexion_MatriculeValide_RetourneOk() {
        // Arrange
        var dto = new ConnexionResultatDto { Matricule = "G0001", TypeUtilisateur = "Membre", Type = "GLOBAL", SiteId = null };
        _authServiceMock.Setup(s => s.SeConnecterAsync("G0001")).ReturnsAsync(dto);

        // Act
        var resultat = await _controller.Connexion("G0001");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultat);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    public async Task Connexion_MatriculeInconnu_RetourneUnauthorized() {
        // Arrange
        _authServiceMock.Setup(s => s.SeConnecterAsync("X9999")).ReturnsAsync((ConnexionResultatDto?)null);

        // Act
        var resultat = await _controller.Connexion("X9999");

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(resultat);
    }
}


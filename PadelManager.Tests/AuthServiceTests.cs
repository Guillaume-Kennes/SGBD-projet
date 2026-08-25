using Moq;
using PadelManager.Interfaces;
using PadelManager.Models;
using PadelManager.Services;
using Xunit;

namespace PadelManager.Tests;

public class AuthServiceTests {
    private readonly Mock<IMembreRepository> _membreRepoMock;
    private readonly Mock<IAdministrateurRepository> _adminRepoMock;
    private readonly AuthService _authService;

    public AuthServiceTests() {
        _membreRepoMock = new Mock<IMembreRepository>();
        _adminRepoMock = new Mock<IAdministrateurRepository>();
        _authService = new AuthService(_membreRepoMock.Object, _adminRepoMock.Object);
    }

    [Fact]
    public async Task SeConnecterAsync_MatriculeMembreValide_RetourneDtoMembre() {
        // Arrange : TypeMembreNavigation toujours chargée en pratique (MembreRepository.Include)
        var membre = new Membre {
            Matricule = "G001", TypeMembre = "GLOBAL", SiteId = null,
            TypeMembreNavigation = new TypeMembre { Code = "GLOBAL", Libelle = "Membre global", AnticipationMaxJours = 21, PrefixeMatricule = "G" }
        };
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("G001")).ReturnsAsync(membre);

        // Act
        var resultat = await _authService.SeConnecterAsync("G001");

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal("G001", resultat!.Matricule);
        Assert.Equal("Membre", resultat.TypeUtilisateur);
        Assert.Equal("GLOBAL", resultat.Type);
        Assert.Null(resultat.SiteId);
        Assert.Equal(21, resultat.AnticipationMaxJours);
    }

    [Fact]
    public async Task SeConnecterAsync_MatriculeAdministrateurValide_RetourneDtoAdministrateur() {
        // Arrange
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("AG01")).ReturnsAsync((Membre?)null);
        var admin = new Administrateur { Matricule = "AG01", Type = "GLOBAL", SiteId = null };
        _adminRepoMock.Setup(r => r.GetByMatriculeAsync("AG01")).ReturnsAsync(admin);

        // Act
        var resultat = await _authService.SeConnecterAsync("AG01");

        // Assert
        Assert.NotNull(resultat);
        Assert.Equal("Administrateur", resultat!.TypeUtilisateur);
        Assert.Equal("GLOBAL", resultat.Type);
        Assert.Null(resultat.AnticipationMaxJours);
    }

    [Fact]
    public async Task SeConnecterAsync_MatriculeInconnu_RetourneNull() {
        // Arrange
        _membreRepoMock.Setup(r => r.GetByMatriculeAsync("X9999")).ReturnsAsync((Membre?)null);
        _adminRepoMock.Setup(r => r.GetByMatriculeAsync("X9999")).ReturnsAsync((Administrateur?)null);

        // Act
        var resultat = await _authService.SeConnecterAsync("X9999");

        // Assert
        Assert.Null(resultat);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SeConnecterAsync_MatriculeVideOuNull_RetourneNull(string? matricule) {
        // Act
        var resultat = await _authService.SeConnecterAsync(matricule!);

        // Assert
        Assert.Null(resultat);
        _membreRepoMock.Verify(r => r.GetByMatriculeAsync(It.IsAny<string>()), Times.Never);
    }
}




using PadelManager.Models.Dtos;

namespace PadelManager.Interfaces;

public interface IAuthService {
    Task<ConnexionResultatDto?> SeConnecterAsync(string matricule);
}


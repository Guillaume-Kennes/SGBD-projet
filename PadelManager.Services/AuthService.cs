using PadelManager.Interfaces;
using PadelManager.Models.Dtos;

namespace PadelManager.Services;

public class AuthService : IAuthService {
    private readonly IMembreRepository _membreRepository;
    private readonly IAdministrateurRepository _administrateurRepository;

    public AuthService(IMembreRepository membreRepository, IAdministrateurRepository administrateurRepository) {
        _membreRepository = membreRepository;
        _administrateurRepository = administrateurRepository;
    }

    public async Task<ConnexionResultatDto?> SeConnecterAsync(string matricule) {
        if (string.IsNullOrWhiteSpace(matricule))
            return null;

        var membre = await _membreRepository.GetByMatriculeAsync(matricule);
        if (membre != null) {
            return new ConnexionResultatDto {
                Matricule = membre.Matricule,
                TypeUtilisateur = "Membre",
                Type = membre.TypeMembre,
                SiteId = membre.SiteId,
                AnticipationMaxJours = membre.TypeMembreNavigation.AnticipationMaxJours
            };
        }

        var administrateur = await _administrateurRepository.GetByMatriculeAsync(matricule);
        if (administrateur != null) {
            return new ConnexionResultatDto {
                Matricule = administrateur.Matricule,
                TypeUtilisateur = "Administrateur",
                Type = administrateur.Type,
                SiteId = administrateur.SiteId
            };
        }

        return null; // matricule inconnu
    }
}


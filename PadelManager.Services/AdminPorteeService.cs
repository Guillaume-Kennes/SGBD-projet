using PadelManager.Interfaces;
using PadelManager.Models.Dtos;

namespace PadelManager.Services;

public class AdminPorteeService : IAdminPorteeService {
    private readonly IAdministrateurRepository _administrateurRepository;

    public AdminPorteeService(IAdministrateurRepository administrateurRepository) {
        _administrateurRepository = administrateurRepository;
    }

    public async Task<PorteeAdminResultatDto> VerifierPorteeSiteAsync(string adminMatricule, int? siteIdDemande) {
        var admin = await _administrateurRepository.GetByMatriculeAsync(adminMatricule);
        if (admin == null)
            return Refuse("Administrateur inconnu.");

        if (admin.Type == "GLOBAL")
            return Autorise();

        // SITE : jamais null ("tous les sites" n'a pas de sens pour un admin de site), jamais un
        // autre site que le sien — un siteId incorrect est rejeté plutôt qu'ignoré silencieusement.
        if (siteIdDemande == null || siteIdDemande != admin.SiteId)
            return Refuse("Cet administrateur n'est pas autorisé pour ce site.");

        return Autorise();
    }

    public async Task<PorteeAdminResultatDto> VerifierAdminGlobalAsync(string adminMatricule) {
        var admin = await _administrateurRepository.GetByMatriculeAsync(adminMatricule);
        if (admin == null)
            return Refuse("Administrateur inconnu.");

        if (admin.Type != "GLOBAL")
            return Refuse("Réservé à l'administrateur global.");

        return Autorise();
    }

    private static PorteeAdminResultatDto Autorise() => new() { Autorise = true };

    private static PorteeAdminResultatDto Refuse(string message) => new() { Autorise = false, MessageErreur = message };
}

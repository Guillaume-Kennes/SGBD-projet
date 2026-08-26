using PadelManager.Models.Dtos;

namespace PadelManager.Interfaces;

// Vérification de portée serveur pour les écrans admin (issue #13). Ce projet ne comportant pas
// de middleware d'autorisation, l'identité de l'appelant (matricule admin) est passée
// explicitement par chaque contrôleur admin, qui appelle ce service AVANT le service métier
// concerné — factorisé ici pour ne pas dupliquer la même logique GLOBAL/SITE dans chacun d'eux.
public interface IAdminPorteeService {
    // Portée générale, lecture comme écriture (horaires, fermetures ponctuelles, matchs/terrains,
    // chiffre d'affaires, statistiques, membres). Un admin GLOBAL est autorisé quel que soit
    // siteIdDemande (y compris null = tous les sites). Un admin SITE n'est autorisé que si
    // siteIdDemande est EXACTEMENT son propre site (jamais null, jamais un autre site).
    Task<PorteeAdminResultatDto> VerifierPorteeSiteAsync(string adminMatricule, int? siteIdDemande);

    // FERMETURE_HEBDO_GLOBALE (EF-bk-023) : cas à part, réservé exclusivement à un admin GLOBAL,
    // sans notion de siteId — rejette tout admin SITE sans exception, quel que soit le paramètre.
    Task<PorteeAdminResultatDto> VerifierAdminGlobalAsync(string adminMatricule);
}

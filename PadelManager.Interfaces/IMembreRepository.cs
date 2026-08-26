using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface IMembreRepository {
    Task<Membre?> GetByMatriculeAsync(string matricule);

    // Liste des membres pour la vue administrateur (EF-bk-017). siteId fourni -> uniquement les
    // membres de type SITE rattachés à ce site (Global/Libre ne sont rattachés à aucun site, ils
    // n'apparaissent donc jamais dans une vue Site) ; omis -> tous les membres, tous types.
    Task<List<Membre>> GetTousAsync(int? siteId);
}

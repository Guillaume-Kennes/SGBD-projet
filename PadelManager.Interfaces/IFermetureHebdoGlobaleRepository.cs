using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface IFermetureHebdoGlobaleRepository {
    Task<FermetureHebdoGlobale?> GetByAnneeAsync(short annee);
}

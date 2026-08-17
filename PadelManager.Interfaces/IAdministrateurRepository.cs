using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface IAdministrateurRepository {
    Task<Administrateur?> GetByMatriculeAsync(string matricule);
}


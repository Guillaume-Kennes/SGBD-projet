using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface IMembreRepository {
    Task<Membre?> GetByMatriculeAsync(string matricule);


}


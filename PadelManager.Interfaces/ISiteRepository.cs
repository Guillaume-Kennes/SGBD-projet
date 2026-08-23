using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface ISiteRepository {
    Task<Site?> GetByIdAsync(int id);

    Task<List<Site>> GetAllAsync();
}

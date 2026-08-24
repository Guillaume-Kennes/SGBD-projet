using PadelManager.Models;

namespace PadelManager.Interfaces;

public interface ITerrainRepository {
    Task<Terrain?> GetByIdAsync(int id);

    Task<List<Terrain>> GetBySiteIdAsync(int siteId);
}

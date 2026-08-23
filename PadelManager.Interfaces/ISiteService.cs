using PadelManager.Models.Dtos;

namespace PadelManager.Interfaces;

public interface ISiteService {
    Task<SiteDto?> ObtenirParIdAsync(int id);

    Task<List<SiteDto>> ObtenirTousAsync();
}

using PadelManager.Interfaces;
using PadelManager.Models;
using PadelManager.Models.Dtos;

namespace PadelManager.Services;

public class SiteService : ISiteService {
    private readonly ISiteRepository _siteRepository;

    public SiteService(ISiteRepository siteRepository) {
        _siteRepository = siteRepository;
    }

    public async Task<SiteDto?> ObtenirParIdAsync(int id) {
        var site = await _siteRepository.GetByIdAsync(id);
        return site == null ? null : VersDto(site);
    }

    public async Task<List<SiteDto>> ObtenirTousAsync() {
        var sites = await _siteRepository.GetAllAsync();
        return sites.Select(VersDto).ToList();
    }

    private static SiteDto VersDto(Site site) => new() {
        Id = site.Id,
        Nom = site.Nom
    };
}

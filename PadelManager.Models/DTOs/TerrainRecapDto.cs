namespace PadelManager.Models.Dtos;

// Récapitulatif des terrains d'un site pour la vue administrateur (EF-bk-014 : "état des matchs
// ET des terrains"). IDs (TERRAIN.id, pas le numéro d'affichage) triés ascendant, cohérent avec
// AdminMatchDto.TerrainId — écran admin, où les IDs bruts sont exposés pour le contrôle/debug. La
// mise en forme (liste complète ou plage compressée) est laissée au client.
public class TerrainRecapDto {
    public int SiteId { get; set; }
    public string NomSite { get; set; } = null!;
    public List<int> TerrainIds { get; set; } = new();
}

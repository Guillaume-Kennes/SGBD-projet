namespace PadelManager.Models.Dtos;

// Récapitulatif des terrains d'un site pour la vue administrateur (EF-bk-014 : "état des matchs
// ET des terrains"). Numeros (TERRAIN.numero, pas l'id brut) triés ascendant, cohérent avec le
// reste de l'app (AdminMatchDto.NumeroTerrain) — la colonne "ID terrain" du tableau garde déjà le
// rôle de référence brute pour le contrôle DB. La mise en forme (liste complète ou plage
// compressée) est laissée au client.
public class TerrainRecapDto {
    public int SiteId { get; set; }
    public string NomSite { get; set; } = null!;
    public List<int> Numeros { get; set; } = new();
}

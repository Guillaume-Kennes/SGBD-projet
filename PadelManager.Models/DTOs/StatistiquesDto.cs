namespace PadelManager.Models.Dtos;

// Statistiques classiques d'un site (EF-bk-016) : matchs publics/privés, taux d'occupation,
// membres actifs. Approximation volontairement simple (CDC : "set raisonnable et classique, pas
// très poussé"), sur l'ensemble de la période disponible en base, sans fenêtre temporelle.
public class StatistiquesDto {
    public int SiteId { get; set; }
    public string NomSite { get; set; } = null!;
    public int NombreMatchsPublics { get; set; }
    public int NombreMatchsPrives { get; set; }

    // (nombre de matchs créés) / (nombre de créneaux DISPONIBILITE du site × nombre de terrains
    // du site) — ratio brut (0.60 = 60%), pas un pourcentage déjà multiplié par 100.
    public decimal TauxOccupation { get; set; }

    // Membres distincts ayant au moins une PARTICIPATION sur un match de ce site, payée ou non.
    public int MembresActifs { get; set; }
}

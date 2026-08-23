using System;
using System.Collections.Generic;

using PadelManager.Models;

public partial class Match
{
    public int Id { get; set; }

    public int SiteId { get; set; }

    public int TerrainId { get; set; }

    public DateTime DateHeure { get; set; }

    public string Visibilite { get; set; } = null!;

    public string OrganisateurMatricule { get; set; } = null!;

    public string Statut { get; set; } = null!;

    public virtual ICollection<Dette> DetteMatchOrigines { get; set; } = new List<Dette>();

    public virtual ICollection<Dette> DetteMatchReglements { get; set; } = new List<Dette>();

    public virtual Membre OrganisateurMatriculeNavigation { get; set; } = null!;

    public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();

    public virtual ICollection<Penalite> Penalites { get; set; } = new List<Penalite>();

    public virtual Site Site { get; set; } = null!;

    public virtual Terrain Terrain { get; set; } = null!;
}

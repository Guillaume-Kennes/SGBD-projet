using System;
using System.Collections.Generic;

using PadelManager.Models;

public partial class Membre
{
    public string Matricule { get; set; } = null!;

    public string TypeMembre { get; set; } = null!;

    public int? SiteId { get; set; }

    public virtual ICollection<Dette> Dettes { get; set; } = new List<Dette>();

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();

    public virtual ICollection<Penalite> Penalites { get; set; } = new List<Penalite>();

    public virtual Site? Site { get; set; }

    public virtual TypeMembre TypeMembreNavigation { get; set; } = null!;
}

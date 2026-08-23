using System;
using System.Collections.Generic;

using PadelManager.Models;

public partial class Site
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;

    public virtual ICollection<Administrateur> Administrateurs { get; set; } = new List<Administrateur>();

    public virtual ICollection<Disponibilite> Disponibilites { get; set; } = new List<Disponibilite>();

    public virtual ICollection<HoraireSite> HoraireSites { get; set; } = new List<HoraireSite>();

    public virtual ICollection<JourFermeture> JourFermetures { get; set; } = new List<JourFermeture>();

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual ICollection<Membre> Membres { get; set; } = new List<Membre>();

    public virtual ICollection<Terrain> Terrains { get; set; } = new List<Terrain>();
}

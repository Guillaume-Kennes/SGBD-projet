using System;
using System.Collections.Generic;

using PadelManager.Models;

public partial class Disponibilite
{
    public int Id { get; set; }

    public int SiteId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly HeureDebut { get; set; }

    public TimeOnly HeureFin { get; set; }

    public virtual Site Site { get; set; } = null!;
}

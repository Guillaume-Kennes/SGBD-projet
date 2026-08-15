using System;
using System.Collections.Generic;

namespace PadelManager.Models;

public partial class Administrateur
{
    public string Matricule { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int? SiteId { get; set; }

    public virtual Site? Site { get; set; }
}

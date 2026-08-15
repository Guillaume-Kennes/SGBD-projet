using System;
using System.Collections.Generic;

namespace PadelManager.Models;

public partial class Terrain
{
    public int Id { get; set; }

    public int SiteId { get; set; }

    public int Numero { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual Site Site { get; set; } = null!;
}

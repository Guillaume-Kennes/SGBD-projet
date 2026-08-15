using System;
using System.Collections.Generic;

namespace PadelManager.Models;

public partial class JourFermeture
{
    public int Id { get; set; }

    public int? SiteId { get; set; }

    public DateOnly Date { get; set; }

    public virtual Site? Site { get; set; }
}

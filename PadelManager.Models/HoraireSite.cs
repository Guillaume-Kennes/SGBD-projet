using System;
using System.Collections.Generic;

namespace PadelManager.Models;

public partial class HoraireSite
{
    public int Id { get; set; }

    public int SiteId { get; set; }

    public short Annee { get; set; }

    public TimeOnly HeureDebutReservation { get; set; }

    public TimeOnly HeureFinReservation { get; set; }

    public virtual Site Site { get; set; } = null!;
}

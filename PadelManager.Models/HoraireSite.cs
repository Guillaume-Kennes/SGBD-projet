using System;
using System.Collections.Generic;

using PadelManager.Models;

public partial class HoraireSite
{
    public int Id { get; set; }

    public int SiteId { get; set; }

    public short Annee { get; set; }

    public string JoursOuverture { get; set; } = null!;

    public TimeOnly HeureDebutReservation { get; set; }

    public TimeOnly HeureFinReservation { get; set; }

    public virtual Site Site { get; set; } = null!;
}

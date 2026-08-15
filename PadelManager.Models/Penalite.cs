using System;
using System.Collections.Generic;

namespace PadelManager.Models;

public partial class Penalite
{
    public int Id { get; set; }

    public string MembreMatricule { get; set; } = null!;

    public int MatchOrigineId { get; set; }

    public DateTime DateApplication { get; set; }

    public DateOnly DelaiJusquAu { get; set; }

    public virtual Match MatchOrigine { get; set; } = null!;

    public virtual Membre MembreMatriculeNavigation { get; set; } = null!;
}

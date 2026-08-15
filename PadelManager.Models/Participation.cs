using System;
using System.Collections.Generic;

namespace PadelManager.Models;

public partial class Participation
{
    public int Id { get; set; }

    public int MatchId { get; set; }

    public string MembreMatricule { get; set; } = null!;

    public DateTime DateInscription { get; set; }

    public virtual Match Match { get; set; } = null!;

    public virtual Membre MembreMatriculeNavigation { get; set; } = null!;

    public virtual Paiement? Paiement { get; set; }
}

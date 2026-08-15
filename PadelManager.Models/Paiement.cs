using System;
using System.Collections.Generic;

namespace PadelManager.Models;

public partial class Paiement
{
    public int Id { get; set; }

    public int ParticipationId { get; set; }

    public decimal MontantParticipation { get; set; }

    public decimal MontantDetteReportee { get; set; }

    public decimal? MontantTotal { get; set; }

    public DateTime DatePaiement { get; set; }

    public virtual Participation Participation { get; set; } = null!;
}

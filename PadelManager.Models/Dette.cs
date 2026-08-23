using System;
using System.Collections.Generic;
using PadelManager.Models;

public partial class Dette
{
    public int Id { get; set; }

    public string MembreMatricule { get; set; } = null!;

    public int MatchOrigineId { get; set; }

    public int? MatchReglementId { get; set; }

    public decimal Montant { get; set; }

    public bool Soldee { get; set; }

    public DateTime DateCreation { get; set; }

    public DateTime? DateReglement { get; set; }

    public virtual Match MatchOrigine { get; set; } = null!;

    public virtual Match? MatchReglement { get; set; }

    public virtual Membre MembreMatriculeNavigation { get; set; } = null!;
}

using System;
using System.Collections.Generic;

using PadelManager.Models;

public partial class TypeMembre
{
    public string Code { get; set; } = null!;

    public string Libelle { get; set; } = null!;

    public int AnticipationMaxJours { get; set; }

    public string PrefixeMatricule { get; set; } = null!;

    public virtual ICollection<Membre> Membres { get; set; } = new List<Membre>();
}

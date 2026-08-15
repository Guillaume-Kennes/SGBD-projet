using System;
using System.Collections.Generic;

namespace PadelManager.Models;

public partial class TypeMembre
{
    public string Code { get; set; } = null!;

    public string Libelle { get; set; } = null!;

    public int DelaiMinimumJours { get; set; }

    public string PrefixeMatricule { get; set; } = null!;

    public bool PeutOrganiser { get; set; }

    public virtual ICollection<Membre> Membres { get; set; } = new List<Membre>();
}

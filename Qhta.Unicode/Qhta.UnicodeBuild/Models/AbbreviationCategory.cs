﻿using System;
using System.Collections.Generic;

namespace Qhta.Unicode.Models;

public partial class AbbreviationCategory
{
    public string AbbrCat { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Abbreviation> Abbreviations { get; set; } = new List<Abbreviation>();
}

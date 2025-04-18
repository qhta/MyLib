using System;
using System.Collections.Generic;

namespace Qhta.Unicode.Models;

public partial class Alias
{
    public int Ord { get; set; }

    public string Alias1 { get; set; } = null!;

    public byte? Type { get; set; }

    public virtual UnicodeDatum UnicodeDatum { get; set; } = null!;
}

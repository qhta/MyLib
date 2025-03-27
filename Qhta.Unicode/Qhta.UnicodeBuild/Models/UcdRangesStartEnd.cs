using System;
using System.Collections.Generic;

namespace Qhta.Unicode.Models;

public partial class UcdRangesStartEnd
{
    public string StartCp { get; set; } = null!;

    public string? EndCp { get; set; }

    public int? BlockRange { get; set; }

    public int? Start { get; set; }

    public int? End { get; set; }

    public int? StartBl { get; set; }

    public int? EndBl { get; set; }
}

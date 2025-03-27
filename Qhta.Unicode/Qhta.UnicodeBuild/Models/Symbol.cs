﻿using System;
using System.Collections.Generic;

namespace Qhta.Unicode.Models;

public partial class Symbol
{
    public string Char { get; set; } = null!;

    public string? Name { get; set; }

    public string? Code { get; set; }

    public string? Notes { get; set; }
}

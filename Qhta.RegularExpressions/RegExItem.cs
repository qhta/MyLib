using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.RegularExpressions
{
  public class RegExItem
  {
    public RegExTag Tag { get; set; }

    public int Start { get; set; }

    public int Length => Str?.Length ?? 0;

    public string Str { get; set; }

    public RegExStatus Status { get; set; }

    public override string ToString()
    {
      return $"{Tag} ({Start}, {Length}) {Status}: {Str}";
    }
  }
}

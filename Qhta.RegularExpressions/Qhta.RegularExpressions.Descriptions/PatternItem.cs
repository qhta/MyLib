using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.RegularExpressions.Descriptions
{
  public class PatternItem
  {
    public string Str { get; set; }

    public string Description { get; set; }

    public bool? IsOK { get; internal set; }

  }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.RegularExpressions
{
  public abstract class RegExNamedItem: RegExItem
  {
    public string Name { get; set; }
  }
}

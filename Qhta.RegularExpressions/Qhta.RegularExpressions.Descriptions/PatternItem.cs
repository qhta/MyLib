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

    public override bool Equals(object obj)
    {
      bool result = true;

      if (obj is PatternItem other)
      {
        if (this.Str != other.Str)
        {
          result = false;
        }
        if (!this.Description.Equals(other.Description, StringComparison.CurrentCultureIgnoreCase))
        {
          result = false;
        }
        this.IsOK = result;
        return result;
      }
      return false;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}

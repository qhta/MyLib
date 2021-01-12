using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.RegularExpressions
{
  public class RegExCharset: RegExItem
  {
    public RegExItems Items { get; private set; } = new RegExItems();

    public override bool Equals(object obj)
    {
      if (obj is RegExCharset other)
      {
        if (this.Items == null || !this.Items.Equals(other.Items))
          return false;
        return base.Equals(obj);
      }
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}

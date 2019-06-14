using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.TextUtils
{
  public class StringsEqualityComparer : IEqualityComparer<string[]>
  {
    StringComparison stringComparison = StringComparison.CurrentCulture;

    public StringsEqualityComparer() { }

    public StringsEqualityComparer(StringComparison comparison)
    {
      stringComparison = comparison;
    }

    public bool Equals(string[] x, string[] y)
    {
      if (x.Length != y.Length)
        return false;
      for (int i = 0; i < x.Length; i++)
        if (!(x[i].Equals(y[i], stringComparison)))
          return false;
      return true;
    }

    public int GetHashCode(string[] obj)
    {
      string s = String.Join("\0", obj);
      return s.GetHashCode();
    }
  }
}

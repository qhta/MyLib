using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.TextUtils
{
  /// <summary>
  /// A handy implementation of string equality comparer.
  /// </summary>
  public class StringsEqualityComparer : IEqualityComparer<string[]>
  {
    StringComparison stringComparison = StringComparison.CurrentCulture;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public StringsEqualityComparer() { }

    /// <summary>
    /// Constructor with explicit comparison.
    /// </summary>
    /// <param name="comparison"></param>
    public StringsEqualityComparer(StringComparison comparison)
    {
      stringComparison = comparison;
    }

    /// <summary>
    /// Maint compare method.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(string[]? x, string[]? y)
    {
      if (x == null && y == null) return true;
      if (x == null || y == null) return false;
      if (x.Length != y.Length)
        return false;
      for (int i = 0; i < x.Length; i++)
        if (!(x[i].Equals(y[i], stringComparison)))
          return false;
      return true;
    }

    /// <summary>
    /// GetHashCode metod. Required when Equals defined.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetHashCode(string[] obj)
    {
      string s = String.Join("\0", obj);
      return s.GetHashCode();
    }
  }
}

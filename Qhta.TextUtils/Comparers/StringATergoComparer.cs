using System;
using System.Collections.Generic;
using System.Linq;

namespace Qhta.TextUtils
{
  /// <summary>
  /// Comparer for inverted strings.
  /// </summary>
  public class StringATergoComparer : IComparer<String>, IEqualityComparer<String>
  {
    HashSet<int> hashes = new HashSet<int>();

    /// <summary>
    /// Main compare method
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare(String? x, String? y)
    {
      if (x == null)
        return (y == null) ? 0 : -1;
      if (y == null) return 1;
      x = x.Reverse().ToString()??"";
      y = y.Reverse().ToString()??"";
      int result = x.CompareTo(y);
      if (result == 0)
        return 1;
      else
        return result;
    }

    /// <summary>
    /// Equality comparer never shows true.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool Equals(string? x, string? y)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// GetHashCode metod. Required when Equals defined.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public int GetHashCode(string? obj)
    {
      if (obj is string)
      {
        int hash = (obj as string).GetHashCode();
        while (hashes.Contains(hash))
          hash++;
        hashes.Add(hash);
        return hash;
      }
      throw new NotImplementedException();
    }

  }

}

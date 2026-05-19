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
    readonly HashSet<int> hashes = [];

    /// <summary>
    /// Main compare method
    /// </summary>
    /// <param name="str1">First string to compare</param>
    /// <param name="str2">Second string to compare</param>
    /// <returns></returns>
    public int Compare(String? str1, String? str2)
    {
      if (str1 == null)
        return (str2 == null) ? 0 : -1;
      if (str2 == null) return 1;
      str1 = new string(str1.Reverse().ToArray());
      str2 = new string(str2.Reverse().ToArray());
      int result = String.Compare(str1, str2, StringComparison.Ordinal);
      if (result == 0)
        return 1;
      else
        return result;
    }

    /// <summary>
    /// Equality comparer never shows true as Compare method returns 1 when strings are equal.
    /// </summary>
    /// <param name="str1">First string to compare</param>
    /// <param name="str2">Second string to compare</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool Equals(string? str1, string? str2)
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
      if (obj is string str)
      {
        int hash = str.GetHashCode();
        while (hashes.Contains(hash))
          hash++;
        hashes.Add(hash);
        return hash;
      }
      throw new NotImplementedException();
    }

  }

}

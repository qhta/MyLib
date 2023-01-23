using System;
using System.Collections.Generic;

namespace Qhta.TextUtils
{
  /// <summary>
  /// List sort comparer allowing duplicates. Normal comparer returns 0 if compared values are equal. This comparer returns 1 in this case.
  /// </summary>
  /// <typeparam name="T">Any comparable type (string type preferred)</typeparam>
  public class DuplicateAllowingCompararer<T> : IComparer<T>, IEqualityComparer<T> where T : IComparable
  {
    HashSet<int> hashes = new HashSet<int>();

    /// <summary>
    /// Main comparison method
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare(T? x, T? y)
    {
      if (x is null)
        return (y is null) ? 1 : -1;
      int result = x.CompareTo(y);
      if (result == 0)
        return 1;
      else
        return result;
    }

    /// <summary>
    /// Equality comparer newer shows true.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(T? x, T? y)
    {
      return false;
    }

    /// <summary>
    /// GetHashCode metod. Required when Equals defined.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public int GetHashCode(T obj)
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

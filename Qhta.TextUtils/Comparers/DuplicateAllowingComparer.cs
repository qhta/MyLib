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

    public int Compare(T x, T y)
    {
      int result = x.CompareTo(y);
      if (result == 0)
        return 1;
      else
        return result;
    }

    public bool Equals(T x, T y)
    {
      return false;
    }

    public int GetHashCode(T obj)
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

using System;
using System.Collections.Generic;

namespace Qhta.TextUtils
{
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

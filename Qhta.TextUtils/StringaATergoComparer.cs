using System;
using System.Collections.Generic;
using System.Linq;

namespace Qhta.TextUtils
{
  public class StringATergoComparer : IComparer<String>, IEqualityComparer<String>
  {
    HashSet<int> hashes = new HashSet<int>();

    public int Compare(String x, String y)
    {
      x = x.Reverse().ToString();
      y = y.Reverse().ToString();
      int result = x.CompareTo(y);
      if (result == 0)
        return 1;
      else
        return result;
    }

    public bool Equals(string x, string y)
    {
      throw new NotImplementedException();
    }

    public int GetHashCode(string obj)
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

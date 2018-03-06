using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.TextUtils
{
  public class StringATergoComparer : IComparer<String>, IEqualityComparer<String>
  {
    HashSet<int> hashes = new HashSet<int>();

    public int Compare(String x, String y)
    {
      int result = new String(x.Reverse().CompareTo(y.Reverse);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.TextUtils
{
  /// <summary>
  /// A list of methods operating on list of strings.
  /// </summary>
  public static class StringListUtils
  {

    /// <summary>
    /// Finds a string in a list.
    /// </summary>
    /// <param name="aList"></param>
    /// <param name="s"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int IndexOf(this List<string> aList, string s, StringComparison comparison = StringComparison.CurrentCulture)
    {
      for (int i = 0; i < aList.Count; i++)
        if (aList[i].Equals(s, comparison))
          return i;
      return -1;
    }

    /// <summary>
    /// Checks whether both lists are equal or both contain the same elements in different order.
    /// </summary>
    public static bool IsEqualOrMixed(this List<string> aList, List<string> bList, StringComparison comparison = StringComparison.CurrentCulture)
    {
      if (aList.Count != bList.Count)
        return false;
      foreach (string s in aList)
      {
        int k = bList.IndexOf(s, comparison);
        if (k >= 0)
          bList.RemoveAt(k);
        else
          return false;
      }
      return true;
    }

    /// <summary>
    /// Checks in which positions the first list contains elements not found in the second list.
    /// </summary>
    public static int[] FindInclusions(this List<string> aList, List<string> bList, StringComparison comparison = StringComparison.CurrentCulture)
    {
      List<int> result = new List<int>();
      for (int i = 0, j=0; i<aList.Count; i++,j++)
      {
        string aItem = aList[i];
        if (j < bList.Count)
        {
          string bItem = bList[j];
          if (!aItem.Equals(bItem, comparison))
          {
            result.Add(i);
            j--;
          }
        }
        else
          result.Add(i);
      }
      return result.ToArray();
    }
  }
}

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
    /// <param name="inputList"></param>
    extension(List<string> inputList)
    {
      /// <summary>
      /// Finds a string in a list.
      /// </summary>
      /// <param name="s"></param>
      /// <param name="comparison"></param>
      /// <returns></returns>
      public int IndexOf(string s, StringComparison comparison = StringComparison.CurrentCulture)
      {
        for (int i = 0; i < inputList.Count; i++)
          if (inputList[i].Equals(s, comparison))
            return i;
        return -1;
      }

      /// <summary>
      /// Checks whether both lists are equal or both contain the same elements in different order.
      /// </summary>
      public bool IsEqualOrMixed(List<string> bList, StringComparison comparison = StringComparison.CurrentCulture)
      {
        if (inputList.Count != bList.Count)
          return false;
        foreach (string s in inputList)
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
      public int[] FindInclusions(List<string> bList, StringComparison comparison = StringComparison.CurrentCulture)
      {
        List<int> result = new List<int>();
        for (int i = 0, j=0; i<inputList.Count; i++,j++)
        {
          string aItem = inputList[i];
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
}

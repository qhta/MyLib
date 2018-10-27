using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  struct SortInfo
  {
    public Binding Binding;
    public ListSortDirection SortDirection;
  }

  class Comparator : IComparer<object>
  {
    public Comparator(List<SortInfo> sortedBy)
    {
      SortedBy = sortedBy;
    }

    List<SortInfo> SortedBy;

    public int Compare(object x, object y)
    {
      foreach (var sortInfo in SortedBy)
      {
        var value1 = sortInfo.Binding.GetValue(x);
        var value2 = sortInfo.Binding.GetValue(y);
        if (value1 is IComparable cValue1 && value2 is IComparable)
        {
          var cmp = cValue1.CompareTo(value2);
          if (cmp!=0)
            return cmp;
        }
      }
      return 0;
    }
  }

}

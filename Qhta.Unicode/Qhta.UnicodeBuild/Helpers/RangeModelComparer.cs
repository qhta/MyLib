using System.Collections.Generic;

namespace Qhta.UnicodeBuild.Helpers
{
  public class RangeModelComparer : IComparer<object>
  {
    public int Compare(object? x, object? y)
    {
      if (x is RangeModel rangeX && y is RangeModel rangeY)
      {
        return rangeX.CompareTo(rangeY);
      }
      return 0;
    }
  }
}
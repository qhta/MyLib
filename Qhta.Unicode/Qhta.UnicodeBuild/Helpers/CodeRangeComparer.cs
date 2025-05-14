using System.Collections.Generic;

namespace Qhta.UnicodeBuild.Helpers
{
  public class CodeRangeComparer : IComparer<object>
  {
    public int Compare(object? x, object? y)
    {
      if (x is CodeRange rangeX && y is CodeRange rangeY)
      {
        return rangeX.CompareTo(rangeY);
      }
      return 0;
    }
  }
}
using System.Collections.Generic;

namespace Qhta.UnicodeBuild.Helpers
{
  /// <summary>
  /// Comparer for <see cref="CodeRange"/> objects.
  /// </summary>
  public class CodeRangeComparer : IComparer<object>
  {
    /// <summary>
    /// Method to compare two <see cref="CodeRange"/> objects.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
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
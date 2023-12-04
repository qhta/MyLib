namespace Qhta.WPF.Utils;

struct SortInfo
{
  public Binding Binding;
  public ListSortDirection SortDirection;
}

/// <summary>
/// Helper class to sort bindings.
/// </summary>
class Comparator : IComparer<object>
{
  public Comparator(List<SortInfo> sortedBy)
  {
    SortedBy = sortedBy;
  }

  List<SortInfo> SortedBy;

  /// <summary>
  /// Compares to values gets from object x and y using binding.
  /// </summary>
  /// <param name="x"></param>
  /// <param name="y"></param>
  /// <returns></returns>
  public int Compare(object? x, object? y)
  {
    if (x != null && y != null)
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

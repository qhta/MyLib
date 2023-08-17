namespace Qhta.WPF.Utils.ViewModels
{
  /// <summary>
  /// Specifies what to do with a column filter.
  /// </summary>
  public enum ColumnFilterOperation
  {
    /// <summary>
    /// Add a filter to collection filter.
    /// </summary>
    Add,

    /// <summary>
    /// Edit a filter and replace the old one in collection filter.
    /// </summary>
    Edit,

    /// <summary>
    /// Clear the filter and remove the old one from collection filter.
    /// </summary>
    Clear,

  }
}

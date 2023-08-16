namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// View model stored and shown in FilterDialog.
/// </summary>
public abstract class ColumnFilterViewModel: ViewModel
{
  /// <summary>
  /// Creates Predicate basing on current properties.
  /// </summary>
  /// <returns>Predicate that takes a property value from the object.</returns>
  public abstract DataGridColumnFilter? CreateFilter(PropertyInfo propInfo);
}

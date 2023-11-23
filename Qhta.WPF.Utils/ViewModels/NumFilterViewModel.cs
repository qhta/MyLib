namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// Abstract base ColumnFilterViewModel of numeric property filter edited in NumFilterView.
/// </summary>
public abstract class NumFilterViewModel : ColumnFilterViewModel
{

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  public NumFilterViewModel(PropPath propPath, string columnName) : base(propPath, columnName)
  { }

  /// <summary>
  /// Copying constructor (empty, just for pass specific class constructor).
  /// </summary>
  /// <returns></returns>
  public NumFilterViewModel(NumFilterViewModel other) : base(other)
  {
  }

}

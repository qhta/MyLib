namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// Abstract base ColumnFilterViewModel of numeric property filter edited in NumFilterView.
/// </summary>
public abstract class NumFilterViewModel : FilterViewModel
{

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  public NumFilterViewModel(PropPath propPath, string columnName, IObjectOwner? owner) : base(propPath, columnName, owner)
  { }

  /// <summary>
  /// Copying constructor (empty, just for pass specific class constructor).
  /// </summary>
  public NumFilterViewModel(NumFilterViewModel other) : base(other)
  {
  }

}

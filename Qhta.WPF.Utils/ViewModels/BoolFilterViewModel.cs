namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// specific ColumnFilterViewModel of boolean property filter edited in BoolFilterView.
/// </summary>
public class BoolFilterViewModel : FilterViewModel
{

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  public BoolFilterViewModel(PropPath propPath, string propName, IObjectOwner? owner) : base(propPath, propName, owner)
  { }

  /// <summary>
  /// Copying constructor.
  /// </summary>
  /// <returns></returns>
  public BoolFilterViewModel(BoolFilterViewModel other) : base(other)
  {
    this.Function = other.Function;
  }

  /// <summary>
  /// Creates a copy of this instance;
  /// </summary>
  /// <returns></returns>
  public override FilterViewModel CreateCopy()
  {
    return new BoolFilterViewModel(this);
  }

  /// <summary>
  /// This method copies properties from the other instance of the same type.
  /// </summary>
  public override void CopyFrom(FilterViewModel? other)
  {
    if (other is BoolFilterViewModel otherFilter)
    {
      this.Function = otherFilter.Function;;
    }
  }

  /// <summary>
  /// Clears the <see cref="Function"/> property.
  /// </summary>
  /// <exception cref="NotImplementedException"></exception>
  public override void ClearFilter()
  {
    Function = null;
  }

  /// <summary>
  /// Selected predicate function.
  /// </summary>
  public BoolPredicateFunction? Function
  {
    get { return _Function; }
    set
    {
      if (_Function != value)
      {
        _Function = value;
        NotifyFunctionChanged();
      }
    }
  }
  private BoolPredicateFunction? _Function = BoolPredicateFunction.IsTrue;

  private void NotifyFunctionChanged()
  {
    NotifyPropertyChanged(nameof(Function));
    foreach (var enumName in typeof(BoolPredicateFunction).GetEnumNames())
      NotifyPropertyChanged(enumName);
    NotifyPropertyChanged(nameof(CanCreateFilter));
    CommandManager.InvalidateRequerySuggested();
  }

  #region Individual boolean properties for Function used in RadioButton.

  /// <summary>
  /// Specifies whether predicate function is IsTrue.
  /// </summary>
  public bool IsTrue { get => Function == BoolPredicateFunction.IsTrue; set { if (value) Function = BoolPredicateFunction.IsTrue; } }

  /// <summary>
  /// Specifies whether predicate function is IsFalse.
  /// </summary>
  public bool IsFalse { get => Function == BoolPredicateFunction.IsFalse; set { if (value) Function = BoolPredicateFunction.IsFalse; } }

  /// <summary>
  /// Specifies whether predicate function is IsEmpty.
  /// </summary>
  public bool IsEmpty { get => Function == BoolPredicateFunction.IsEmpty; set { if (value) Function = BoolPredicateFunction.IsEmpty; } }

  /// <summary>
  /// Specifies whether predicate function is NotEmpty.
  /// </summary>
  public bool NotEmpty { get => Function == BoolPredicateFunction.NotEmpty; set { if (value) Function = BoolPredicateFunction.NotEmpty; } }

  #endregion

  /// <inheritdoc/>
  public override bool CanCreateFilter => Function!=null;

  /// <summary>
  /// Creates DataGridColumnFilter basing on current properties.
  /// </summary>
  /// <returns></returns>
  public override ColumnFilter? CreateFilter()
  {
    if (Function == null || PropPath == null)
      return null;
    Func<object?, object?, bool> compareFunction;
    switch (Function)
    {
      case BoolPredicateFunction.IsEmpty:
        compareFunction = IsEmptyFunction;
        break;
      case BoolPredicateFunction.NotEmpty:
        compareFunction = NotEmptyFunction;
        break;
      case BoolPredicateFunction.IsTrue:
        compareFunction = IsTrueFunction;
        break;
      case BoolPredicateFunction.IsFalse:
        compareFunction = IsFalseFunction;
        break;
      default:
        return null;
    }
    var dataGridColumnFilter = new ColumnFilter(PropPath, compareFunction);
    return dataGridColumnFilter;
  }

  private bool IsEmptyFunction(object? propValue, object? otherValue)
  {
    return propValue == null;
  }

  private bool NotEmptyFunction(object? propValue, object? otherValue)
  {
    return propValue != null;
  }

  private bool IsTrueFunction(object? propValue, object? otherValue)
  {
    if (propValue is bool boolValue)
      return boolValue == true;
    return false;
  }

  private bool IsFalseFunction(object? propValue, object? otherValue)
  {
    if (propValue is bool boolValue)
      return boolValue == false;
    return false;
  }

}

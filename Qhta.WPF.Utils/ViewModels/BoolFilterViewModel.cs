using System.Text.RegularExpressions;

namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// View model shown in TextFilterWindow.
/// </summary>
public class BoolFilterViewModel : ColumnFilterViewModel
{

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  public BoolFilterViewModel(PropertyInfo propInfo, string propName): base(propInfo, propName)
    { }

  /// <summary>
  /// Copying constructor.
  /// </summary>
  /// <returns></returns>
  public BoolFilterViewModel(BoolFilterViewModel other): base(other)
  {
    this.Function = other.Function;
  }

  /// <summary>
  /// Creates a copy of this instance;
  /// </summary>
  /// <returns></returns>
  public override ColumnFilterViewModel CreateCopy()
  {
    return new BoolFilterViewModel(this);
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
  private BoolPredicateFunction? _Function;

  private void NotifyFunctionChanged()
  {
    NotifyPropertyChanged(nameof(Function));
    foreach (var enumName in typeof(BoolPredicateFunction).GetEnumNames())
      NotifyPropertyChanged(enumName);
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

  /// <summary>
  /// Creates DataGridColumnFilter basing on current properties.
  /// </summary>
  /// <returns></returns>
  public override ColumnFilter? CreateFilter()
  {
    if (Function == null)
      return null;
    var dataGridColumnFilter = new ColumnFilter(PropInfo);
    switch (Function)
    {
      case BoolPredicateFunction.IsEmpty:
        dataGridColumnFilter.CompareFunction = IsEmptyFunction;
        break;
      case BoolPredicateFunction.NotEmpty:
        dataGridColumnFilter.CompareFunction = NotEmptyFunction;
        break;
      case BoolPredicateFunction.IsTrue:
        dataGridColumnFilter.CompareFunction = IsTrueFunction;
        break;
      case BoolPredicateFunction.IsFalse:
        dataGridColumnFilter.CompareFunction = IsFalseFunction;
        break;
      default:
        return null;
    }
    dataGridColumnFilter.Predicate =
      new Predicate<object>(obj =>
      {
        var value = dataGridColumnFilter.PropertyInfo.GetValue(obj, null);
        return dataGridColumnFilter.CompareFunction(value, dataGridColumnFilter.OtherValue);
      });
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

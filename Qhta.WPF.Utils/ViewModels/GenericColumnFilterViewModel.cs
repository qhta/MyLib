using System.CodeDom;

namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// Generic column filter view model that enables user to change data type
/// </summary>
public class GenericColumnFilterViewModel : ColumnFilterViewModel
{

  /// <summary>
  /// Initializing constructor
  /// </summary>
  /// <param name="propPath"></param>
  /// <param name="columnName"></param>
  public GenericColumnFilterViewModel(PropPath propPath, string columnName) : base(propPath, columnName)
  {
    PropertyChanged += GenericColumnFilterViewModel_PropertyChanged;
    SpecificFilter = CreateCopy();
  }

  /// <summary>
  /// Initializing constructor
  /// </summary>
  /// <param name="columnFilter"></param>
  /// <param name="columnName"></param>
  public GenericColumnFilterViewModel(ColumnFilter columnFilter, string columnName) : base(columnFilter.PropPath, columnName)
  {
    PropertyChanged += GenericColumnFilterViewModel_PropertyChanged;
    SpecificFilter = CreateCopy();
  }

  /// <summary>
  /// Copying constructor that copies the data of the view model is needed
  /// because we need to edit its copy and after Cancel button is pressed
  /// the previous content remains unchanged.
  /// </summary>
  public GenericColumnFilterViewModel(ColumnFilterViewModel other, string columnName) : base(other)
  {
    PropPath = other.PropPath;
    ColumnName = columnName;
    PropertyChanged += GenericColumnFilterViewModel_PropertyChanged;
  }


  private void GenericColumnFilterViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(PropPath))
    {
      if (PropPath != null)
        SpecificFilter = CreateCopy();
    }
  }

  /// <summary>
  /// Data type of selected property.
  /// </summary>
  public Type? DataType
  {
    get
    {
      var type = PropPath?.Last().PropertyType;
      return type;
    }
  }

  /// <summary>
  /// Specific filter which is changed on dataType change.
  /// </summary>

  public ColumnFilterViewModel? SpecificFilter
  {
    get { return _SpecificFilter; }
    set
    {
      if (_SpecificFilter != value)
      {
        _SpecificFilter = value;
        NotifyPropertyChanged(nameof(SpecificFilter));
        if (value!=null)
          Instance = value;
      }
    }
  }
  private ColumnFilterViewModel? _SpecificFilter; 

  /// <summary>
  /// Implements abstract CreateCopy
  /// </summary>
  /// <returns></returns>
  public override ColumnFilterViewModel CreateCopy()
  {
    if (PropPath != null && ColumnName != null)
      return GenericColumnFilterViewModel.CreateCopy(PropPath, ColumnName, DataType ?? typeof(object));
    throw new InvalidOperationException($"Cant copy {GetType()} filterViewModel");
  }

  /// <summary>
  /// Creates a specific dataType Filter.
  /// </summary>
  /// <param name="propPath"></param>
  /// <param name="columnName"></param>
  /// <param name="dataType"></param>
  /// <returns></returns>
  public static ColumnFilterViewModel CreateCopy(PropPath propPath, string columnName, Type dataType)
  {
    if (dataType == typeof(string))
      return new TextFilterViewModel(propPath, columnName);
    else
      if (dataType == typeof(bool))
      return new BoolFilterViewModel(propPath, columnName);
    else
      if (dataType.IsEnum == true)
      return new EnumFilterViewModel(dataType, propPath, columnName);
    else
      if (dataType == typeof(int))
      return new NumFilterViewModel<int>(propPath, columnName);
    else
      if (dataType == typeof(uint))
      return new NumFilterViewModel<uint>(propPath, columnName);
    else
      if (dataType == typeof(byte))
      return new NumFilterViewModel<byte>(propPath, columnName);
    else
      if (dataType == typeof(sbyte))
      return new NumFilterViewModel<sbyte>(propPath, columnName);
    else
      if (dataType == typeof(Int16))
      return new NumFilterViewModel<Int16>(propPath, columnName);
    else
      if (dataType == typeof(UInt16))
      return new NumFilterViewModel<UInt16>(propPath, columnName);
    else
      if (dataType == typeof(Int64))
      return new NumFilterViewModel<Int64>(propPath, columnName);
    else
      if (dataType == typeof(UInt64))
      return new NumFilterViewModel<UInt64>(propPath, columnName);
    else
      if (dataType == typeof(Single))
      return new NumFilterViewModel<Single>(propPath, columnName);
    else
      if (dataType == typeof(Double))
      return new NumFilterViewModel<Double>(propPath, columnName);
    else
      if (dataType == typeof(Decimal))
      return new NumFilterViewModel<Decimal>(propPath, columnName);
    else
      if (dataType == typeof(DateTime))
      return new NumFilterViewModel<DateTime>(propPath, columnName);
    else
      if (dataType == typeof(TimeSpan))
      return new NumFilterViewModel<TimeSpan>(propPath, columnName);
    else
      return new ObjFilterViewModel(propPath, columnName);
  }

  /// <summary>
  /// Creates a specific ColumnFiltere.
  /// </summary>
  /// <returns></returns>
  public override ColumnFilter? CreateFilter()
  {
    if (SpecificFilter != null)
      return SpecificFilter.CreateFilter();
    throw new InvalidOperationException($"Cant create filter in {GetType()}");
  }

  /// <summary>
  /// Clears the specific filter
  /// </summary>
  public override void ClearFilter()
  {
    if (SpecificFilter != null)
      SpecificFilter.ClearFilter();
  }
}

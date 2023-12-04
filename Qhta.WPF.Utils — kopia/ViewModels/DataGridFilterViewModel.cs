
namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// ViewModel for filter of DataGrid
/// </summary>
public class DataGridFilterViewModel : FilterViewModel, IObjectOwner
{
  /// <summary>
  /// Initializing constructor
  /// </summary>
  public DataGridFilterViewModel(DataGrid dataGrid) : base()
  {
    TargetControl = dataGrid;
    ApplyFilterCommand = new RelayCommand<object>(ApplyFilterExecute, ApplyFilterCanExecute) { Name = "ApplyFilterCommand" };
    ClearFilterCommand = new RelayCommand<object>(ClearFilterExecute, ClearFilterCanExecute) { Name = "ClearFilterCommand" };
  }

  /// <summary>
  /// Control to which this object is assigned.
  /// </summary>
  public DataGrid TargetControl { get; private set; }

  /// <summary>
  /// Result of the FilterDialog
  /// </summary>
  public FilterResultOperation DialogResult { get; set; }

  /// <summary>
  /// Names of columns to select.
  /// </summary>
  public IEnumerable<string>? ColumnNames => Columns?.Select(item => item.ColumnName);


  /// <summary>
  /// This view model is not edited
  /// but its component filter is the edited instance.
  /// </summary>
  public override FilterViewModel? EditedInstance
  {
    get { return _EditedInstance; }
    set
    {
      if (_EditedInstance != value)
      {
        if (_EditedInstance!=null)
          _EditedInstance.PropertyChanged -= _EditedInstance_PropertyChanged;
        _EditedInstance = value;
        if (_EditedInstance!=null)
          _EditedInstance.PropertyChanged += _EditedInstance_PropertyChanged;
        NotifyPropertyChanged(nameof(EditedInstance));
      }
    }
  }

  private void _EditedInstance_PropertyChanged(object? sender, PropertyChangedEventArgs args)
  {
    if (args.PropertyName==nameof(CanCreateFilter))
    {
      //ApplyFilterCommand.NotifyCanExecuteChanged();
      CommandManager.InvalidateRequerySuggested();
    }
  }

  private FilterViewModel? _EditedInstance;

  #region IObjectOwner implementation
  /// <summary>
  /// Returns the Filter component.
  /// </summary>
  public object? GetComponent(string propName)
  {
    if (propName != nameof(EditedInstance))
      throw new InvalidOperationException($"You can get only {nameof(EditedInstance)} component");
    return EditedInstance;
  }

  /// <summary>
  /// Changes the Filter component to new instance.
  /// </summary>
  public bool ChangeComponent(string propName, object? newComponent)
  {
    if (newComponent is FilterViewModel newFilter)
      EditedInstance = newFilter;
    else
    if (newComponent == null)
      EditedInstance = null;
    else
      throw new InvalidOperationException($"New Filter object must be a {nameof(FilterViewModel)}");
    return true;
  }

  /// <summary>
  /// Changes the Instance component to new instance.
  /// </summary>
  public bool ChangeComponent(object? oldComponent, object? newComponent)
  {
    if (newComponent is FilterViewModel newFilter)
      EditedInstance = newFilter;
    else
    if (newComponent == null)
      EditedInstance = null;
    else
      throw new InvalidOperationException($"New Filter object must be a {nameof(FilterViewModel)}");
    return true;
  }
  #endregion

  #region FilterViewModel implementation
  /// <inheritdoc/>
  public override FilterViewModel? CreateCopy()
  {
    return EditedInstance?.CreateCopy();
  }

  /// <inheritdoc/>
  public override bool CanCreateFilter
  {
    get
    {
      return EditedInstance?.CanCreateFilter==true;
    }
  }

  /// <inheritdoc/>
  public override IFilter? CreateFilter()
  {
    return EditedInstance?.CreateFilter();
  }

  /// <inheritdoc/>
  public override void ClearFilter()
  {
    throw new NotImplementedException();
  }
  #endregion

  #region ApplyFilterCommand

  /// <summary>
  /// This command applies the edited instance filter to the owner data grid.
  /// </summary>
  public Command ApplyFilterCommand { get; private set; }

  /// <summary>
  ///Editable instance must be able to get the filter
  /// </summary>
  /// <returns></returns>
  protected virtual bool ApplyFilterCanExecute(object? parameter)
  {
    return EditedInstance?.CanCreateFilter==true;
  }

  /// <summary>
  /// Creates a new compound filter and adds this to it.
  /// In current owner changes edited instance to the compound filter.
  /// Also creates a new generic column filter and adds it to the owner filter as next operand.
  /// </summary>
  protected virtual void ApplyFilterExecute(object? parameter)
  {
    var filter = EditedInstance?.CreateFilter();
    if (filter != null) 
    {
      CollectionViewBehavior.SetCollectionFilter(TargetControl, filter);
    }
  }
  #endregion

  #region ClearFilterCommand

  /// <summary>
  /// This command removes this filter from owner AND/OR filter.
  /// </summary>
  public Command ClearFilterCommand { get; private set; }

  /// <summary>
  /// Can execute only if has owner compound filter.
  /// </summary>
  /// <returns></returns>
  protected virtual bool ClearFilterCanExecute(object? parameter)
  {
    return Owner is CompoundFilterViewModel;
  }

  /// <summary>
  /// Clears this filter from owner compound filter.
  /// </summary>
  protected virtual void ClearFilterExecute(object? parameter)
  {
    throw new NotImplementedException();
  }
  #endregion

}

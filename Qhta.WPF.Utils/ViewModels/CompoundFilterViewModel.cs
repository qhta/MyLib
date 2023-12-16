namespace Qhta.WPF.Utils.ViewModels;
/// <summary>
/// A compound filter which can contain many subfilters and evaluate their and/or aggregate function.
/// </summary>
public class CompoundFilterViewModel : FilterViewModel, IObjectOwner
{
  /// <summary>
  /// Default constructor.
  /// </summary>
  /// <param name="owner"></param>
  public CompoundFilterViewModel(IObjectOwner? owner) : base(owner)
  {
    AddNextFilterCommand = new RelayCommand<object>(AddNextFilterExecute, AddNextFilterCanExecute) { Name = "AddNextFilterCommand" };
  }


  /// <summary>
  /// Specifies what to do with compound filters.
  /// </summary>
  public BooleanOperation? Operation
  {
    get { return _Operation; }
    set
    {
      if (_Operation != value)
      {
        _Operation = value;
        NotifyPropertyChanged(nameof(Operation));
        NotifyPropertyChanged(nameof(CanCreateFilter));
      }
    }
  }
  private BooleanOperation? _Operation;

  /// <summary>
  /// Operations to select
  /// </summary>
  public Dictionary<string, BooleanOperation> Operations { get; private set; }
    = new Dictionary<string, BooleanOperation>()
  {
    { CommonStrings.And, BooleanOperation.And },
    { CommonStrings.Or, BooleanOperation.Or },
  };

  /// <summary>
  /// Component filters.
  /// </summary>
  public ObservableCollection<FilterViewModel> Items { get; private set; } = new ObservableCollection<FilterViewModel>();

  /// <summary>
  /// Adds a component filter;
  /// </summary>
  /// <param name="item"></param>
  public void Add(FilterViewModel item)
  {
    Items.Add(item);
    item.Owner = this;
    Owner?.ChangeComponent(EditedInstance, this);
  }

  /// <summary>
  /// Removes a component filter;
  /// </summary>
  /// <param name="item"></param>
  public bool Remove(FilterViewModel item)
  {
    var result = Items.Remove(item);
    return result;
  }

  /// <inheritdoc/>
  public override FilterViewModel CreateCopy()
  {
    var other = new CompoundFilterViewModel(Owner);
    other.Operation = this.Operation;
    foreach (var item in Items)
    {
      var newItem = item.CreateCopy();
      if (newItem != null)
        other.Add(newItem);
    }
    return other;
  }

  /// <summary>
  /// This method copies properties from the other instance of the same type.
  /// </summary>
  public override void CopyFrom(FilterViewModel? other)
  {
    if (other is CompoundFilterViewModel otherFilter)
    {
      this.Operation = otherFilter.Operation;
      this.Items = otherFilter.Items;
    }
  }

  /// <summary>
  /// Gets a collection of filtered columns in the EditedInstance.
  /// </summary>
  public override ColumnsViewInfo? GetFilteredColumns()
  {
    if (Items.Count == 0) return null;

    var columns = new ColumnsViewInfo();
    foreach (var item in Items)
    {
      var itemColumns = item.GetFilteredColumns();
      if (itemColumns != null)
        foreach (var column in itemColumns)
          if (!columns.Contains(column))
            columns.Add(column);
    }
    return columns;
  }

  /// <inheritdoc/>
  public override bool ContainsColumn(ColumnViewInfo column)
  {
    foreach (var item in Items)
      if (item.ContainsColumn(column))
        return true;
    return false;
  }

  /// <inheritdoc/>
  public override bool CanCreateFilter => Operation != null;

  /// <inheritdoc/>
  public override IFilter? CreateFilter()
  {
    if (Operation == null)
      return null;
    var result = new CompoundFilter((BooleanOperation)Operation);
    foreach (var item in Items)
    {
      var itemFilter = item.CreateFilter();
      if (itemFilter != null)
        result.Items.Add(itemFilter);
    }
    return result;
  }

  /// <inheritdoc/>
  public override void ClearFilter()
  {
    //throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public object? GetComponent(string propName)
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public bool ChangeComponent(string propName, object? newComponent)
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public bool ChangeComponent(object? oldComponent, object? newComponent)
  {
    if (newComponent is FilterViewModel newFilter)
    {
      for (int i = 0; i < Items.Count; i++)
      {
        if (Items[i] == oldComponent)
        {
          Items[i] = newFilter;
          return true;
        }
      }
    }
    return false;
  }

  #region AddNextFilter

  /// <summary>
  /// This command adds AND/OR filter above this filter.
  /// </summary>
  public Command AddNextFilterCommand { get; private set; }

  /// <summary>
  /// In theory can always execute, but must check owner (and parameter).
  /// </summary>
  /// <returns></returns>
  protected virtual bool AddNextFilterCanExecute(object? parameter)
  {
    return true;
  }

  /// <summary>
  /// Creates a new compound filter and adds this to it.
  /// In current owner changes edited instance to the compound filter.
  /// Also creates a new generic column filter and adds it to the owner filter as next operand.
  /// </summary>
  protected virtual void AddNextFilterExecute(object? parameter)
  {

    if (Column != null)
    {
      var nextOp = new GenericColumnFilterViewModel(Column, this);
      this.Add(nextOp);
    }
  }
  #endregion

  /// <inheritdoc/>
  public override string? ToString()
  {
    return $"{Operation}({(String.Join(", ", Items.Select(item=>item.ToString())))})";
  }
}

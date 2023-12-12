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
    foreach (var item in Items)
    {
      var newItem = item.CreateCopy();
      if (newItem != null)
        other.Items.Add(newItem);
    }
    other.Operation = this.Operation;
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
  public override FilterableColumns? GetFilteredColumns()
  {
    if (Items.Count == 0) return null;

    var columns = new FilterableColumns();
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
  public override bool CanCreateFilter => Operation!=null;

  /// <inheritdoc/>
  public override IFilter? CreateFilter()
  {
    if (Operation == null)
      return null;
    var result = new CompoundFilter((BooleanOperation)Operation);
    foreach(var item in Items)
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
    //if (newComponent is FilterViewModel newFilter)
    //{
    //  for (int i = 0; i < Items.Count; i++)
    //  {
    //    if (Items[i] == oldComponent)
    //    {
    //      Items[i] = newFilter;
    //      return true;
    //    }
    //  }
    //}
    return false;
  }

}

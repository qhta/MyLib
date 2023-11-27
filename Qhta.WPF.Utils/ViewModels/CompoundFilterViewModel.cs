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
    Items.CollectionChanged += Items_CollectionChanged;
  }

  private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
  {
    //Debug.WriteLine($"{e.Action}");
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

  /// <inheritdoc/>
  public override FilterViewModel CreateCopy()
  {
    var other = new CompoundFilterViewModel(Owner);
    foreach (var item in Items)
    {
      var newItem = item.CreateCopy();
      other.Items.Add(newItem);
    }
    other.Operation = this.Operation;
    return other;
  }

  /// <inheritdoc/>
  public override IFilter? CreateFilter()
  {
    if (Operation is not null)
      return new CompoundFilter((BooleanOperation)Operation);
    return null;
  }

  /// <inheritdoc/>
  public override void ClearFilter()
  {
    throw new NotImplementedException();
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
}

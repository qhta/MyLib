namespace Qhta.MVVM;

/// <summary>
/// Collection of objects that serves as a filter for ObservableList collection.
/// </summary>
public class FilteredCollection<T> : ObservableCollectionObject, IFiltered,
    IEnumerable,
    //ICollection,
    //IList,
    IEnumerable<T>,
    ICollection<T>,
    //IList<T>,
    INotifyCollectionChanged, INotifyPropertyChanged

{
  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="sourceCollection"></param>
  public FilteredCollection(ObservableList<T> sourceCollection)
  {
    SourceCollection = sourceCollection;
    SourceCollection.CollectionChanged += SourceCollection_CollectionChanged;
  }

  /// <summary>
  /// Collection that serves as an original source of items.
  /// </summary>
  public ObservableList<T> SourceCollection { get; private set; }


  /// <summary>
  /// Checks whether the filter is active.
  /// </summary>
  public bool IsFiltered
  {
    get { return Filter != null && _IsFiltered; }
    set
    {
      if (_IsFiltered != value)
      {
        _IsFiltered = value;
        NotifyPropertyChanged(nameof(IsFiltered));
      }
    }
  }
  private bool _IsFiltered;

  Predicate<object>? IFiltered.Filter
  {
    get => _filter as Predicate<object>;
    set
    {
      _ObjectFilter = value;
      if (_ObjectFilter != null)
        this.Filter = new Predicate<T>(item => item != null && _ObjectFilter(item));
      else
        this.Filter = null;
    }
  }

  private Predicate<object>? _ObjectFilter;

  /// <summary>
  /// Items qualifier
  /// </summary>
  public Predicate<T>? Filter
  {
    get => _filter;
    set
    {
      if (value != _filter)
      {
        _filter = value;
        NotifyPropertyChanged(nameof(Filter));
        ApplyFilter();
      }
    }
  }
  private Predicate<T>? _filter;

  /// <summary>
  /// If <see cref="IsFiltered"/> then passes Reset change notification.
  /// Otherwise passes the original notification.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  private void SourceCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    if (IsFiltered)
      NotifyCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    else
      NotifyCollectionChanged(this, e);
  }

  /// <summary>
  /// Apply filter to all source items.
  /// </summary>
  public void ApplyFilter()
  {
    IsFiltered = Filter != null;
    NotifyCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
  }

  /// <summary>
  /// Enumerates filtered items if <see cref="IsFiltered"/>.
  /// Otherwise enumerates original items.
  /// </summary>
  /// <returns></returns>
  public IEnumerator<T> GetEnumerator()
  {
    if (_IsFiltered && Filter != null)
      return SourceCollection.Where(item => Filter(item)).GetEnumerator();
    else
      return SourceCollection.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return this.GetEnumerator();
  }

  /// <summary>
  /// Adds an item to the source collection, but first check the predicate 
  /// and throw an exception if the item does not meet the filter condition.
  /// </summary>
  /// <param name="item"></param>
  public void Add(T item)
  {
    if (_IsFiltered && Filter != null)
      if (item != null && !Filter(item))
        throw new InvalidOperationException($"Item {item} does not meet the filter condition");
    SourceCollection.Add(item);
  }

  /// <summary>
  /// if <see cref="IsFiltered"/> then removes the filtered items from the source collection.
  /// Otherwise clears the source collection.
  /// </summary>
  public void Clear()
  {
    if (_IsFiltered && Filter != null)
      SourceCollection.RemoveAll(Filter);
    else
      SourceCollection.Clear();
  }

  /// <summary>
  /// if <see cref="IsFiltered"/> then checks whether filtered items contains an item.
  /// Otherwise checks whether the source collection contains an item.
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  public bool Contains(T item)
  {
    if (_IsFiltered && Filter != null)
      return ((IEnumerable<T>)this).Any(item => Filter(item));
    return SourceCollection.Contains(item);
  }

  /// <summary>
  /// if <see cref="IsFiltered"/> then copies filtered items to the array.
  /// Otherwise copies the source collection items.
  /// </summary>
  /// <param name="array"></param>
  /// <param name="arrayIndex"></param>
  public void CopyTo(T[] array, int arrayIndex)
  {
    if (IsFiltered)
      ((IEnumerable<T>)this).ToArray().CopyTo(array, arrayIndex);
    else
      SourceCollection.CopyTo(array, arrayIndex);
  }

  /// <summary>
  /// Removes the item from the source collection.
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  public bool Remove(T item)
  {
    return SourceCollection.Remove(item);
  }

  /// <summary>
  /// if <see cref="IsFiltered"/> then returns filtered items count.
  /// Otherwise returns the source collection count.
  /// </summary>
  public int Count
  {
    get
    {
      if (IsFiltered)
        return ((IEnumerable<T>)this).Count();
      else
        return SourceCollection.Count;
    }
  }

  /// <summary>
  /// Get/sets is read-only. 
  /// If is not set explicitily, then checks if the source collection is read-only.
  /// </summary>
  public bool IsReadOnly
  {
    get => _isReadOnly ?? SourceCollection.IsReadOnly;
    set => _isReadOnly = value;
  }
  private bool? _isReadOnly;
}

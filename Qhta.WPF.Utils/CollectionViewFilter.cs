namespace Qhta.WPF.Utils;

/// <summary>
/// Prepared filter for a collection view.
/// </summary>
public class CollectionViewFilter
{
  /// <summary>
  /// Internal storage of column filters. 
  /// The order of filtering can be different than the order of columns.
  /// </summary>
  private Dictionary<string, ColumnFilter> Filters = new Dictionary<string, ColumnFilter>();

  /// <summary>
  /// Default constructor. Initialized Predicate.
  /// </summary>
  public CollectionViewFilter()
  {
    Predicate = new Predicate<object>((object item) =>
    {
      foreach (ColumnFilter filter in Filters.Values)
      {
        if (filter.Predicate.Invoke(item) == false)
          return false;
      }
      return true;
    });
  }

  /// <summary>
  /// Qualifier function for the whole object.
  /// </summary>
  public Predicate<object> Predicate { get; private set; }

  /// <summary>
  /// Checks if there is no column filter.
  /// </summary>
  public bool IsEmpty()
  {
    return Filters.Count == 0;
  }

  /// <summary>
  /// Checks if the filter for this property name already exits.
  /// </summary>
  /// <param name="propName"></param>
  public bool ContainsFilter(string propName)
  {
    return Filters.ContainsKey(propName);
  }

  /// <summary>
  /// Adds a column filter to collection view filter.
  /// </summary>
  /// <param name="propName"></param>
  /// <param name="filter"></param>
  public void AddFilter(string propName, ColumnFilter filter)
  {
    Filters.Add(propName, filter);
  }

  /// <summary>
  /// Changes a column filter to another one.
  /// </summary>
  /// <param name="propName"></param>
  /// <param name="filter"></param>
  public void ChangeFilter(string propName, ColumnFilter filter)
  {
    Filters[propName] = filter;
  }

  /// <summary>
  /// Removes a column filter from collection view filter.
  /// </summary>
  /// <param name="propName"></param>
  public void RemoveFilter(string propName)
  {
    Filters.Remove(propName);
  }

  /// <summary>
  /// Adds, changes or removes filter for a column.
  /// Returns predicate or null (if is empty)
  /// </summary>
  /// <param name="propName"></param>
  /// <param name="filter"></param>
  public Predicate<object>? ApplyFilter(string propName, ColumnFilter? filter)
  {
    if (filter!=null)
    {
    if (ContainsFilter(propName))
      ChangeFilter(propName, filter);
    else
      AddFilter(propName, filter);
    }
    else
      RemoveFilter(propName);
    if (IsEmpty())
      return null;
    return Predicate;
  }
}

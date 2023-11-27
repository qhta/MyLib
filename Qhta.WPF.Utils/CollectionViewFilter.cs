using System.Collections.Immutable;

namespace Qhta.WPF.Utils;

/// <summary>
/// Prepared filter for a collection view. This is an immutable object.
/// </summary>
public class CollectionViewFilter: IFilter
{
  /// <summary>
  /// Internal storage of column filters. 
  /// The order of filtering can be different than the order of columns.
  /// </summary>
  public ImmutableDictionary<string, IFilter> Filters { get; private set; }

  /// <summary>
  /// Default constructor. Initialized always true Predicate.
  /// </summary>
  public CollectionViewFilter()
  {
    Filters = ImmutableDictionary<string, IFilter>.Empty;
    Predicate = new Predicate<object>((object item) => true);
  }

  /// <summary>
  /// Copyint constructor. Initialized Filters and Predicate
  /// </summary>
  public CollectionViewFilter(ImmutableDictionary<string, IFilter> filters)
  {
    Filters = filters;
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
  /// Implementation of IFilter.
  /// </summary>
  /// <returns></returns>
  public Predicate<object> GetPredicate() => Predicate;

  /// <summary>
  /// Implementation of IFilter.
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  public bool Accept(object item) => Predicate.Invoke(item);

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
  /// Adds a column filter to collection view filter. Returns new instance of Filters.
  /// </summary>
  /// <param name="propName"></param>
  /// <param name="filter"></param>
  public ImmutableDictionary<string, IFilter> AddFilter(string propName, IFilter filter)
  {
    return Filters.Add(propName, filter);
  }

  /// <summary>
  /// Changes a column filter to another one. Returns new instance of Filters.
  /// </summary>
  /// <param name="propName"></param>
  /// <param name="filter"></param>
  public ImmutableDictionary<string, IFilter> ChangeFilter(string propName, IFilter filter)
  {
    return Filters.SetItem(propName, filter);
  }

  /// <summary>
  /// Removes a column filter from collection view filter. Returns new instance of Filters.
  /// </summary>
  /// <param name="propName"></param>
  public ImmutableDictionary<string, IFilter> RemoveFilter(string propName)
  {
    return Filters.Remove(propName);
  }

  /// <summary>
  /// Adds, changes or removes filter for a column and returns the new instance of the class.
  /// </summary>
  /// <param name="propName"></param>
  /// <param name="filter"></param>
  public CollectionViewFilter ApplyFilter(string propName, IFilter? filter)
  {
    if (filter != null)
    {
      if (ContainsFilter(propName))
        return new CollectionViewFilter(ChangeFilter(propName, filter));
      else
        return new CollectionViewFilter(AddFilter(propName, filter));
    }
    else
      return new CollectionViewFilter(RemoveFilter(propName));
  }
}

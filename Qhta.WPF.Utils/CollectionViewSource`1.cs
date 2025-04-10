﻿
namespace Qhta.WPF.Utils;

/// <summary>
/// CollectionViewSource specialized for a specific item type.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CollectionViewSource<T> : System.Windows.Data.CollectionViewSource, IEnumerable<T>, IFilteredCollection<T>
{
  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="sourceCollection"></param>
  public CollectionViewSource(IEnumerable<T> sourceCollection)
  {
    SourceCollection = sourceCollection;
  }

  /// <summary>
  /// Represents the source collection passed in constructor.
  /// </summary>
  public IEnumerable<T> SourceCollection { get; private set; }

  /// <summary>
  /// Invokes base GetDefaultView with SourceCollection.
  /// </summary>
  /// <returns></returns>
  public ICollectionView GetDefaultView(IEnumerable<T> sourceCollection)
  {
    var result = System.Windows.Data.CollectionViewSource.GetDefaultView(sourceCollection);
    return result;
  }

  /// <summary>
  /// Implemented IFiltered property.
  /// </summary>
  public bool IsFiltered { get; set; }

  /// <summary>
  /// Implemented IFiltered predicate.
  /// </summary>
  public new IFilter<T>? Filter { get; set; }

  /// <summary>
  /// Enumerator on source collection
  /// </summary>
  /// <returns></returns>
  public IEnumerator<T> GetEnumerator()
  {
    return SourceCollection.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return ((IEnumerable)SourceCollection).GetEnumerator();
  }
}

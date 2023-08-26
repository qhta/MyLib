namespace Qhta.MVVM
{
  /// <summary>
  /// Interface for collection that can filter items of other collection.
  /// </summary>
  public interface IFilteredCollection<T>
  {
    /// <summary>
    /// Is collection filtered.
    /// </summary>
    bool IsFiltered { get; set; }

    /// <summary>
    /// Predicate to filter Items.
    /// </summary>
    IFilter<T>? Filter { get; set; }
  }
}

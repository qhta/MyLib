namespace Qhta.MVVM
{
  /// <summary>
  /// Interface for collection that can filter items of other collection.
  /// </summary>
  public interface IFilteredCollection
  {
    /// <summary>
    /// Is collection filtered.
    /// </summary>
    bool IsFiltered { get; set; }

    /// <summary>
    /// Predicate to filter Items.
    /// </summary>
    IFilter? Filter { get; set; }
  }
}

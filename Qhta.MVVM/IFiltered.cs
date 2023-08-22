namespace Qhta.MVVM
{
  /// <summary>
  /// Interface for collection that can filter items of other collection.
  /// </summary>
  public interface IFiltered
  {
    /// <summary>
    /// Is collection filtered.
    /// </summary>
    bool IsFiltered { get; set; }

    /// <summary>
    /// Predicate to filter Items.
    /// </summary>
    Predicate<object>? Filter { get; set; }
  }
}

namespace Qhta.MVVM
{
  /// <summary>
  /// Interface for an object that has an <see cref="IsFiltered"/> property.
  /// </summary>
  public interface IFiltered
  {
    /// <summary>
    /// A number of the item.
    /// </summary>
    bool IsFiltered { get; set; }
  }
}

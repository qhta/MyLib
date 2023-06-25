namespace Qhta.MVVM
{
  /// <summary>
  /// Interface for an object that has a <see cref="Number"/> property.
  /// </summary>
  public interface INumbered
  {
    /// <summary>
    /// A number of the item.
    /// </summary>
    int Number { get; set; }
  }
}

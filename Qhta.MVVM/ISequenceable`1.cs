namespace Qhta.MVVM
{
  /// <summary>
  /// Interface for a collection that can go to next or previous object.
  /// </summary>
  /// <typeparam name="ItemType"></typeparam>
  public interface ISequenceable<ItemType> where ItemType: class
  {
    /// <summary>
    /// Next object.
    /// </summary>
    ItemType Next { get; set; }
    /// <summary>
    /// Previous object.
    /// </summary>
    ItemType Prior { get; set; }
  }
}

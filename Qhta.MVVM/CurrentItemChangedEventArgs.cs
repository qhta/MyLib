namespace Qhta.MVVM
{
  /// <summary>
  /// <see cref="EventArgs"/> to notify that a current item in a list has changed.
  /// </summary>
  public class CurrentItemChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Constructor to set an old and a new item.
    /// </summary>
    /// <param name="newItem"></param>
    /// <param name="oldItem"></param>
    public CurrentItemChangedEventArgs(object? newItem, object? oldItem)
    {
      NewItem = newItem;
      OldItem = oldItem;
    }

    /// <summary>
    /// New current item.
    /// </summary>
    public object? NewItem { get; private set; }

    /// <summary>
    /// Old current item.
    /// </summary>
    public object? OldItem { get; private set; }
  }
}

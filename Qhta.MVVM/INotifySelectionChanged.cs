namespace Qhta.MVVM
{
  /// <summary>
  /// Interface that declares <see cref="SelectionChanged"/> event.
  /// </summary>
  public interface INotifySelectionChanged
  {
    /// <summary>
    /// An event to notify that selection has been changed.
    /// </summary>
    event SelectionChangedEventHandler SelectionChanged;
  }
}

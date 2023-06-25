using System;
using System.Collections;

namespace Qhta.MVVM
{
  /// <summary>
  /// <see cref="EventArgs"/> to notify that a selection has been changed.
  /// </summary>
  public class SelectionChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Constructor to set an newly selected and a unselected item.
    /// </summary>
    /// <param name="selectedItems"></param>
    /// <param name="unselectedItems"></param>
    public SelectionChangedEventArgs(IList selectedItems, IList unselectedItems)
    {
      AddedItems = selectedItems;
      RemovedItems = unselectedItems;
    }

    /// <summary>
    /// Newly selected items.
    /// </summary>
    public IList AddedItems { get; private set; }

    /// <summary>
    /// Newly unselected items.
    /// </summary>
    public IList RemovedItems { get; private set; }
  }
}

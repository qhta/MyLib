using System;
using System.Collections;

namespace Qhta.MVVM
{
  public class SelectionChangedEventArgs : EventArgs
  {
    public SelectionChangedEventArgs(IList selectedItems, IList unselectedItems)
    {
      AddedItems = selectedItems;
      RemovedItems = unselectedItems;
    }
    public IList AddedItems { get; private set; }
    public IList RemovedItems { get; private set; }
  }
}

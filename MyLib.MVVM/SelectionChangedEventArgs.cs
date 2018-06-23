using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MyLib.MVVM
{
  public class SelectionChangedEventArgs : RoutedEventArgs
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

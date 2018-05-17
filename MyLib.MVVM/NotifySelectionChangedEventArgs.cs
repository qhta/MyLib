using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MyLib.MVVM
{
  public class NotifySelectionChangedEventArgs : EventArgs
  {
    public NotifySelectionChangedEventArgs(IList selectedItems, IList unselectedItems)
    {
      SelectedItems = selectedItems;
      UnselectedItems = unselectedItems;
    }
    public IList SelectedItems { get; private set; }
    public IList UnselectedItems { get; private set; }
  }
}

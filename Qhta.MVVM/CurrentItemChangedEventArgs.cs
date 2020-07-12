using System;

namespace Qhta.MVVM
{
  public class CurrentItemChangedEventArgs : EventArgs
  {
    public CurrentItemChangedEventArgs(object newItem, object oldItem)
    {
      NewItem = newItem;
      OldItem = oldItem;
    }
    public object NewItem { get; private set; }
    public object OldItem { get; private set; }
  }
}

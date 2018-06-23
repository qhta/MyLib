using System;
using System.Collections.Generic;
using System.Text;

namespace MyLib.MVVM
{
  public class RoutedEventArgs : EventArgs
  {
    public RoutedEventArgs() { }
    public RoutedEventArgs(RoutedEvent routedEvent)
    {
      RoutedEvent = routedEvent;
    }
    public RoutedEventArgs(RoutedEvent routedEvent, object source)
    {
      RoutedEvent = routedEvent;
      Source = source;
    }

    public RoutedEvent RoutedEvent { get; set; }
    public bool Handled { get; set; }
    public object Source { get; set; }
    public object OriginalSource { get; }
  }
}

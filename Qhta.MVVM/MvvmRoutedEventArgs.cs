using System;

namespace Qhta.MVVM
{
  public class RoutedEventArgs : EventArgs
  {
    public RoutedEventArgs() { }
    public RoutedEventArgs(MvvmRoutedEvent routedEvent)
    {
      RoutedEvent = routedEvent;
    }
    public RoutedEventArgs(MvvmRoutedEvent routedEvent, object source)
    {
      RoutedEvent = routedEvent;
      Source = source;
    }

    public MvvmRoutedEvent RoutedEvent { get; set; }
    public bool Handled { get; set; }
    public object Source { get; set; }
    public object OriginalSource { get; }
  }
}

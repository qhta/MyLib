using System;

namespace Qhta.MVVM
{
  public class MvvmRoutedEventArgs : EventArgs
  {
    public MvvmRoutedEventArgs() { }
    public MvvmRoutedEventArgs(MvvmRoutedEvent routedEvent)
    {
      RoutedEvent = routedEvent;
    }
    public MvvmRoutedEventArgs(MvvmRoutedEvent routedEvent, object source)
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

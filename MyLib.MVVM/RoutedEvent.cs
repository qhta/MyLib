using System;
using System.Collections.Generic;
using System.Text;

namespace MyLib.MVVM
{
  public sealed class RoutedEvent
  {
    public string Name { get; }
    public RoutingStrategy RoutingStrategy { get; set; }
    public Type HandlerType { get; }
    public Type OwnerType { get; set; }
  }
}

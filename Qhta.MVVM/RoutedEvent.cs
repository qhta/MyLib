using System;

namespace Qhta.MVVM
{
  public sealed class RoutedEvent
  {
    public string Name { get; }
    public RoutingStrategy RoutingStrategy { get; set; }
    public Type HandlerType { get; }
    public Type OwnerType { get; set; }
  }
}

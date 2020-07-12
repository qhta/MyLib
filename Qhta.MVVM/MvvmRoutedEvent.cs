using System;

namespace Qhta.MVVM
{
  public sealed class MvvmRoutedEvent
  {
    public string Name { get; }
    public MvvmRoutingStrategy RoutingStrategy { get; set; }
    public Type HandlerType { get; }
    public Type OwnerType { get; set; }
  }
}

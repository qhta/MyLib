using System;

namespace Qhta.MVVM
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class DisplayPropertyAttribute: Attribute
  {
  }
}

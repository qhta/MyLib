using System;

namespace Qhta.MVVM
{

  /// <summary>
  /// An attribute that annotates a property which should be displayed.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class DisplayPropertyAttribute: Attribute
  {
  }
}

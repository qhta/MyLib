using System;
using System.Collections.Generic;
using System.Text;

namespace MyLib.MVVM
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class DisplayPropertyAttribute: Attribute
  {
  }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.ObservableViewModels
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
  public class IndexAttribute: Attribute
  {
    public IndexAttribute()
    {
    }

    public IndexAttribute(string name)
    {
      Name = name;
    }

    public string Name { get; private set; }
  }
}

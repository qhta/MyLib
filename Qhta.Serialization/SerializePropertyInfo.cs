using System;
using System.Reflection;

namespace Qhta.Serialization
{
  public class SerializePropertyInfo
  {
    public int Order { get; set;}
    public string Name { get; set;}
    public PropertyInfo PropInfo { get; set;}

    public override string ToString()
    {
      return Name;
    }
  }

}

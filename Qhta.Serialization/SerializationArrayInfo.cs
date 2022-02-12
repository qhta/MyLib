using System;
using System.Reflection;
using System.Collections.Generic;

namespace Qhta.Serialization
{
  public class SerializationArrayInfo: SerializationPropertyInfo
  {
    public SerializationArrayInfo(string name, PropertyInfo propInfo, int order): 
      base(name, propInfo, order) { }

    public KnownTypesDictionary KnownItemTypes { get; private set; } = new ();
  }

}

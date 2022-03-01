using System;
using System.Reflection;
using System.Collections.Generic;

namespace Qhta.Xml.Serialization
{
  public class SerializationArrayInfo: SerializationPropertyInfo
  {
    public SerializationArrayInfo(string name, PropertyInfo propInfo): 
      base(name, propInfo) 
    { 
      Order = Order;
    }

    public KnownItemTypesDictionary KnownItemTypes { get; private set; } = new ();

  }

}

using System;
using System.Reflection;
using System.Collections.Generic;

namespace Qhta.Xml.Serialization
{
  public class ArrayPropertyInfo: SerializationPropertyInfo
  {
    public ArrayPropertyInfo(string name, PropertyInfo propInfo) :
      base(name, propInfo)
    {
      Order = Order;
    }

    public KnownItemTypesDictionary KnownItemTypes { get; set; } = new ();

  }

}

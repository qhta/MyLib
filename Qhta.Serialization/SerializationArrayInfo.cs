using System;
using System.Reflection;
using System.Collections.Generic;

namespace Qhta.Xml.Serialization
{
  public class SerializationArrayInfo: SerializationPropertyInfo
  {
    public SerializationArrayInfo(string name, PropertyInfo propInfo, int order): 
      base(name, propInfo, order) { }

    public KnownItemTypesDictionary KnownItemTypes { get; private set; } = new ();

    public Type? CollectionType { get; set; }
  }

}

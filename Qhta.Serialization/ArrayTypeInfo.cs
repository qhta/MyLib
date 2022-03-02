using System;
using System.Reflection;
using System.Collections.Generic;

namespace Qhta.Xml.Serialization
{
  public class ArrayTypeInfo: SerializationTypeInfo
  {
    public ArrayTypeInfo(Type type): base(type) { }

    // Just defined in base class
    //public KnownItemTypesDictionary KnownItemTypes { get; private set; } = new ();

  }

}

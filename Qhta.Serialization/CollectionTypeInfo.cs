using System;
using System.Reflection;
using System.Collections.Generic;

namespace Qhta.Xml.Serialization
{
  public class CollectionTypeInfo: ArrayTypeInfo
  {
    public CollectionTypeInfo(Type type) : base(type) { }

    public MethodInfo? AddMethodInfo { get; set; }
  }

}

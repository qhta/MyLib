using System;
using System.Reflection;
using System.Collections.Generic;

namespace Qhta.Xml.Serialization
{
  public class CollectionPropertyInfo: ArrayPropertyInfo
  {
    public CollectionPropertyInfo(string name, PropertyInfo propInfo) :
      base(name, propInfo)
    { }


    public MethodInfo? AddMethodInfo { get; set; }
  }

}

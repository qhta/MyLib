using System;
using System.Reflection;
using System.Collections.Generic;

namespace Qhta.Xml.Serialization
{
  public class DictionaryTypeInfo: CollectionTypeInfo
  {
    public DictionaryTypeInfo(Type type): base(type) { }

    public SerializationTypeInfo? KeyTypeInfo { get; set; }

    public string? KeyName { get; set; }

    public SerializationTypeInfo? ValueTypeInfo { get; set; }
  }

}

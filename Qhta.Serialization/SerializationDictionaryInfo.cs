using System;
using System.Reflection;
using System.Collections.Generic;

namespace Qhta.Xml.Serialization
{
  public class SerializationDictionaryInfo: SerializationCollectionInfo
  {
    public SerializationDictionaryInfo(string name, PropertyInfo propInfo): 
      base(name, propInfo) { }

    public SerializationTypeInfo? KeyTypeInfo { get; set; }

    public string? KeyName { get; set; }

    public SerializationTypeInfo? ValueTypeInfo { get; set; }
  }

}

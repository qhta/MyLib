using System;
using System.Reflection;
using System.Collections.Generic;

namespace Qhta.Xml.Serialization
{
  public class SerializationDictionaryInfo: SerializationArrayInfo
  {
    public SerializationDictionaryInfo(string name, PropertyInfo propInfo,  Type keyType, int order): 
      base(name, propInfo, order) 
    { 
      KeyType = keyType;
    }

    public Type KeyType { get; set; }

    public string? KeyName { get; set; }

    public Type? ValueType { get; set; }

  }

}

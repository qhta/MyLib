using System;
using System.Reflection;
using System.Collections.Generic;

namespace Qhta.Xml.Serialization
{
  public class DictionaryInfo: CollectionInfo
  {
    public string? KeyName { get; set; }

    public string? ValueName { get; set; }

    public SerializationTypeInfo? KeyTypeInfo { get; set; }

    public SerializationTypeInfo? ValueTypeInfo { get; set; }
  }

}

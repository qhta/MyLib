using System.Reflection;

namespace Qhta.Xml.Serialization;

public class DictionaryInfo: CollectionInfo
{
  public string? KeyName { get; set; }

  public PropertyInfo? KeyProperty { get; set; }

  public string? ValueName { get; set; }

  //public PropertyInfo? ValueProperty { get; set; }

  [XmlReference]
  public SerializationTypeInfo? KeyTypeInfo { get; set; }

  [XmlReference]
  public SerializationTypeInfo? ValueTypeInfo { get; set; }
}
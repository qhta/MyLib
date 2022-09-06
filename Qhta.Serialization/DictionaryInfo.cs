using System.Reflection;

namespace Qhta.Xml.Serialization;

public class DictionaryInfo: CollectionInfo
{
  [XmlAttribute]
  public string? KeyName { get; set; }

  [XmlIgnore]
  public PropertyInfo? KeyProperty { get; set; }

  [XmlReference]
  public SerializationTypeInfo? KeyTypeInfo { get; set; }

  [XmlReference]
  public SerializationTypeInfo? ValueTypeInfo { get; set; }
}
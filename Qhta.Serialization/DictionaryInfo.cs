using System.Reflection;

namespace Qhta.Xml.Serialization;

public class DictionaryInfo: ContentItemInfo
{

  [XmlAttribute]
  [XmlReference]
  public SerializationTypeInfo? KeyTypeInfo { get; set; }

  [XmlAttribute]
  [XmlReference]
  public SerializationTypeInfo? ValueTypeInfo { get; set; }

  [XmlAttribute]
  public string? KeyName { get; set; }

  [XmlIgnore]
  public PropertyInfo? KeyProperty { get; set; }
}
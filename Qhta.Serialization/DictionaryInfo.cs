using System.Reflection;

namespace Qhta.Xml.Serialization;

public class DictionaryInfo: CollectionInfo
{
  [XmlAttribute]
  [DefaultValue(false)]
  public override bool IsDictionary => true;

  [XmlAttribute]
  public string? KeyName { get; set; }

  [XmlIgnore]
  public PropertyInfo? KeyProperty { get; set; }

  [XmlAttribute]
  [XmlReference]
  public SerializationTypeInfo? KeyTypeInfo { get; set; }

  [XmlAttribute]
  [XmlReference]
  public SerializationTypeInfo? ValueTypeInfo { get; set; }
}
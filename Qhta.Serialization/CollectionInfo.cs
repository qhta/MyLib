using System.Reflection;

namespace Qhta.Xml.Serialization;


public class CollectionInfo
{
  [XmlAttribute]
  [DefaultValue(false)]
  public virtual bool IsDictionary => false;

  /// <summary>
  /// If a collection of objects stores references only.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsReferences { get; set;}

  /// <summary>
  /// Known types for collection items.
  /// </summary>
  [XmlReferences]
  public KnownItemTypesDictionary KnownItemTypes { get; set; } = new();
}
using System.Reflection;

namespace Qhta.Xml.Serialization;


public class CollectionInfo
{
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
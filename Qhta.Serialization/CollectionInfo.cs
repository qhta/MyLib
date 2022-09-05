using System.Reflection;

namespace Qhta.Xml.Serialization;

public class CollectionInfo
{
  [XmlReference]
  public SerializationTypeInfo? CollectionTypeInfo { get; set; }

  public SerializationTypeInfo? ItemTypeInfo { get; set; }

  /// <summary>
  /// Known types for collection items.
  /// </summary>
  [XmlReferences]
  public KnownItemTypesDictionary KnownItemTypes { get; set; } = new KnownItemTypesDictionary();

  public MethodInfo? AddMethodInfo { get; set; }

  public bool IsReferences { get; set;}
}
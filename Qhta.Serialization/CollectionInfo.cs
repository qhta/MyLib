using System.Reflection;

namespace Qhta.Xml.Serialization;

public class CollectionInfo
{

  public SerializationTypeInfo? CollectionTypeInfo { get; set; }

  public SerializationTypeInfo? ItemTypeInfo { get; set; }

  /// <summary>
  /// Known types for collection items.
  /// </summary>
  public KnownItemTypesDictionary KnownItemTypes { get; set; } = new KnownItemTypesDictionary();

  public MethodInfo? AddMethodInfo { get; set; }
}
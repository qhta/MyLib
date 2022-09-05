using System.Xml.Linq;

namespace Qhta.Xml.Serialization;

public class SerializationTypeInfo: ITypeInfo
{

  public SerializationTypeInfo(Type aType)
  {
    var aName = aType.Name;
    var rootAttribute = aType.GetCustomAttribute<XmlRootAttribute>();
    if (rootAttribute != null)
      aName = rootAttribute.ElementName;
    var aNamespace = aType.Namespace;
    Name = new QualifiedName(aName, aNamespace);
    Type = aType;
  }

  /// <summary>
  /// A type to serialize or deserialize
  /// </summary>
  public Type Type { get; set; }

  /// <summary>
  /// XmlElement name for a type
  /// </summary>
  public QualifiedName Name { get; set; }


  /// <summary>
  /// A public constructor to invoke while deserialization
  /// </summary>
  public ConstructorInfo? KnownConstructor { get; set; }

  /// <summary>
  /// Converter to read write XML.
  /// </summary>
  public XmlConverter? XmlConverter { get; set; }

  /// <summary>
  /// Converter to/from string value.
  /// </summary>
  public TypeConverter? TypeConverter { get; set; }

  /// <summary>
  /// Known properties to serialize as XML attributes.
  /// </summary>
  public KnownPropertiesDictionary PropsAsAttributes { get; set; } = new KnownPropertiesDictionary();

  /// <summary>
  /// Known properties to serialize as XML elements.
  /// </summary>
  public KnownPropertiesDictionary PropsAsElements { get; set; } = new KnownPropertiesDictionary();

  /// <summary>
  /// Known property to accept content of XmlElement.
  /// </summary>
  public SerializationPropertyInfo? KnownContentProperty { get; set; }

  /// <summary>
  /// Known property to accept text content of XmlElement.
  /// </summary>
  public SerializationPropertyInfo? KnownTextProperty { get; set; }

  /// <summary>
  /// Known types for collection items.
  /// </summary>
  public KnownItemTypesDictionary KnownItemTypes { get; set; } = new KnownItemTypesDictionary();

  /// <summary>
  /// Optional collection info filled if a property is an array, collection or dictionary
  /// </summary>
  public CollectionInfo? CollectionInfo { get; set; }

  public override string ToString()
  {
    return $"{this.GetType().Name}({Type?.FullName})";
  }
}
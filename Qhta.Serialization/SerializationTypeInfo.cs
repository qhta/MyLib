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
    Type = aType;
    Name = new QualifiedName(aName, aNamespace);
  }

  /// <summary>
  /// XmlElement name for a type
  /// </summary>
  [XmlAttribute]
  public QualifiedName Name { get; set; }

  /// <summary>
  /// A type to serialize or deserialize
  /// </summary>
  public Type Type { get; set; }

  public bool ShouldSerializeType() => Name != new QualifiedName(Type.Name, Type.Namespace);

  /// <summary>
  /// A public constructor to invoke while deserialization
  /// </summary>
  [XmlIgnore]
  public ConstructorInfo? KnownConstructor { get; set; }

  [XmlAttribute]
  [DefaultValue(true)]
  public bool HasKnownConstructor => KnownConstructor != null;

  /// <summary>
  /// Converter to/from string value.
  /// </summary>
  public TypeConverter? TypeConverter { get; set; }

  /// <summary>
  /// Converter to read/write XML.
  /// </summary>
  public XmlConverter? XmlConverter { get; set; }

  /// <summary>
  /// Known properties to serialize as XML attributes.
  /// </summary>
  public KnownPropertiesDictionary MembersAsAttributes { get; set; } = new ();

  public bool ShouldSerializePropertiesAsAttributes() => MembersAsAttributes.Any();

  /// <summary>
  /// Known properties to serialize as XML elements.
  /// </summary>
  public KnownPropertiesDictionary MembersAsElements { get; set; } = new ();

  public bool ShouldSerializePropertiesAsElements() => MembersAsElements.Any();

  /// <summary>
  /// Known property to accept content of XmlElement.
  /// </summary>
  public SerializationMemberInfo? ContentProperty { get; set; }

  /// <summary>
  /// Known property to accept text content of XmlElement.
  /// </summary>
  public SerializationMemberInfo? TextProperty { get; set; }

  /// <summary>
  /// If a class can be substituted by subclasses then these classes are listed here.
  /// </summary>
  [XmlElement]
  public KnownTypesDictionary? KnownSubtypes { get; set; }

  /// <summary>
  /// Specifies if a type is serialized as a collection but not as a dictionary.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsCollection => CollectionInfo != null && !IsDictionary;

  /// <summary>
  /// Specifies If a type is serialized as a dictionary but not as a collection.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsDictionary => CollectionInfo is DictionaryInfo;

  /// <summary>
  /// Optional collection info filled if a property is an array, collection or dictionary
  /// </summary>
  [XmlElement]
  public CollectionInfo? CollectionInfo { get; set; }


  public override string ToString()
  {
    return Name.ToString();
  }
}
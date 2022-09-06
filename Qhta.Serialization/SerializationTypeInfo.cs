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
  //{
  //  get => _Name;
  //  set
  //  {
  //    _Name = value;
  //    if (Type == null)
  //      Type = Type.GetType(Name.ToString().Replace(':', '.'));
  //  }
  //}
  //private QualifiedName _Name;

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
  public KnownPropertiesDictionary PropertiesAsAttributes { get; set; } = new ();

  public bool ShouldSerializePropertiesAsAttributes() => PropertiesAsAttributes.Any();

  /// <summary>
  /// Known properties to serialize as XML elements.
  /// </summary>
  public KnownPropertiesDictionary PropertiesAsElements { get; set; } = new ();

  public bool ShouldSerializePropertiesAsElements() => PropertiesAsElements.Any();

  /// <summary>
  /// Known property to accept content of XmlElement.
  /// </summary>
  public SerializationPropertyInfo? ContentProperty { get; set; }

  /// <summary>
  /// Known property to accept text content of XmlElement.
  /// </summary>
  public SerializationPropertyInfo? TextProperty { get; set; }

  ///// <summary>
  ///// Known types for collection items.
  ///// </summary>
  //public KnownItemTypesDictionary KnownItemTypes { get; set; } = new KnownItemTypesDictionary();

  /// <summary>
  /// Optional collection info filled if a property is an array, collection or dictionary
  /// </summary>
  public CollectionInfo? CollectionInfo { get; set; }

  public override string ToString()
  {
    return Name.ToString();
  }
}
namespace Qhta.Xml.Reflection;

public class SerializationTypeInfo : ITypeNameInfo, INamedElement
{
  public SerializationTypeInfo(Type aType)
  {
    var aName = aType.Name;
    var rootAttribute = aType.GetCustomAttribute<XmlRootAttribute>();
    if (rootAttribute != null)
      aName = rootAttribute.ElementName;
    else if (aName.EndsWith("[]"))
      aName = aName.Shorten(2).Pluralize();
    Type = aType;
    XmlName = aName;
    if (rootAttribute != null)
      XmlNamespace = rootAttribute.Namespace;
    ClrNamespace = aType.Namespace;
  }

  /// <summary>
  ///   A public constructor to invoke while deserialization
  /// </summary>
  [XmlIgnore]
  public ConstructorInfo? KnownConstructor { get; set; }

  [XmlAttribute]
  [DefaultValue(true)]
  public bool HasKnownConstructor => KnownConstructor != null;

  /// <summary>
  ///   Converter to/from string value.
  /// </summary>
  public TypeConverter? TypeConverter { get; set; }

  /// <summary>
  ///   Converter to read/write XML.
  /// </summary>
  public IXmlConverter? XmlConverter { get; set; }

  /// <summary>
  ///   Known properties to serialize as XML attributes.
  /// </summary>
  public KnownMembersCollection MembersAsAttributes { get; set; } = new();

  /// <summary>
  ///   Known properties to serialize as XML elements.
  /// </summary>
  public KnownMembersCollection MembersAsElements { get; set; } = new();

  /// <summary>
  ///   Known property to accept content of XmlElement.
  /// </summary>
  public SerializationMemberInfo? ContentProperty { get; set; }

  /// <summary>
  ///   Known property to accept text content of XmlElement.
  /// </summary>
  public SerializationMemberInfo? TextProperty { get; set; }

  /// <summary>
  ///   If a class can be substituted by subclasses then these classes are listed here.
  /// </summary>
  [XmlElement]
  public KnownTypesCollection? KnownSubtypes { get; set; }

  /// <summary>
  ///   Specifies if a type is serialized as a collection but not as a dictionary.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsCollection => ContentProperty == null && ContentInfo != null && !IsDictionary;

  /// <summary>
  ///   Specifies If a type is serialized as a dictionary but not as a collection.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsDictionary => ContentInfo is DictionaryInfo;

  /// <summary>
  ///   Optional collection info filled if a property is an array, collection or dictionary
  /// </summary>
  [XmlElement]
  public ContentItemInfo? ContentInfo { get; set; }

  /// <summary>
  ///   Name of the Xml element
  /// </summary>
  [XmlAttribute]
  public string XmlName { get; set; }

  /// <summary>
  ///   XmlNamespace of the element
  /// </summary>
  [XmlAttribute]
  public string? XmlNamespace { get; set; }

  /// <summary>
  ///   ClrNamespace of the element
  /// </summary>
  [XmlAttribute]
  public string? ClrNamespace { get; set; }

  /// <summary>
  ///   Mapped type
  /// </summary>
  public Type Type { get; set; }

  public bool ShouldSerializeType()
  {
    return String.IsNullOrEmpty(XmlName);
  }

  public bool ShouldSerializePropertiesAsAttributes()
  {
    return MembersAsAttributes.Any();
  }

  public bool ShouldSerializePropertiesAsElements()
  {
    return MembersAsElements.Any();
  }

  public override string ToString()
  {
    if (XmlNamespace != null)
      return $"{XmlNamespace}:{XmlName} => {Type.FullName}";
    return $"{XmlName} => {Type.FullName}";
  }
}
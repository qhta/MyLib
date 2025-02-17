﻿namespace Qhta.Xml.Reflection;

/// <summary>
/// Information needed for type serialization
/// </summary>
/// <seealso cref="Qhta.Xml.Reflection.ITypeNameInfo" />
/// <seealso cref="Qhta.Xml.Reflection.INamedElement" />
public class SerializationTypeInfo : ITypeNameInfo, INamedElement
{
  /// <summary>
  /// Initializes a new instance of the <see cref="SerializationTypeInfo"/> class.
  /// </summary>
  /// <param name="aType">a type.</param>
  public SerializationTypeInfo(Type aType)
  {
    KnownMembers = new KnownMembersCollection(this);
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
  /// Gets a value indicating whether this instance has known constructor.
  /// </summary>
  /// <value>
  ///   <c>true</c> if this instance has known constructor; otherwise, <c>false</c>.
  /// </value>
  [XmlAttribute]
  [DefaultValue(true)]
  public bool HasKnownConstructor => KnownConstructor != null;

  /// <summary>
  /// A public constructor info to invoke while deserialization
  /// </summary>
  [XmlIgnore]
  public ConstructorInfo? KnownConstructor { [DebuggerStepThrough] get; set; }

    /// <summary>
  ///   Converter to/from string value.
  /// </summary>
  public TypeConverter? TypeConverter { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Converter to read/write XML.
  /// </summary>
  public IXmlConverter? XmlConverter { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Known properties to serialize as XML attributes.
  /// </summary>
  public KnownMembersCollection KnownMembers { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Known properties to serialize as XML attributes.
  /// </summary>
  public IEnumerable<SerializationMemberInfo> MembersAsAttributes => KnownMembers.Where(item => item.IsAttribute);

  /// <summary>
  ///   Known properties to serialize as XML elements.
  /// </summary>
  public IEnumerable<SerializationMemberInfo> MembersAsElements => KnownMembers.Where(item => !item.IsAttribute);

  /// <summary>
  ///   Known property to accept text content of XmlElement.
  /// </summary>
  public SerializationMemberInfo? TextProperty { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Specifies that a derived types can't occur when serializing object values.
  /// </summary>
  public bool IsSealed { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   If a class can be substituted by subclasses then these classes are listed here.
  /// </summary>
  [XmlElement]
  public KnownTypesCollection? KnownSubtypes { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Specifies whether the type is serialized as a collection but not as a dictionary.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsCollection => ContentInfo is ContentItemInfo && !IsDictionary;

  /// <summary>
  ///   Specifies whether the type is serialized as a dictionary but not as a collection.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsDictionary => ContentInfo is DictionaryInfo;

  /// <summary>
  ///   Known property to accept content of XmlElement.
  /// </summary>
  public SerializationMemberInfo? ContentProperty { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Specifies whether the type instance must be serialized as an object, not a simple collection.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsObject { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Optional collection info filled if a property is an array, collection or dictionary
  /// </summary>
  [XmlElement]
  public ContentItemInfo? ContentInfo { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Name of the Xml element
  /// </summary>
  [XmlAttribute]
  public string XmlName { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   XmlNamespace of the element
  /// </summary>
  [XmlAttribute]
  public string? XmlNamespace { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   ClrNamespace of the element
  /// </summary>
  [XmlAttribute]
  public string? ClrNamespace { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Mapped type
  /// </summary>
  public Type Type { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Gets the value that indicates whether the type should be serialized.
  /// </summary>
  /// <returns></returns>
  public bool ShouldSerializeType()
  {
    return String.IsNullOrEmpty(XmlName);
  }

  /// <summary>
  /// Gets the value that indicates whether there are properties to be serialized as xml attributes.
  /// </summary>
  /// <returns></returns>
  public bool ShouldSerializePropertiesAsAttributes()
  {
    return MembersAsAttributes.Any();
  }

  /// <summary>
  /// Gets the value that indicates whether there are properties to be serialized as xml properties.
  /// </summary>
  /// <returns></returns>
  public bool ShouldSerializePropertiesAsElements()
  {
    return MembersAsElements.Any();
  }

  /// <summary>
  /// Converts to string in format "namespace:name"
  /// </summary>
  /// <returns>
  /// A <see cref="System.String" /> that represents this instance.
  /// </returns>
  public override string ToString()
  {
    if (XmlNamespace != null)
      return $"{XmlNamespace}:{XmlName} => {Type.FullName}";
    return $"{XmlName} => {Type.FullName}";
  }

  /// <summary>
  /// Gets the the qualified name (XmlName, XmlNamespace) of the element
  /// </summary>
  public QualifiedName QualifiedName => new(XmlName, XmlNamespace);
}
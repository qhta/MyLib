namespace Qhta.Xml.Reflection;

/// <summary>
///   Represents the information about property or field needed for serialization/deserialization.
/// </summary>
public class SerializationMemberInfo : INamedElement, IComparable<SerializationMemberInfo>
{
  /// <summary>
  ///   Constructor with parameters.
  /// </summary>
  /// <param name="parentType">A type to hold this member</param>
  /// <param name="name">Attribute or element name used for serialization></param>
  /// <param name="memberInfo">Applied member info. It can be either PropertyInfo or FieldInfo</param>
  /// <param name="order">Needed to sort the order for serialization</param>
  public SerializationMemberInfo(SerializationTypeInfo parentType, string name, MemberInfo memberInfo, int order = int.MaxValue)
  {
    ParentType = parentType;
    XmlName = name;
    Member = memberInfo;
    Order = order;
  }

  /// <summary>
  ///   Constructor with parameters.
  /// </summary>
  /// <param name="parentType">A type to hold this member</param>
  /// <param name="name">Attribute or element name used for serialization></param>
  /// <param name="memberInfo">Applied member info. It can be either PropertyInfo or FieldInfo</param>
  /// <param name="order">Needed to sort the order for serialization</param>
  public SerializationMemberInfo(SerializationTypeInfo parentType, QualifiedName name, MemberInfo memberInfo, int order = int.MaxValue)
  {
    this.ParentType = parentType;
    XmlName = name.LocalName;
    ClrNamespace = name.Namespace;
    Member = memberInfo;
    IsNullable = Member.GetCustomAttribute<XmlElementAttribute>()?.IsNullable ?? false;
    Order = order;
  }

  /// <summary>
  /// A serialization info for the type where this member belongs.
  /// </summary>
  public SerializationTypeInfo ParentType { get; private set; }

  /// <summary>
  /// Gets a value indicating whether the member is a field of some class
  /// </summary>
  /// <value>
  ///   <c>true</c> if this instance is field; otherwise, <c>false</c>.
  /// </value>
  [XmlIgnore] public bool IsField => Member is FieldInfo;

  /// <summary>
  /// Converts the member to field reflection info
  /// </summary>
  [XmlIgnore] public FieldInfo? Field => Member as FieldInfo;

  /// <summary>
  /// Converts the member to property reflection info
  /// </summary>
  [XmlIgnore] public PropertyInfo? Property => Member as PropertyInfo;

  /// <summary>
  /// Gets the member type (field type or property type).
  /// </summary>
  public Type? MemberType => Property?.PropertyType ?? Field?.FieldType;

  /// <summary>
  /// Gets a value indicating whether the member can be written in a class instance.
  /// </summary>
  public bool CanWrite => Property?.CanWrite ?? !Field?.IsInitOnly ?? false;

  /// <summary>
  /// Specifies whether serialization as XML attribute is preferred over XML element.
  /// </summary>
  public bool IsAttribute { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Needed to sort the order of properties for serialization.
  /// </summary>
  [XmlAttribute]
  public int Order { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Applied member info.
  /// </summary>
  [XmlIgnore]
  public MemberInfo Member { [DebuggerStepThrough] get; }

  /// <summary>
  /// Specifies whether the member is part of composite key (multiple members forming a key).
  /// </summary>
  public bool IsCompositeKey { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   More members if it is composite key.
  /// </summary>
  [XmlIgnore]
  public PropertyInfo[]? MoreMembers { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   XSD standard data type for simple value text conversion.
  /// </summary>
  [XmlAttribute]
  public SimpleType? DataType { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Specific format for text conversion.
  /// </summary>
  [XmlAttribute]
  public string? Format { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Specific culture info for text conversion.
  /// </summary>
  [XmlAttribute]
  public CultureInfo? Culture { [DebuggerStepThrough] get; set; }

  ///// <summary>
  /////   Conversion options for default TypeConverter.
  ///// </summary>
  //[XmlAttribute]
  //public ConversionOptions? ConversionOptions { get; set; }

  /// <summary>
  ///   Specifies whether a member is serialized as parent element content
  ///   (without xml property tag).
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsContentElement { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Specifies whether a member is nullable.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsNullable { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Specifies whether a member is serialized as a reference to an object.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsReference { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Specifies Id property of referenced type.
  /// </summary>
  [XmlIgnore]
  public SerializationMemberInfo? IdProperty { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Specifies a default value (for simple types only) which is not serialized.
  /// </summary>
  [XmlAttribute]
  public object? DefaultValue { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Applied type info of the member value.
  /// </summary>
  [XmlAttribute]
  [XmlReference]
  public SerializationTypeInfo ValueType { [DebuggerStepThrough] get; set; } = null!;

  /// <summary>
  ///   Used for conversion value from/to string.
  /// </summary>
  public TypeConverter? TypeConverter { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Used for conversion value from/to xml.
  /// </summary>
  public IXmlConverter? XmlConverter { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   A method used to specify if a member should be serialized at run-time.
  ///   The method should be a parameterless function of type boolean.
  /// </summary>
  [XmlIgnore]
  public MethodInfo? CheckMethod { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Specifies whether this instance has a check method.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool HasCheckMethod => CheckMethod != null;

  /// <summary>
  /// Gets a value indicating whether this instance has known subtypes.
  /// </summary>
  /// <value>
  ///   <c>true</c> if this instance is polymorphic; otherwise, <c>false</c>.
  /// </value>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsPolymorphic => GetKnownSubtypes() != null;

  /// <summary>
  ///   If a valueType can be substituted by subclasses then these classes are listed here.
  /// </summary>
  [XmlElement]
  public KnownTypesCollection? KnownSubtypes { [DebuggerStepThrough] get; set; }

  /// <summary>
  ///   Specifies whether the type is serialized as a collection but not as a dictionary
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsCollection => GetCollectionInfo() != null && !IsDictionary;

  /// <summary>
  ///   Specifies whether the type is serialized as a dictionary but not as a collection
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsDictionary => GetCollectionInfo() is DictionaryContentInfo;

  /// <summary>
  ///   Specifies whether the type instance must be serialized as an object, not a simple collection.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsObject
  {
    get => _IsObject || ValueType?.IsObject == true;
    set => _IsObject = value;
  }
  private bool _IsObject;

  /// <summary>
  ///   Optional collection info filled if a member is an array, collection or dictionary.
  /// </summary>
  [XmlElement]
  public ContentInfo? ContentInfo { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Compares the order of two instances. Allows two items of the same order to occur in dictionary.
  /// </summary>
  /// <param name="other">An object to compare with this instance.</param>
  /// <returns>
  /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
  /// <list type="table"><listheader><term> Value</term><description> Meaning</description></listheader><item><term> Less than zero</term><description> This instance precedes <paramref name="other" /> in the sort order.</description></item><item><term> Zero</term><description> This instance occurs in the same position in the sort order as <paramref name="other" />.</description></item><item><term> Greater than zero</term><description> This instance follows <paramref name="other" /> in the sort order.</description></item></list>
  /// </returns>
  public int CompareTo(SerializationMemberInfo? other)
  {
    if (Order >= other?.Order)
      return 1;
    return -1;
  }

  /// <summary>
  ///   Attribute or element name used for serialization.
  /// </summary>
  [XmlAttribute]
  public string XmlName { [DebuggerStepThrough] get; private set; }

  /// <summary>
  ///   Attribute or element XML namespace used for serialization.
  /// </summary>
  [XmlAttribute]
  public string? XmlNamespace { [DebuggerStepThrough] get; private set; }

  /// <summary>
  ///   ClrNamespace of the property or field.
  /// </summary>
  [XmlAttribute]
  public string? ClrNamespace { [DebuggerStepThrough] get; private set; }

  /// <summary>
  /// Gets the qualified name (XmlName, XmlNamespace) of the element.
  /// </summary>
  [XmlIgnore] public QualifiedName QualifiedName => new(XmlName, ClrNamespace);

  /// <summary>
  /// Gets the value of the member (field value or property value).
  /// </summary>
  /// <param name="obj">The object.</param>
  /// <returns></returns>
  public object? GetValue(object? obj)
  {
    if (Property != null)
    {
      var getMethod = Property.GetGetMethod();
      if (getMethod != null)
        return getMethod.Invoke(obj, []);
    }
    else
    if (Field != null)
      return Field?.GetValue(obj);
    return null;
  }

  /// <summary>
  /// Sets the value of the member (field value or property value).
  /// </summary>
  /// <param name="obj">The object.</param>
  /// <param name="value">The value.</param>
  public void SetValue(object? obj, object? value)
  {
    if (IsField)
      Field?.SetValue(obj, value);
    else
      Property?.SetValue(obj, value);
  }

  /// <summary>
  ///   Gets known subtypes as saved or from ValueType.
  /// </summary>
  /// <returns></returns>
  public KnownTypesCollection? GetKnownSubtypes()
  {
    return KnownSubtypes ?? ValueType?.KnownSubtypes;
  }

  /// <summary>
  ///   Gets CollectionInfo as saved or from ValueType.
  /// </summary>
  /// <returns></returns>
  public ContentInfo? GetCollectionInfo()
  {
    if (ContentInfo is CollectionContentInfo collectionInfo)
      return collectionInfo;
    return ValueType?.ContentInfo as CollectionContentInfo;
  }

  /// <summary>
  /// Converts to string in format 'name(member-name)'
  /// </summary>
  /// <returns>
  /// A <see cref="System.String" /> that represents this instance.
  /// </returns>
  public override string ToString()
  {
    return $"{GetType().Name}({Member.Name})";
  }

  /// <summary>
  /// Gets the type converter to serialize/deserialize as a string.
  /// </summary>
  /// <returns></returns>
  public TypeConverter? GetTypeConverter()
  {
    return TypeConverter ?? ValueType?.TypeConverter;
  }
}
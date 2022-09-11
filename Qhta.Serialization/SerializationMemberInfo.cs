using System.Reflection;
using System.Xml.Serialization;

namespace Qhta.Xml.Serialization;

/// <summary>
/// Represents the information about property or field needed for serialization/deserialization.
/// </summary>
public class SerializationMemberInfo: IComparable<SerializationMemberInfo>
{

  /// <summary>
  /// Constructor with parameters.
  /// </summary>
  /// <param name="name">Attribute or element name used for serialization></param>
  /// <param name="memberInfo">Applied member info. It can be either PropertyInfo or FieldInfo</param>
  /// <param name="order">Needed to sort the order for serialization</param>
  public SerializationMemberInfo(string name, MemberInfo memberInfo, int order=int.MaxValue)
  {
    Name = new QualifiedName(name);
    Member = memberInfo;
    IsNullable = Member.GetCustomAttribute<XmlElementAttribute>()?.IsNullable ?? false;
    Order = order;
  }

  [XmlIgnore]
  public bool IsField => Member is FieldInfo;

  [XmlIgnore]
  public FieldInfo? Field => Member as FieldInfo;

  [XmlIgnore]
  public bool IsProperty => Member is PropertyInfo;

  [XmlIgnore]
  public PropertyInfo? Property => Member as PropertyInfo;

  public Type? MemberType => Property?.PropertyType ?? Field?.FieldType;

  public bool CanWrite => Property?.CanWrite ?? !Field?.IsInitOnly ?? false;

  public object? GetValue(object? obj) => Property?.GetValue(obj) ?? Field?.GetValue(obj);

  public void SetValue(object? obj, object? value)
  {
    if (IsProperty)
      Property?.SetValue(obj, value);
    else
      Field?.SetValue(obj, value);
  }

  

  /// <summary>
  /// Needed to sort the order of properties for serialization.
  /// </summary>
  [XmlAttribute]
  public int Order { get; set; }

  /// <summary>
  /// Attribute or element name used for serialization.
  /// </summary>
  [XmlAttribute]
  public QualifiedName Name { get; init; }

  /// <summary>
  /// Applied member info.
  /// </summary>
  [XmlIgnore]
  public MemberInfo Member { get;}

  /// <summary>
  /// Specifies if a member is nullable.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsNullable { get; set;}

  /// <summary>
  /// Specifies if  member is serialized as a reference to an object.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsReference { get; set; }

  /// <summary>
  /// Specifies a default value (for simple types only) which is not serialized.
  /// </summary>
  [XmlAttribute]
  public object? DefaultValue { get; set; }

  /// <summary>
  /// Applied type info of the member value.
  /// </summary>
  [XmlAttribute]
  [XmlReference]
  public SerializationTypeInfo? ValueType { get; set; }

  /// <summary>
  /// Used for conversion value from/to string.
  /// </summary>
  public TypeConverter? TypeConverter { get; set; }

  /// <summary>
  /// Used for conversion value from/to xml.
  /// </summary>
  public XmlConverter? XmlConverter { get; set; }

  /// <summary>
  /// A method used to specify if a member should be serialized at run-time.
  /// The method should be a parameterless function of type boolean.
  /// </summary>
  [XmlIgnore]
  public MethodInfo? CheckMethod { get; set; }

  [XmlAttribute]
  [DefaultValue(false)]
  public bool HasCheckMethod => CheckMethod != null;

  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsPolymorfic => GetKnownSubtypes() != null;

  /// <summary>
  /// If a valueType can be substituted by subclasses then these classes are listed here.
  /// </summary>
  [XmlElement]
  public KnownTypesDictionary? KnownSubtypes { get; set; }

  /// <summary>
  /// Get KnownSubtypes as saved or from ValueType.
  /// </summary>
  /// <returns></returns>
  public KnownTypesDictionary? GetKnownSubtypes() => KnownSubtypes ?? ValueType?.KnownSubtypes;

  /// <summary>
  /// If a type is serialized as a collection but not as a dictionary
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsCollection => GetCollectionInfo() != null && !IsDictionary;

  /// <summary>
  /// If a type is serialized as a dictionary but not as a collection
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool IsDictionary => GetCollectionInfo() is DictionaryInfo;

  /// <summary>
  /// Optional collection info filled if a member is an array, collection or dictionary.
  /// </summary>
  [XmlElement]
  public CollectionInfo? CollectionInfo { get; set; }

  /// <summary>
  /// Get CollectionInfo as saved or from ValueType.
  /// </summary>
  /// <returns></returns>
  public CollectionInfo? GetCollectionInfo() => CollectionInfo ?? ValueType?.CollectionInfo;

  public int CompareTo(SerializationMemberInfo? other)
  {
    if (this.Order >= other?.Order)
      return 1;
    return -1;
  }

  public override string ToString()
  {
    return $"{this.GetType().Name}({Member.Name})";
  }

  public TypeConverter? GetTypeConverter() => TypeConverter ?? ValueType?.TypeConverter;
}
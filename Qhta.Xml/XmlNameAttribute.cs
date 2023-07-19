namespace Qhta.Xml;

/// <summary>
/// Specifies a kind of serialized/desetialized name.
/// </summary>
public enum XmlAttributeNameKind
{
  /// <summary>
  /// Name represents serialized/deserialized element.
  /// </summary>
  Element,
  /// <summary>
  /// Name represents serialized/deserialized property or field.
  /// </summary>
  Property,
  /// <summary>
  /// Name represents a method used in serialization/deserialization.
  /// </summary>
  Method,
}

/// <summary>
/// Defines an attribute which can specify a name recognized in a class/interface/struct.
/// There may be multiple such attributes declared for a single type.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
public class XmlNameAttribute : Attribute
{

  /// <summary>
  /// Default constructor.
  /// </summary>
  public XmlNameAttribute()
  {
  }

  /// <summary>
  /// Constructor with element name.
  /// </summary>
  /// <param name="elementName"></param>
  public XmlNameAttribute(string? elementName)
  {
    ElementName = elementName;
  }

  private string? Name { get; set; }

  /// <summary>
  /// A type of the name
  /// </summary>
  public XmlAttributeNameKind NameType { get; set; }

  /// <summary>
  /// Gets/sets a name as an element name.
  /// </summary>
  public string? ElementName
  {
    get => NameType == XmlAttributeNameKind.Element ? Name : null;
    set
    {
      Name = value;
      NameType = XmlAttributeNameKind.Element;
    }
  }

  /// <summary>
  /// Gets/sets a name as a property (or field) name.
  /// </summary>
  public string? PropertyName
  {
    get => NameType == XmlAttributeNameKind.Property ? Name : null;
    set
    {
      Name = value;
      NameType = XmlAttributeNameKind.Property;
    }
  }

  /// <summary>
  /// Gets/sets a name as a method name.
  /// </summary>
  public string? MethodName
  {
    get => NameType == XmlAttributeNameKind.Method ? Name : null;
    set
    {
      Name = value;
      NameType = XmlAttributeNameKind.Method;
    }
  }
}
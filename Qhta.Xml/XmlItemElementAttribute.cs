namespace Qhta.Xml;

/// <summary>
/// Defines an attribute which can specify data needed to serialize/deserialize class/interface/struct items.
/// There may be multiple such attributes declared for a single container.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
public class XmlItemElementAttribute : XmlArrayItemAttribute
{
  /// <summary>
  /// Default constructor.
  /// </summary>
  public XmlItemElementAttribute()
  {
  }

  /// <summary>
  /// Initializing constructor with element name.
  /// </summary>
  /// <param name="elementName"></param>
  public XmlItemElementAttribute(string? elementName) : base(elementName)
  {
  }

  /// <summary>
  /// Initializing constructor with element name and type.
  /// </summary>
  /// <param name="elementName"></param>
  /// <param name="type"></param>
  public XmlItemElementAttribute(string? elementName, Type? type) : base(elementName, type)
  {
  }

  /// <summary>
  /// Initializing constructor with element type.
  /// </summary>
  /// <param name="type"></param>
  public XmlItemElementAttribute(Type? type) : base(type)
  {
  }

  /// <summary>
  /// A method of the container to add an item on deserialization.
  /// </summary>
  public string? AddMethod { get; set; }

  /// <summary>
  /// Type of the converter type used to convert an item on serialization/deserialization.
  /// </summary>
  public Type? ConverterType { get; set; }

  /// <summary>
  /// Arguments to be passed to the type converter.
  /// </summary>
  public object[]? Args { get; set; }
}
namespace Qhta.Xml;

/// <summary>
/// Defines an attribute which can specify data of dictionary property, field or class that are needed for serialization/deserialization.
/// Extends <see cref="XmlCollectionAttribute"/>
/// </summary>

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class)]
public class XmlDictionaryAttribute : XmlCollectionAttribute
{
  /// <summary>
  /// Default constructor
  /// </summary>
  public XmlDictionaryAttribute()
  {
  }

  /// <summary>
  /// Initializing constructor
  /// </summary>
  /// <param name="elementName"></param>
  public XmlDictionaryAttribute(string? elementName) : base(elementName)
  {
  }

  /// <summary>
  /// Specifies a type of the key in the dictionary key-value pair.
  /// </summary>
  public Type? KeyType { get; set; }

  /// <summary>
  /// Specifies a name of the key in the dictionary key-value pair.
  /// </summary>
  public string? KeyName { get; set; }

  /// <summary>
  /// Specifies a type of the value in the dictionary key-value pair.
  /// </summary>
  public Type? ValueType { get; set; }

  /// <summary>
  /// Specifies that keys are serialized/deserialized as XML attributes.
  /// </summary>
  public bool AttributesAreKeys { get; set; }

  /// <summary>
  /// Specifies that keys are serialized/deserialized as XML elements.
  /// </summary>
  public bool ElementsAreKeys { get; set; }
}
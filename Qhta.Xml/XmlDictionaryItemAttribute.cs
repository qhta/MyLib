namespace Qhta.Xml;

/// <summary>
/// Defines an attribute which can specify item data of dictionary property, field or class that are needed for serialization/deserialization.
/// There may be multiple such attributes declared for a single dictionary.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true)]
public class XmlDictionaryItemAttribute : XmlItemElementAttribute
{
  /// <summary>
  /// Default constructor.
  /// </summary>
  /// <param name="elementName"></param>
  public XmlDictionaryItemAttribute(string elementName) : base(elementName)
  {
  }

  /// <summary>
  /// Initializing constructor with key name and item type.
  /// </summary>
  /// <param name="elementName"></param>
  /// <param name="keyName"></param>
  /// <param name="itemType"></param>
  public XmlDictionaryItemAttribute(string elementName, string keyName, Type? itemType = null) : base(elementName, itemType)
  {
    KeyAttributeName = keyName;
  }

  /// <summary>
  /// Initializing constructor with item type and key name.
  /// </summary>
  /// <param name="elementName"></param>
  /// <param name="itemType"></param>
  /// <param name="keyName"></param>
  public XmlDictionaryItemAttribute(string elementName, Type? itemType, string? keyName = null) : base(elementName, itemType)
  {
    KeyAttributeName = keyName;
  }

  /// <summary>
  /// Name of the key attribute.
  /// </summary>
  public string? KeyAttributeName { get; }

  /// <summary>
  /// Name of the value attribute.
  /// </summary>
  public string? ValueAttributeName { get; }
}
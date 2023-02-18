namespace Qhta.Xml.Reflection;

/// <summary>
/// Represents information on content item that is a dictionary
/// </summary>
/// <seealso cref="Qhta.Xml.Reflection.ContentItemInfo" />
public class DictionaryInfo : ContentItemInfo
{
  /// <summary>
  /// Gets or sets the key type information.
  /// </summary>
  /// <value>
  /// The key type information.
  /// </value>
  [XmlAttribute]
  [XmlReference]
  public SerializationTypeInfo? KeyTypeInfo { get; set; }

  /// <summary>
  /// Gets or sets the value type information.
  /// </summary>
  /// <value>
  /// The value type information.
  /// </value>
  [XmlAttribute]
  [XmlReference]
  public SerializationTypeInfo? ValueTypeInfo { get; set; }

  /// <summary>
  /// Gets or sets the name of the key.
  /// </summary>
  /// <value>
  /// The name of the key.
  /// </value>
  [XmlAttribute] public string? KeyName { get; set; }

  /// <summary>
  /// Gets or sets the key property.
  /// </summary>
  /// <value>
  /// The key property.
  /// </value>
  [XmlIgnore] public PropertyInfo? KeyProperty { get; set; }
}
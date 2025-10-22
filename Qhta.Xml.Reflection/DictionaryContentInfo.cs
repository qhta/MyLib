namespace Qhta.Xml.Reflection;

/// <summary>
/// Represents information on content of the dictionary type.
/// </summary>
public class DictionaryContentInfo : CollectionContentInfo
{

  /// <summary>
  /// The key type information.
  /// </summary>
  [XmlAttribute]
  [XmlReference]
  public SerializationTypeInfo? KeyTypeInfo { get; set; }

  /// <summary>
  /// Gets or sets the name of the key.
  /// </summary>
  /// <value>
  /// The name of the key.
  /// </value>
  [XmlAttribute] public string? KeyName { get; set; }

  /// <summary>
  /// Determines whether the specified <see cref="ContentInfo"/> is equal to the current <see cref="ContentInfo"/>.
  /// </summary>
  public override bool Equals(ContentInfo? other)
  {
    return other is DictionaryContentInfo dictionaryContentInfo &&
           base.Equals(other) &&
           object.Equals(KeyTypeInfo, dictionaryContentInfo.KeyTypeInfo) &&
           KeyName == dictionaryContentInfo.KeyName;
  }
}
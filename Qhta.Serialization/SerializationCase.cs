namespace Qhta.Xml.Serialization;

/// <summary>
/// Specifies modes for name case change.
/// </summary>
public enum SerializationCase
{
  /// <summary>
  /// Leave unchanged
  /// </summary>
  Unchanged,
  /// <summary>
  /// Turn first letter to lowercase
  /// </summary>
  LowercaseFirstLetter,
  /// <summary>
  /// Turn first letter to uppercase
  /// </summary>
  UppercaseFirstLetter
}
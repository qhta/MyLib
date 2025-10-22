namespace Qhta.Xml.Reflection;

/// <summary>
/// Information on item type for array types
/// </summary>
public class ArrayContentInfo : ContentInfo
{
  /// <summary>
  /// The value type information.
  /// </summary>
  [XmlAttribute]
  [XmlReference]
  public SerializationTypeInfo? ValueTypeInfo { get; set; }

  /// <summary>
  /// Determines whether the specified <see cref="ContentInfo"/> is equal to the current <see cref="ContentInfo"/>.
  /// </summary>
  public override bool Equals(ContentInfo? other)
  {
    return other is CollectionContentInfo collectionContentInfo
           && base.Equals(other)
           && object.Equals(ValueTypeInfo, collectionContentInfo.ValueTypeInfo);
  }
}
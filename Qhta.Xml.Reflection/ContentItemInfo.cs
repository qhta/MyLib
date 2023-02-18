using System.Runtime.Serialization;

namespace Qhta.Xml.Reflection;

/// <summary>
/// Information on class item that represents object content
/// </summary>
[KnownType(typeof(DictionaryInfo))]
public class ContentItemInfo : IEquatable<ContentItemInfo>
{
  /// <summary>
  ///   If a collection of objects stores references only.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool StoresReferences { get; set; }

  /// <summary>
  ///   Known types for collection items.
  /// </summary>
  [XmlReferences]
  public KnownItemTypesCollection KnownItemTypes { get;/* set;*/ } = new();


  /// <summary>
  /// Indicates whether the current object is equal to another object of the same type.
  /// </summary>
  /// <param name="other">An object to compare with this object.</param>
  /// <returns>
  ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
  /// </returns>
  public bool Equals(ContentItemInfo? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return StoresReferences == other.StoresReferences && KnownItemTypes.Equals(other.KnownItemTypes);
  }
}
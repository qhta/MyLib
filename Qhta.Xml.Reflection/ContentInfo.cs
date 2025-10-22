namespace Qhta.Xml.Reflection;

/// <summary>
/// Information on type content. It is used for array, collection or dictionary types.
/// </summary>
[KnownType(typeof(CollectionContentInfo))]
[KnownType(typeof(DictionaryContentInfo))]
public class ContentInfo : IEquatable<ContentInfo>
{
  /// <summary>
  ///  If a collection of objects stores references only.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool StoresReferences { get; set; }

  /// <summary>
  ///   Known types for collection items.
  /// </summary>
  [XmlReferences]
  public KnownItemTypesCollection KnownItemTypes { get; } = new();


  /// <summary>
  /// Indicates whether the content info object is equal to another object of the same type.
  /// Checks StoresReferences and KnownItemTypes for equality.
  /// </summary>
  public virtual bool Equals(ContentInfo? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return this.GetType() == other.GetType() &&
      StoresReferences == other.StoresReferences 
      && KnownItemTypes.Equals(other.KnownItemTypes);
  }
}
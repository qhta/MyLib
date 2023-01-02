using System.Runtime.Serialization;

namespace Qhta.Xml.Serialization;

[KnownType(typeof(DictionaryInfo))]
public class ContentItemInfo: IEquatable<ContentItemInfo>
{
  /// <summary>
  /// If a collection of objects stores references only.
  /// </summary>
  [XmlAttribute]
  [DefaultValue(false)]
  public bool StoresReferences { get; set;}

  /// <summary>
  /// Known types for collection items.
  /// </summary>
  [XmlReferences]
  public KnownItemTypesCollection KnownItemTypes { get; set; } = new();

  public bool Equals(ContentItemInfo? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return StoresReferences == other.StoresReferences && KnownItemTypes.Equals(other.KnownItemTypes);
  }

  //public override bool Equals(object? obj)
  //{
  //  if (ReferenceEquals(null, obj)) return false;
  //  if (ReferenceEquals(this, obj)) return true;
  //  if (obj.GetType() != this.GetType()) return false;
  //  return Equals((CollectionInfo)obj);
  //}

  //public override int GetHashCode()
  //{
  //  return HashCode.Combine(IsReferences, KnownItemTypes);
  //}
}
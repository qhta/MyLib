using System.Linq;

namespace Qhta.Xml.Serialization;

public class TypeInfoCollection<TypeNameInfo> : ICollection<TypeNameInfo>, IEquatable<TypeInfoCollection<TypeNameInfo>>
  where TypeNameInfo : class, ITypeNameInfo
{
  [XmlAttribute] public string? DefaultNamespace { get; set; }

  public Dictionary<Type, TypeNameInfo> TypeIndexedItems { get; } = new();

  public SortedDictionary<QualifiedName, TypeNameInfo> FullNameIndexedItems { get; } = new();

  public SortedDictionary<string, TypeNameInfo> ShortNameIndexedItems { get; } = new();

  public List<string> DuplicatedShortNames { get; } = new();

  public void Add(TypeNameInfo item)
  {
    TypeIndexedItems.Add(item.Type, item);
    FullNameIndexedItems.Add(new QualifiedName(item.XmlName, item.XmlNamespace), item);
    if (!ShortNameIndexedItems.ContainsKey(item.XmlName))
      ShortNameIndexedItems.Add(item.XmlName, item);
    else
      DuplicatedShortNames.Add(item.XmlName);

  }

  public void Clear()
  {
    TypeIndexedItems.Clear();
    FullNameIndexedItems.Clear();
  }

  public bool Contains(TypeNameInfo item)
  {
    return TypeIndexedItems.Values.Contains(item);
  }

  public void CopyTo(TypeNameInfo[] array, int arrayIndex)
  {
    TypeIndexedItems.Values.CopyTo(array, arrayIndex);
  }

  public bool Remove(TypeNameInfo item)
  {
    var ok1 = TypeIndexedItems.Remove(item.Type);
    var ok2 = FullNameIndexedItems.Remove(new QualifiedName(item.XmlName, item.ClrNamespace));
    return ok1 || ok2;
  }

  public int Count => TypeIndexedItems.Count;

  public bool IsReadOnly => false;

  public IEnumerator<TypeNameInfo> GetEnumerator()
  {
    foreach (var item in FullNameIndexedItems.Values)
      yield return item;
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  public bool Equals(TypeInfoCollection<TypeNameInfo>? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return DefaultNamespace == other.DefaultNamespace
           && TypeIndexedItems.Equals(other.TypeIndexedItems)
           && FullNameIndexedItems.Equals(other.FullNameIndexedItems);
  }

  public void Add(string name, TypeNameInfo item)
  {
    TypeIndexedItems.Add(item.Type, item);
    FullNameIndexedItems.Add(name, item);
  }

  public bool TryGetValue(Type type, [NotNullWhen(true)][MaybeNullWhen(false)] out TypeNameInfo typeInfo)
  {
    return TypeIndexedItems.TryGetValue(type, out typeInfo);
  }

  public bool TryGetValue(QualifiedName qualifiedName, [NotNullWhen(true)][MaybeNullWhen(false)] out TypeNameInfo typeInfo)
  {
    return FullNameIndexedItems.TryGetValue(qualifiedName, out typeInfo);
  }
  public bool TryGetValue(XmlQualifiedTagName xmlQualifiedTagName, [NotNullWhen(true)][MaybeNullWhen(false)] out TypeNameInfo typeInfo)
  {
    var qualifiedName = new QualifiedName(xmlQualifiedTagName.Name, xmlQualifiedTagName.Namespace);
    return FullNameIndexedItems.TryGetValue(qualifiedName, out typeInfo);
  }

  public bool TryGetValue(string name, [NotNullWhen(true)][MaybeNullWhen(false)] out TypeNameInfo typeInfo)
  {
    if (name == "sbyte")
      TestTools.Stop();
    if (FullNameIndexedItems.TryGetValue(new QualifiedName(name), out typeInfo))
      return true;
    if (ShortNameIndexedItems.TryGetValue(name, out typeInfo))
      return true;
    return false;
  }

  public TypeNameInfo? FindTypeInfo(Type itemType)
  {
    var result = this.FirstOrDefault(item => itemType == item.Type);
    if (result == null)
      result = this.FirstOrDefault(item => itemType.IsSubclassOf(item.Type));
    if (result == null || result.Type == typeof(Object))
      return null;
    return result;
  }

  //public override bool Equals(object? obj)
  //{
  //  if (ReferenceEquals(null, obj)) return false;
  //  if (ReferenceEquals(this, obj)) return true;
  //  if (obj.GetType() != this.GetType()) return false;
  //  return Equals((TypesDictionary<ItemType>)obj);
  //}

  //public override int GetHashCode()
  //{
  //  return Combine(BaseNamespace, TypeIndexedItems, NameIndexedItems);
  //}

  public IEnumerable<Type> Keys => TypeIndexedItems.Keys;

}
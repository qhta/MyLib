﻿using System.Collections;
using static System.HashCode;

namespace Qhta.Xml.Serialization;

public class TypesInfoCollection<TypeNameInfo> : ICollection<TypeNameInfo>, IEquatable<TypesInfoCollection<TypeNameInfo>> where TypeNameInfo: ITypeNameInfo
{
  [XmlAttribute]
  public string? DefaultNamespace { get; set; }

  private Dictionary<Type, TypeNameInfo> TypeIndexedItems { get; set; } = new();

  private SortedDictionary<QualifiedName, TypeNameInfo> NameIndexedItems { get; set; } = new();

  public void Add(TypeNameInfo item)
  {
    TypeIndexedItems.Add(item.Type, item);
    NameIndexedItems.Add(new QualifiedName(item.XmlName, item.XmlNamespace), item);
  }

  public void Clear()
  {
    TypeIndexedItems.Clear();
    NameIndexedItems.Clear();
  }

  public bool Contains(TypeNameInfo item)
    => TypeIndexedItems.Values.Contains(item);

  public void CopyTo(TypeNameInfo[] array, int arrayIndex)
    => TypeIndexedItems.Values.CopyTo(array, arrayIndex);

  public bool Remove(TypeNameInfo item)
  {
    var ok1 = TypeIndexedItems.Remove(item.Type);
    var ok2 = NameIndexedItems.Remove(new QualifiedName(item.XmlName, item.ClrNamespace));
    return ok1 || ok2;
  }

  public int Count => TypeIndexedItems.Count;

  public bool IsReadOnly => false;

  public bool TryGetValue(Type type, [MaybeNullWhen(false)] out TypeNameInfo typeInfo)
    => TypeIndexedItems.TryGetValue(type, out typeInfo);

  public bool TryGetValue(QualifiedName qualifiedName, [MaybeNullWhen(false)] out TypeNameInfo typeInfo)
    => NameIndexedItems.TryGetValue(qualifiedName, out typeInfo);

  public bool TryGetValue(string name, [MaybeNullWhen(false)] out TypeNameInfo typeInfo)
  {
    if (name == "OrderedItem")
      TestTools.Stop();
    if (NameIndexedItems.TryGetValue(new QualifiedName(name), out typeInfo))
      return true;
    if (DefaultNamespace != null)
      if (NameIndexedItems.TryGetValue(new QualifiedName(name, DefaultNamespace), out typeInfo))
        return true;
    return false;
  }
  public IEnumerator<TypeNameInfo> GetEnumerator()
  {
    foreach (var item in NameIndexedItems.Values)
      yield return item;
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  public TypeNameInfo? FindTypeInfo(Type itemType)
  {
    var result = (this as IEnumerable<TypeNameInfo>).FirstOrDefault(item => itemType == item.Type);
    if (result == null)
      result = (this as IEnumerable<TypeNameInfo>).FirstOrDefault(item => itemType.IsSubclassOf(item.Type));
    return result;
  }

  public bool Equals(TypesInfoCollection<TypeNameInfo>? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return DefaultNamespace == other.DefaultNamespace 
           && TypeIndexedItems.Equals(other.TypeIndexedItems) 
           && NameIndexedItems.Equals(other.NameIndexedItems);
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
}
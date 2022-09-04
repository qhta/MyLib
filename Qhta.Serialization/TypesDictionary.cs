namespace Qhta.Xml.Serialization;

public class TypesDictionary<ItemType>: IEnumerable<ItemType>, IDictionary<QualifiedName, ItemType>, IDictionary<Type, ItemType> where ItemType : ITypeInfo
{
  private readonly Dictionary<Type, ItemType> TypeIndexedItems = new Dictionary<Type, ItemType>();
  private readonly SortedDictionary<QualifiedName, ItemType> NameIndexedItems = new SortedDictionary<QualifiedName, ItemType>();

  public ItemType? FindTypeInfo(Type itemType)
  {
    var result = (this as IEnumerable<ItemType>).FirstOrDefault(item => itemType == item.Type);
    if (result == null)
      result = (this as IEnumerable<ItemType>).FirstOrDefault(item => itemType.IsSubclassOf(item.Type));
    return result;
  }

  public IEnumerable<QualifiedName> KnownTags(Type type)
  {
    foreach (var item in NameIndexedItems)
      if (type == item.Value.Type)
        yield return item.Key;
  }

  public void Add(ItemType value)
  {
    if (!TypeIndexedItems.ContainsKey(value.Type))
      TypeIndexedItems.Add(value.Type, value);
    var key = value.Name;
    if (!NameIndexedItems.ContainsKey(key)) 
      (NameIndexedItems).Add(key, value);
  }

  public void Add(string elementName, ItemType value)
  {
    if (!TypeIndexedItems.ContainsKey(value.Type))
      TypeIndexedItems.Add(value.Type, value);
    (NameIndexedItems).Add(new QualifiedName(elementName), value);
  }

  public void Add(QualifiedName key, ItemType value)
  {
    if (!TypeIndexedItems.ContainsKey(value.Type))
      TypeIndexedItems.Add(value.Type, value);
    (NameIndexedItems).Add(key, value);
  }

  public bool ContainsKey(QualifiedName key)
  {
    return (NameIndexedItems).ContainsKey(key);
  }

  public bool Remove(QualifiedName key)
  {
    if (NameIndexedItems.TryGetValue(key, out var typeInfo))
      TypeIndexedItems.Remove(typeInfo.Type);
    return NameIndexedItems.Remove(key);
  }

  public bool TryGetValue(string elementName, [MaybeNullWhen(false)] out ItemType value)
  {
    if (NameIndexedItems.TryGetValue(new QualifiedName(elementName), out value))
      return true;
    var candidates = NameIndexedItems.Where(item => item.Key.Name == elementName).ToArray();
    if (candidates.Length == 1)
    {
      value = candidates.First().Value;
      return true;
    }
    return false;
  }

  public bool TryGetValue(QualifiedName key, [MaybeNullWhen(false)] out ItemType value)
  {
    return NameIndexedItems.TryGetValue(key, out value);
  }

  public ItemType this[QualifiedName key] { get => (NameIndexedItems)[key]; set => (NameIndexedItems)[key] = value; }

  public ICollection<QualifiedName> Keys => (NameIndexedItems).Keys;

  public ICollection<ItemType> Values => (TypeIndexedItems).Values;

  public void Add(KeyValuePair<QualifiedName, ItemType> item)
  {
    ((ICollection<KeyValuePair<QualifiedName, ItemType>>)NameIndexedItems).Add(item);
  }

  public void Clear()
  {
    NameIndexedItems.Clear();
    TypeIndexedItems.Clear();
  }

  public bool Contains(KeyValuePair<QualifiedName, ItemType> item)
  {
    return ((ICollection<KeyValuePair<QualifiedName, ItemType>>)NameIndexedItems).Contains(item);
  }

  public void CopyTo(KeyValuePair<QualifiedName, ItemType>[] array, int arrayIndex)
  {
    ((ICollection<KeyValuePair<QualifiedName, ItemType>>)NameIndexedItems).CopyTo(array, arrayIndex);
  }

  public bool Remove(KeyValuePair<QualifiedName, ItemType> item)
  {
    return ((ICollection<KeyValuePair<QualifiedName, ItemType>>)NameIndexedItems).Remove(item);
  }

  public int Count => Math.Max(NameIndexedItems.Count, TypeIndexedItems.Count);

  public bool IsReadOnly => ((ICollection<KeyValuePair<QualifiedName, ItemType>>)NameIndexedItems).IsReadOnly;

  public IEnumerator<KeyValuePair<QualifiedName, ItemType>> GetEnumerator()
  {
    return ((IEnumerable<KeyValuePair<QualifiedName, ItemType>>)NameIndexedItems).GetEnumerator();
  }

  //IEnumerator IEnumerable.GetEnumerator()
  //{
  //  return ((IEnumerable)StringIndexedItems).GetEnumerator();
  //}

  public void Add(Type key, ItemType value)
  {
    ((IDictionary<Type, ItemType>)TypeIndexedItems).Add(key, value);
  }

  public bool ContainsKey(Type type)
  {
    return TypeIndexedItems.ContainsKey(type);
  }

  public bool Remove(Type type)
  {
    return TypeIndexedItems.Remove(type);
  }

  public bool TryGetValue(Type type, /*[MaybeNullWhen(false)]*/ out ItemType value)
  {
    return TypeIndexedItems.TryGetValue(type, out value);
  }

  IEnumerator<ItemType> IEnumerable<ItemType>.GetEnumerator()
  {
    return Values.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return ((IEnumerable)Values).GetEnumerator();
  }

  public ItemType this[Type key] { get => ((IDictionary<Type, ItemType>)TypeIndexedItems)[key]; set => ((IDictionary<Type, ItemType>)TypeIndexedItems)[key] = value; }

  ICollection<Type> IDictionary<Type, ItemType>.Keys => ((IDictionary<Type, ItemType>)TypeIndexedItems).Keys;

  public void Add(KeyValuePair<Type, ItemType> item)
  {
    ((ICollection<KeyValuePair<Type, ItemType>>)TypeIndexedItems).Add(item);
  }

  public bool Contains(KeyValuePair<Type, ItemType> item)
  {
    return ((ICollection<KeyValuePair<Type, ItemType>>)TypeIndexedItems).Contains(item);
  }

  public void CopyTo(KeyValuePair<Type, ItemType>[] array, int arrayIndex)
  {
    ((ICollection<KeyValuePair<Type, ItemType>>)TypeIndexedItems).CopyTo(array, arrayIndex);
  }

  public bool Remove(KeyValuePair<Type, ItemType> item)
  {
    return ((ICollection<KeyValuePair<Type, ItemType>>)TypeIndexedItems).Remove(item);
  }

  IEnumerator<KeyValuePair<Type, ItemType>> IEnumerable<KeyValuePair<Type, ItemType>>.GetEnumerator()
  {
    return ((IEnumerable<KeyValuePair<Type, ItemType>>)TypeIndexedItems).GetEnumerator();
  }
}
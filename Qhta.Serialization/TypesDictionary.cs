namespace Qhta.Xml.Serialization;

public class TypesDictionary<ItemType> : ICollection<ItemType> where ItemType: ITypeInfo
{
  [XmlAttribute]
  public string? BaseNamespace { get; set; }

  private Dictionary<Type, ItemType> TypeIndexedItems { get; set; } = new();

  private SortedDictionary<QualifiedName, ItemType> NameIndexedItems { get; set; } = new();

  public void Add(ItemType item)
  {
    TypeIndexedItems.Add(item.Type, item);
    NameIndexedItems.Add(item.Name, item);
  }

  public void Clear()
  {
    TypeIndexedItems.Clear();
    NameIndexedItems.Clear();
  }

  public bool Contains(ItemType item)
    => TypeIndexedItems.Values.Contains(item);

  public void CopyTo(ItemType[] array, int arrayIndex)
    => TypeIndexedItems.Values.CopyTo(array, arrayIndex);

  public bool Remove(ItemType item)
  {
    var ok1 = TypeIndexedItems.Remove(item.Type);
    var ok2 = NameIndexedItems.Remove(item.Name);
    return ok1 || ok2;
  }

  public int Count => TypeIndexedItems.Count;

  public bool IsReadOnly => false;

  public bool TryGetValue(Type type, [MaybeNullWhen(false)] out ItemType typeInfo)
    => TypeIndexedItems.TryGetValue(type, out typeInfo);

  public bool TryGetValue(QualifiedName qualifiedName, [MaybeNullWhen(false)] out ItemType typeInfo)
    => NameIndexedItems.TryGetValue(qualifiedName, out typeInfo);

  public bool TryGetValue(string name, [MaybeNullWhen(false)] out ItemType typeInfo)
  {
    if (NameIndexedItems.TryGetValue(new QualifiedName(name), out typeInfo))
      return true;
    if (BaseNamespace != null)
      if (NameIndexedItems.TryGetValue(new QualifiedName(name, BaseNamespace), out typeInfo))
        return true;
    return false;
  }
  public IEnumerator<ItemType> GetEnumerator()
  {
    foreach (var item in NameIndexedItems.Values)
      yield return item;
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  public ItemType? FindTypeInfo(Type itemType)
  {
    var result = (this as IEnumerable<ItemType>).FirstOrDefault(item => itemType == item.Type);
    if (result == null)
      result = (this as IEnumerable<ItemType>).FirstOrDefault(item => itemType.IsSubclassOf(item.Type));
    return result;
  }
}
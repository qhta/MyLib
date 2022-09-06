namespace Qhta.Xml.Serialization;

public class KnownItemTypesDictionary: TypesDictionary<SerializationItemInfo>
{
  //private Dictionary<Type, SerializationItemInfo> TypeIndexedItems { get; set; } = new();

  //private SortedDictionary<QualifiedName, SerializationItemInfo> NameIndexedItems { get; set; } = new();

  //public void Add(SerializationItemInfo item)
  //{
  //  TypeIndexedItems.Add(item.Type, item);
  //  NameIndexedItems.Add(item.Name, item);
  //}

  //public void Clear()
  //{
  //  TypeIndexedItems.Clear();
  //  NameIndexedItems.Clear();
  //}

  //public bool Contains(SerializationItemInfo item)
  //  => TypeIndexedItems.Values.Contains(item);

  //public void CopyTo(SerializationItemInfo[] array, int arrayIndex)
  //  => TypeIndexedItems.Values.CopyTo(array, arrayIndex);

  //public bool Remove(SerializationItemInfo item)
  //{
  //  var ok1 = TypeIndexedItems.Remove(item.Type);
  //  var ok2 = NameIndexedItems.Remove(item.Name);
  //  return ok1 || ok2;
  //}

  //public int Count => TypeIndexedItems.Count;

  //public bool IsReadOnly => false;

  //public bool TryGetValue(Type type, [MaybeNullWhen(false)] out SerializationItemInfo typeInfo)
  //  => TypeIndexedItems.TryGetValue(type, out typeInfo);

  //public bool TryGetValue(QualifiedName qualifiedName, [MaybeNullWhen(false)] out SerializationItemInfo typeInfo)
  //  => NameIndexedItems.TryGetValue(qualifiedName, out typeInfo);

  //public bool TryGetValue(string name, [MaybeNullWhen(false)] out SerializationItemInfo typeInfo)
  //{
  //  if (NameIndexedItems.TryGetValue(new QualifiedName(name), out typeInfo))
  //    return true;
  //  //if (BaseNamespace != null)
  //  //  if (NameIndexedItems.TryGetValue(new QualifiedName(name, BaseNamespace), out typeInfo))
  //  //    return true;
  //  return false;
  //}
  //public IEnumerator<SerializationItemInfo> GetEnumerator()
  //{
  //  foreach (var item in TypeIndexedItems.Values)
  //    yield return item;
  //}

  //IEnumerator IEnumerable.GetEnumerator()
  //{
  //  return GetEnumerator();
  //}

  //public SerializationItemInfo? FindTypeInfo(Type itemType)
  //{
  //  var result = (this as IEnumerable<SerializationItemInfo>).FirstOrDefault(item => itemType == item.Type);
  //  if (result == null)
  //    result = (this as IEnumerable<SerializationItemInfo>).FirstOrDefault(item => itemType.IsSubclassOf(item.Type));
  //  return result;
  //}

}
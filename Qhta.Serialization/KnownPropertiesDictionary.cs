using System.Collections.Generic;
using System.Linq;

namespace Qhta.Xml.Serialization;

public class KnownPropertiesDictionary : ICollection<SerializationPropertyInfo>
{
  private static readonly PropOrderComparer propertyInfoOrderComparer = new PropOrderComparer();

  private SortedSet<SerializationPropertyInfo> OrderedItems { get; set; } = new();
  private SortedDictionary<QualifiedName, SerializationPropertyInfo> NameIndexedItems { get; set; } = new();


  public void Add(SerializationPropertyInfo item)
  {
    OrderedItems.Add(item);
    NameIndexedItems.Add(item.Name, item);
  }

  public void Add(QualifiedName name, SerializationPropertyInfo item)
  {
    OrderedItems.Add(item);
    NameIndexedItems.Add(name, item);
  }

  public void Add(string name, SerializationPropertyInfo item)
  {
    OrderedItems.Add(item);
    NameIndexedItems.Add(new QualifiedName(name), item);
  }

  public void Clear()
  {
    OrderedItems.Clear();
    NameIndexedItems.Clear();
  }

  public bool Contains(SerializationPropertyInfo item)
    => OrderedItems.Contains(item);

  public bool ContainsKey(QualifiedName name)
    => NameIndexedItems.ContainsKey(name);

  public bool ContainsKey(string name)
    => NameIndexedItems.ContainsKey(new QualifiedName(name));

  public void CopyTo(SerializationPropertyInfo[] array, int arrayIndex)
    => OrderedItems.CopyTo(array, arrayIndex);

  public bool Remove(SerializationPropertyInfo item)
  {
    var ok1 = OrderedItems.Remove(item);
    var ok2 = NameIndexedItems.Remove(item.Name);
    return ok1 || ok2;
  }

  public int Count => OrderedItems.Count;

  public bool IsReadOnly => false;

  public bool TryGetValue(QualifiedName qualifiedName, [MaybeNullWhen(false)] out SerializationPropertyInfo typeInfo)
    => NameIndexedItems.TryGetValue(qualifiedName, out typeInfo);

  public bool TryGetValue(string name, [MaybeNullWhen(false)] out SerializationPropertyInfo typeInfo)
  {
    if (NameIndexedItems.TryGetValue(new QualifiedName(name), out typeInfo))
      return true;
    //if (BaseNamespace != null)
    //  if (NameIndexedItems.TryGetValue(new QualifiedName(name, BaseNamespace), out typeInfo))
    //    return true;
    return false;
  }
  public IEnumerator<SerializationPropertyInfo> GetEnumerator()
  {
    foreach (var item in OrderedItems)
      yield return item;
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

}
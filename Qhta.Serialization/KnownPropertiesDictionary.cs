using System.Collections.Generic;
using System.Linq;

namespace Qhta.Xml.Serialization;

public class KnownPropertiesDictionary : ICollection<SerializationMemberInfo>
{
  private static readonly PropOrderComparer propertyInfoOrderComparer = new PropOrderComparer();

  private List<SerializationMemberInfo> Items { get; set; } = new();
  private SortedSet<SerializationMemberInfo> OrderedItems { get; set; } = new();
  private SortedDictionary<QualifiedName, SerializationMemberInfo> NameIndexedItems { get; set; } = new();


  public void Add(SerializationMemberInfo item)
  {
    Items.Add(item);
    OrderedItems.Add(item);
    NameIndexedItems.Add(item.Name, item);
  }

  public void Add(QualifiedName name, SerializationMemberInfo item)
  {
    Items.Add(item);
    OrderedItems.Add(item);
    NameIndexedItems.Add(name, item);
  }

  public void Add(string name, SerializationMemberInfo item)
  {
    Items.Add(item);
    OrderedItems.Add(item);
    NameIndexedItems.Add(new QualifiedName(name), item);
  }

  public void Clear()
  {
    Items.Clear();
    OrderedItems.Clear();
    NameIndexedItems.Clear();
  }

  public bool Contains(SerializationMemberInfo item)
    => Items.Contains(item);

  public bool ContainsKey(QualifiedName name)
    => NameIndexedItems.ContainsKey(name);

  public bool ContainsKey(string name)
    => NameIndexedItems.ContainsKey(new QualifiedName(name));

  public void CopyTo(SerializationMemberInfo[] array, int arrayIndex)
    => Items.CopyTo(array, arrayIndex);

  public bool Remove(SerializationMemberInfo item)
  {
    var ok0 = Items.Remove(item);
    var ok1 = OrderedItems.Remove(item);
    var ok2 = NameIndexedItems.Remove(item.Name);
    return ok0 || ok1 || ok2;
  }

  public int Count => Items.Count;

  public bool IsReadOnly => false;

  public bool TryGetValue(QualifiedName qualifiedName, [MaybeNullWhen(false)] out SerializationMemberInfo typeInfo)
    => NameIndexedItems.TryGetValue(qualifiedName, out typeInfo);

  public bool TryGetValue(string name, [MaybeNullWhen(false)] out SerializationMemberInfo typeInfo)
  {
    if (NameIndexedItems.TryGetValue(new QualifiedName(name), out typeInfo))
      return true;
    //if (BaseNamespace != null)
    //  if (NameIndexedItems.TryGetValue(new QualifiedName(name, BaseNamespace), out typeInfo))
    //    return true;
    return false;
  }
  public IEnumerator<SerializationMemberInfo> GetEnumerator()
  {
    foreach (var item in Items)
      yield return item;
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

}
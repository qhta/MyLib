using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Qhta.Xml.Serialization;

public class KnownMembersCollection : ICollection<SerializationMemberInfo>
{
  private static readonly PropOrderComparer propertyInfoOrderComparer = new PropOrderComparer();

  private ObservableCollection<SerializationMemberInfo> Items { get; set; } = new();
  private SortedSet<SerializationMemberInfo> OrderedItems { get; set; } = new();
  private SortedDictionary<QualifiedName, SerializationMemberInfo> NameIndexedItems { get; set; } = new();

  public KnownMembersCollection()
  {
    //Items.CollectionChanged += Items_CollectionChanged;
  }

  //private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
  //{
  //  if (args.Action == NotifyCollectionChangedAction.Add)
  //    if (args.NewItems!=null)
  //      foreach (var item in args.NewItems.Cast<SerializationMemberInfo>())
  //        if (item.XmlName == "Value")
  //          Debug.Assert(true);
  //}

  public void Add(SerializationMemberInfo item)
  {
    Items.Add(item);
    OrderedItems.Add(item);
    NameIndexedItems.Add(new QualifiedName(item.XmlName), item);
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
    var ok2 = NameIndexedItems.Remove(item.QualifiedName);
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

  public void Dump(string header, KnownNamespacesCollection KnownNamespaces)
  {
    if (Count==0) return;
    Debug.WriteLine(header);
    Debug.Indent();
    foreach (var item in this)
    {
      KnownNamespaces.XmlNamespaceToPrefix.TryGetValue(item.XmlNamespace ?? "", out var prefix);
      if (item.Member is FieldInfo field)
        Debug.WriteLine($"{prefix}:{item.XmlName} = field {field.Name}: {item.MemberType}");
      if (item.Member is PropertyInfo property)
        Debug.WriteLine($"{prefix}:{item.XmlName} = property {property.Name}: {item.MemberType}");
    }
    Debug.Unindent();
  }
}
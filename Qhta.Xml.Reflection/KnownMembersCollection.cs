﻿namespace Qhta.Xml.Reflection;

/// <summary>
/// Named collection of serialization member info.
/// </summary>
public class KnownMembersCollection : ICollection<SerializationMemberInfo>, IMembersDictionary
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  /// <summary>
  /// Initializing constructor
  /// </summary>
  /// <param name="ownerTypeInfo"></param>
  public KnownMembersCollection(SerializationTypeInfo ownerTypeInfo)
  {
    Owner = ownerTypeInfo;
  }


  //private static PropOrderComparer propertyInfoOrderComparer { get; } = new();
  private ObservableCollection<SerializationMemberInfo> Items { get; } = new();
  private SortedSet<SerializationMemberInfo> OrderedItems { get; } = new();
  private SortedDictionary<QualifiedName, SerializationMemberInfo> NameIndexedItems { get; } = new();

  public SerializationTypeInfo Owner { get; }

  public void Add(SerializationMemberInfo item)
  {
    Items.Add(item);
    OrderedItems.Add(item);
    NameIndexedItems.Add(new QualifiedName(item.XmlName), item);
  }

  public void Clear()
  {
    Items.Clear();
    OrderedItems.Clear();
    NameIndexedItems.Clear();
  }

  public bool Contains(SerializationMemberInfo item)
  {
    return Items.Contains(item);
  }

  public void CopyTo(SerializationMemberInfo[] array, int arrayIndex)
  {
    Items.CopyTo(array, arrayIndex);
  }

  public bool Remove(SerializationMemberInfo item)
  {
    var ok0 = Items.Remove(item);
    var ok1 = OrderedItems.Remove(item);
    var ok2 = NameIndexedItems.Remove(item.QualifiedName);
    return ok0 || ok1 || ok2;
  }

  public int Count => Items.Count;

  public bool IsReadOnly => false;

  public IEnumerator<SerializationMemberInfo> GetEnumerator()
  {
    foreach (var item in Items)
      yield return item;
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
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

  public bool ContainsKey(QualifiedName name)
  {
    return NameIndexedItems.ContainsKey(name);
  }

  public bool ContainsKey(string name)
  {
    return NameIndexedItems.ContainsKey(new QualifiedName(name));
  }

  public bool TryGetValue(QualifiedName qualifiedName, [MaybeNullWhen(false)] out SerializationMemberInfo typeInfo)
  {
    return NameIndexedItems.TryGetValue(qualifiedName, out typeInfo);
  }

  public bool TryGetValue(string name, [MaybeNullWhen(false)] out SerializationMemberInfo typeInfo)
  {
    if (NameIndexedItems.TryGetValue(new QualifiedName(name), out typeInfo))
      return true;
    //if (BaseNamespace != null)
    //  if (NameIndexedItems.TryGetValue(new QualifiedName(name, BaseNamespace), out typeInfo))
    //    return true;
    return false;
  }

  /// <summary>
  /// Dumps collection to debug output window
  /// </summary>
  /// <param name="header">The header.</param>
  /// <param name="KnownNamespaces">The known namespaces.</param>
  public void Dump(string header, KnownNamespacesCollection KnownNamespaces)
  {
    if (Count == 0) return;
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
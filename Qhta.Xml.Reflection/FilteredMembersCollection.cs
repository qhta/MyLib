namespace Qhta.Xml.Reflection;

/// <summary>
/// Named collection of serialization member info.
/// </summary>
public class FilteredMembersCollection : IEnumerable<SerializationMemberInfo>, IMembersDictionary
{
  private KnownMembersCollection OriginalMembersCollection;

  private Func<SerializationMemberInfo, bool> Predicate;

  /// <summary>
  /// Constructor that forces internal fields initialization
  /// </summary>
  /// <param name="originalMembersCollection">Original members collection which is filtered</param>
  /// <param name="predicate">A predicate on each original members collection item</param>
  public FilteredMembersCollection(KnownMembersCollection originalMembersCollection, Func<SerializationMemberInfo, bool> predicate)
  {
    OriginalMembersCollection = originalMembersCollection;
    Predicate = predicate;
  }

  /// <summary>
  /// Returns an enumerator based on original members collection filtered by predicate.
  /// </summary>
  public IEnumerator<SerializationMemberInfo> GetEnumerator()
  {
    return OriginalMembersCollection.Where(Predicate).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }


  public bool ContainsKey(QualifiedName name)
  {
    Original
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
using System.Linq;

namespace Qhta.Xml.Reflection;

/// <summary>
/// Collection of type name info, which is indexed by type, long name and by short name.
/// </summary>
/// <typeparam name="TypeNameInfo">The type of the ype name information.</typeparam>
public class TypeInfoCollection<TypeNameInfo> : ICollection<TypeNameInfo>, IEquatable<TypeInfoCollection<TypeNameInfo>>
  where TypeNameInfo : class, ITypeNameInfo
{

  /// <summary>
  /// Dictionary of items indexed by type.
  /// </summary>
  /// <value>
  /// The type indexed items.
  /// </value>
  public Dictionary<Type, TypeNameInfo> TypeIndexedItems { get; } = new();

  /// <summary>
  /// Dictionary of items indexed by full name.
  /// </summary>
  /// <value>
  /// The full name indexed items.
  /// </value>
  public SortedDictionary<QualifiedName, TypeNameInfo> FullNameIndexedItems { get; } = new();

  /// <summary>
  /// Dictionary of items indexed by short name. 
  /// If some types have same names (and different namespaces), 
  /// they are registered in <see cref="DuplicatedShortNames"/>
  /// </summary>
  /// <value>
  /// The short name indexed items.
  /// </value>
  public SortedDictionary<string, TypeNameInfo> ShortNameIndexedItems { get; } = new();

  /// <summary>
  /// Gets the duplicated short type names.
  /// The concrete classes can be found in <see cref="FullNameIndexedItems"/>
  /// </summary>
  /// <value>
  /// The duplicated short names.
  /// </value>
  public List<string> DuplicatedShortNames { get; } = new();

  /// <summary>
  /// Adds an item to collection.
  /// </summary>
  /// <param name="item">The object to add. />.</param>
  public void Add(TypeNameInfo item)
  {
    TypeIndexedItems.Add(item.Type, item);
    FullNameIndexedItems.Add(new QualifiedName(item.XmlName, item.XmlNamespace), item);
    if (!ShortNameIndexedItems.ContainsKey(item.XmlName))
      ShortNameIndexedItems.Add(item.XmlName, item);
    else
      DuplicatedShortNames.Add(item.XmlName);

  }

  /// <summary>
  /// Removes all items from the collection.
  /// </summary>
  public void Clear()
  {
    TypeIndexedItems.Clear();
    FullNameIndexedItems.Clear();
  }

  /// <summary>
  /// Determines whether the collection contains the object.
  /// </summary>
  /// <param name="item">The object to locate.</param>
  public bool Contains(TypeNameInfo item)
  {
    return TypeIndexedItems.Values.Contains(item);
  }

  /// <summary>
  /// Copies the elements of the collection to an array.
  /// </summary>
  /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from collection. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
  /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
  public void CopyTo(TypeNameInfo[] array, int arrayIndex)
  {
    TypeIndexedItems.Values.CopyTo(array, arrayIndex);
  }

  /// <summary>
  /// Removes the  occurrence of a specific object from the collection />.
  /// </summary>
  /// <param name="item">The object to remove from the collection.</param>
  /// <returns>
  ///   <see langword="true" /> if <paramref name="item" /> was successfully removed from the collection; otherwise, <see langword="false" />. 
  ///   This method also returns <see langword="false" /> if <paramref name="item" /> is not found in the original collection.
  /// </returns>
  public bool Remove(TypeNameInfo item)
  {
    var ok1 = TypeIndexedItems.Remove(item.Type);
    var ok2 = FullNameIndexedItems.Remove(new QualifiedName(item.XmlName, item.ClrNamespace));
    return ok1 || ok2;
  }

  /// <summary>
  /// Gets the number of elements contained in the collection.
  /// </summary>
  public int Count => TypeIndexedItems.Count;

  /// <summary>
  /// Gets a value indicating whether the collection is read-only.
  /// </summary>
  public bool IsReadOnly => false;

  /// <summary>
  /// Returns an enumerator that iterates through the collection.
  /// </summary>
  /// <returns>
  /// An enumerator that can be used to iterate through the collection.
  /// </returns>
  public IEnumerator<TypeNameInfo> GetEnumerator()
  {
    foreach (var item in FullNameIndexedItems.Values)
      yield return item;
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  /// <summary>
  /// Indicates whether the current object is equal to another object of the same type.
  /// </summary>
  /// <param name="other">An object to compare with this object.</param>
  /// <returns>
  ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; 
  ///   otherwise, <see langword="false" />.
  /// </returns>
  public bool Equals(TypeInfoCollection<TypeNameInfo>? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return TypeIndexedItems.Equals(other.TypeIndexedItems)
           && FullNameIndexedItems.Equals(other.FullNameIndexedItems);
  }

  /// <summary>
  /// Tries to get the element from the collection indexed by type.
  /// </summary>
  /// <param name="type">The type of the searched element.</param>
  /// <param name="typeInfo">variable to hold the element id found.</param>
  /// <returns>
  ///   <see langword="true" /> if the element is found; otherwise, <see langword="false" />.
  /// </returns>
  public bool TryGetValue(Type type, [NotNullWhen(true)][MaybeNullWhen(false)] out TypeNameInfo typeInfo)
  {
    return TypeIndexedItems.TryGetValue(type, out typeInfo);
  }

  /// <summary>
  /// Tries to get the element from the collection indexed by full name.
  /// </summary>
  /// <param name="qualifiedName">The qualified name of the searched element.</param>
  /// <param name="typeInfo">variable to hold the element id found.</param>
  /// <returns>
  ///   <see langword="true" /> if the element is found; otherwise, <see langword="false" />.
  /// </returns>
  public bool TryGetValue(QualifiedName qualifiedName, [NotNullWhen(true)][MaybeNullWhen(false)] out TypeNameInfo typeInfo)
  {
    return FullNameIndexedItems.TryGetValue(qualifiedName, out typeInfo);
  }

  /// <summary>
  /// Tries to get the element from the collection indexed by full or short name.
  /// First the full name index is searched, and if not found then the short name index.
  /// </summary>
  /// <param name="tag">The xml qualified tag name of the searched element.</param>
  /// <param name="typeInfo">variable to hold the element id found.</param>
  /// <returns>
  ///   <see langword="true" /> if the element is found; otherwise, <see langword="false" />.
  /// </returns>
  public bool TryGetValue(XmlQualifiedTagName tag, [NotNullWhen(true)][MaybeNullWhen(false)] out TypeNameInfo typeInfo)
  {
    QualifiedName qualifiedName;
    if (String.IsNullOrEmpty(tag.Namespace) && tag.Name.Contains('.'))
      qualifiedName = new QualifiedName(tag.Name);
    else
      qualifiedName = new QualifiedName(tag.Name, tag.Namespace);
    var result = FullNameIndexedItems.TryGetValue(qualifiedName, out typeInfo);
    if (result == false)
      result =  ShortNameIndexedItems.TryGetValue(qualifiedName.Name, out typeInfo);
    if (result == false)
      Debug.Assert(true);
    return result;
  }

  /// <summary>
  /// Tries to get the element from the collection indexed by full or short name.
  /// First the full name index is searched, and if not found then the short name index.
  /// </summary>
  /// <param name="name">Full or short name of the searched element.</param>
  /// <param name="typeInfo">variable to hold the element id found.</param>
  /// <returns>
  ///   <see langword="true" /> if the element is found; otherwise, <see langword="false" />.
  /// </returns>
  public bool TryGetValue(string name, [NotNullWhen(true)][MaybeNullWhen(false)] out TypeNameInfo typeInfo)
  {
    if (FullNameIndexedItems.TryGetValue(new QualifiedName(name), out typeInfo))
      return true;
    if (ShortNameIndexedItems.TryGetValue(name, out typeInfo))
      return true;
    return false;
  }

  /// <summary>
  /// Finds the type information by querying the collection.
  /// </summary>
  /// <param name="itemType">Type of the item.</param>
  /// <returns></returns>
  public TypeNameInfo? FindTypeInfo(Type itemType)
  {
    var result = this.FirstOrDefault(item => itemType == item.Type);
    if (result == null)
      result = this.FirstOrDefault(item => itemType.IsSubclassOf(item.Type));
    if (result == null || result.Type == typeof(Object))
      return null;
    return result;
  }

  /// <summary>
  ///  Type keys stored in collection
  /// </summary>
  /// <value>
  /// The keys.
  /// </value>
  public IEnumerable<Type> Keys => TypeIndexedItems.Keys;

}
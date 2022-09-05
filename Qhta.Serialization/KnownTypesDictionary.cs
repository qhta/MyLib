using System.Xml.Linq;

namespace Qhta.Xml.Serialization;

public class KnownTypesDictionary: ICollection<SerializationTypeInfo>
{
  [XmlAttribute]
  public string? BaseNamespace { get; set; }

  private Dictionary<Type, SerializationTypeInfo> TypeIndexedItems { get; set; } = new();

  public SortedDictionary<QualifiedName, SerializationTypeInfo> NameIndexedItems { get; set; } = new();

  private Dictionary<string, string> PrefixToNamespace { get; set; } = new();

  private Dictionary<string, string> NamespaceToPrefix { get; set; } = new();

  public void Add(SerializationTypeInfo item)
  {
    TypeIndexedItems.Add(item.Type, item);
    NameIndexedItems.Add(item.Name, item);
  }

  public void Clear()
  {
    TypeIndexedItems.Clear();
    NameIndexedItems.Clear();
    PrefixToNamespace.Clear();
    NamespaceToPrefix.Clear();
  }

  public bool Contains(SerializationTypeInfo item)
    => TypeIndexedItems.Values.Contains(item);

  public void CopyTo(SerializationTypeInfo[] array, int arrayIndex)
    => TypeIndexedItems.Values.CopyTo(array, arrayIndex); 

  public bool Remove(SerializationTypeInfo item)
  {
    var ok1 = TypeIndexedItems.Remove(item.Type);
    var ok2 = NameIndexedItems.Remove(item.Name);
    return ok1 || ok2;
  }

  public int Count => TypeIndexedItems.Count;

  public bool IsReadOnly => false;

  public bool TryGetValue(Type type, [MaybeNullWhen(false)] out SerializationTypeInfo typeInfo)
    => TypeIndexedItems.TryGetValue(type, out typeInfo);

  public bool TryGetValue(QualifiedName qualifiedName, [MaybeNullWhen(false)] out SerializationTypeInfo typeInfo)
    => NameIndexedItems.TryGetValue(qualifiedName, out typeInfo);

  public bool TryGetValue(string name, [MaybeNullWhen(false)] out SerializationTypeInfo typeInfo)
  {
    if (NameIndexedItems.TryGetValue(new QualifiedName(name), out typeInfo))
      return true;
    if (BaseNamespace!=null)
      if (NameIndexedItems.TryGetValue(new QualifiedName(name, BaseNamespace), out typeInfo))
        return true;
    return false;
  }

  public bool TryGetValue(XmlQualifiedName xmlQualifiedName, [MaybeNullWhen(false)] out SerializationTypeInfo typeInfo)
  {
    if (PrefixToNamespace.TryGetValue(xmlQualifiedName.Namespace, out var clrNamespace))
      if (NameIndexedItems.TryGetValue(new QualifiedName(xmlQualifiedName.Name, clrNamespace), out typeInfo))
        return true;
    typeInfo = null;
    return false;
  }

  public XmlQualifiedName GetXmlQualifiedName(QualifiedName qualifiedName)
  {
    if (NamespaceToPrefix.TryGetValue(qualifiedName.Namespace, out var xmlNamespace))
      return new XmlQualifiedName(qualifiedName.Name, xmlNamespace);
    return new XmlQualifiedName(qualifiedName.Name);
  }

  public IEnumerator<SerializationTypeInfo> GetEnumerator()
  {
    foreach (var item in TypeIndexedItems.Values)
      yield return item;
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
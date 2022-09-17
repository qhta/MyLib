using System.Collections.Generic;
using System.Linq;

namespace Qhta.Xml.Serialization;

public class KnownNamespacesCollection : ICollection<XmlNamespaceInfo>
{
  private List<XmlNamespaceInfo> Items { get; set; } = new();

  //private XmlNameTable? NameTable => NamespaceManager.NameTable;

  //private XmlNamespaceManager NamespaceManager { get; set; } = new XmlNamespaceManager(new System.Xml.NameTable());

  internal Dictionary<string, string> ClrToXmlNamespace { get; set; } = new();

  internal Dictionary<string, string> XmlNamespaceToPrefix { get; set; } = new();
  internal Dictionary<string, string> PrefixToXmlNamespace { get; set; } = new();

  public bool TryAdd(string xmlNamespace, string clrNamespace)
  {
    if (xmlNamespace == "")
      return false;
    var hasXmlNamespace = Items.Any(item => item.XmlNamespace == xmlNamespace);
    var hasClrNamespace = Items.Any(item => item.ClrNamespace == clrNamespace);
    if (hasXmlNamespace && hasClrNamespace)
      return false;
    if (hasClrNamespace)
      throw new InvalidOperationException($"ClrNamespace {clrNamespace} is already assigned to XmlNamespace {xmlNamespace}");
    return true;
  }

  public void Add(XmlNamespaceInfo item)
  {
    Items.Add(item);
    if (!string.IsNullOrEmpty(item.Prefix))
    {
      XmlNamespaceToPrefix.Add(item.XmlNamespace, item.Prefix);
      PrefixToXmlNamespace.Add(item.Prefix, item.XmlNamespace);
    }
  }

  public void Add(XmlNamespaceInfo item, string prefix)
  {
    Items.Add(item);
    XmlNamespaceToPrefix.Add(item.XmlNamespace, prefix);
    PrefixToXmlNamespace.Add(prefix, item.XmlNamespace);
  }

  public void AssignPrefixes(string? defaultNamespace)
  {
    XmlNamespaceToPrefix.Clear();
    PrefixToXmlNamespace.Clear();
    int i = 0;
    foreach (var item in Items)
    {
      if (item.Prefix == null)
      {
        if (item.XmlNamespace == defaultNamespace)
          item.Prefix = "";
        else
          item.Prefix = $"n{++i}";
      }

      XmlNamespaceToPrefix.Add(item.XmlNamespace, item.Prefix);
      PrefixToXmlNamespace.Add(item.Prefix, item.XmlNamespace);
    }

    Dump();
  }

  public void AssignPrefix(XmlNamespaceInfo item, string prefix)
  {
    XmlNamespaceToPrefix.Add(item.XmlNamespace, prefix);
    PrefixToXmlNamespace.Add(prefix, item.XmlNamespace);
  }

  public void Clear()
  {
    Items.Clear();
    XmlNamespaceToPrefix.Clear();
    PrefixToXmlNamespace.Clear();
  }

  public bool Contains(XmlNamespaceInfo item)
    => Items.Contains(item);

  public bool ContainsXmlNamespace(string xmlNamespace)
    => Items.Any(item => item.XmlNamespace == xmlNamespace);

  public bool ContainsNamespace(string xmlNamespace)
    => XmlNamespaceToPrefix.TryGetValue(xmlNamespace, out _);

  public bool ContainsPrefix(string prefix)
    => PrefixToXmlNamespace.TryGetValue(prefix, out _);


  public void CopyTo(XmlNamespaceInfo[] array, int arrayIndex)
    => Items.CopyTo(array, arrayIndex);

  public bool Remove(XmlNamespaceInfo item)
  {
    var ok0 = Items.Remove(item);
    var ok1 = XmlNamespaceToPrefix.Remove(item.XmlNamespace);
    var ok2 = PrefixToXmlNamespace.Remove(item.Prefix ?? "");
    return ok0 || ok1 || ok2;
  }

  public int Count => Items.Count;

  public bool IsReadOnly => false;

  //public bool TryGetValue(QualifiedName qualifiedName, [MaybeNullWhen(false)] out XmlNamespaceInfo typeInfo)
  //  => PrefixToNamespace.TryGetValue(qualifiedName, out typeInfo);

  //public bool TryGetValue(string name, [MaybeNullWhen(false)] out XmlNamespaceInfo typeInfo)
  //{
  //  if (PrefixToNamespace.TryGetValue(new QualifiedName(name), out typeInfo))
  //    return true;
  //  //if (BaseNamespace != null)
  //  //  if (NameIndexedItems.TryGetValue(new QualifiedName(name, BaseNamespace), out typeInfo))
  //  //    return true;
  //  return false;
  //}
  public IEnumerator<XmlNamespaceInfo> GetEnumerator()
  {
    foreach (var item in Items)
      yield return item;
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  public void Dump()
  {
    Debug.WriteLine("KnownNamespaces:");
    foreach (var nspace in this)
      Debug.WriteLine($" xmlns:{nspace.Prefix} = \"{nspace.XmlNamespace}\"");
  }
}
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qhta.Xml.Serialization;

public class KnownNamespacesCollection : ICollection<XmlNamespaceInfo>
{
  private List<XmlNamespaceInfo> Items { get; set; } = new();

  //public KnownNamespacesCollection(){}

  //public KnownNamespacesCollection(XmlSerializerNamespaces namespaces)
  //{
  //  foreach (var ns in namespaces.ToArray())
  //    Items.Add(new XmlNamespaceInfo(ns));
  //}


  internal Dictionary<string, string> ClrToXmlNamespace { get; set; } = new();

  internal Dictionary<string, string> XmlNamespaceToPrefix { get; set; } = new();
  internal Dictionary<string, string> PrefixToXmlNamespace { get; set; } = new();

  public bool TryAdd(string clrNamespace)
  {
    if (!ClrToXmlNamespace.TryGetValue(clrNamespace, out var xmlNamespace))
      xmlNamespace = clrNamespace;
    return TryAdd(xmlNamespace, clrNamespace);
  }

  public bool TryAdd(string xmlNamespace, string clrNamespace)
  {
    if (xmlNamespace == "")
      return false;
    var hasXmlNamespace = Items.Any(item => item.XmlNamespace == xmlNamespace);
    var hasClrNamespace = Items.Any(item => item.ClrNamespace == clrNamespace);
    if (hasXmlNamespace && hasClrNamespace)
      return false;
    if (hasClrNamespace)
      return false;
    //throw new InvalidOperationException($"ClrNamespace {clrNamespace} is already assigned to XmlNamespace {xmlNamespace}");
    var xmlNamespaceInfo = new XmlNamespaceInfo { ClrNamespace = clrNamespace, XmlNamespace = xmlNamespace };
    Add(xmlNamespaceInfo);
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

  public void AssignPrefixes(string? defaultNamespace, XmlSerializerNamespaces? namespaces = null)
  {
    XmlNamespaceToPrefix.Clear();
    PrefixToXmlNamespace.Clear();
    foreach (var item in Items)
    {
      if (item.Prefix == null)
      {
        if (item.XmlNamespace == defaultNamespace)
          item.Prefix = "";
        else
        {
          string? prefix = null;
          if (namespaces != null)
            prefix = namespaces.ToArray().FirstOrDefault(item1 => item1.Namespace == item.ClrNamespace)?.Name;
          if (prefix == null)
            prefix = (item.ClrNamespace!=null) ? NamespaceToPrefix(item.ClrNamespace) : "n";
          int i = 2;
          var pfx = prefix;
          while (PrefixToXmlNamespace.ContainsKey(prefix))
          {
            prefix = pfx + (i++).ToString();
          }
          item.Prefix = prefix;
        }
      }
      XmlNamespaceToPrefix.Add(item.XmlNamespace, item.Prefix);
      PrefixToXmlNamespace.Add(item.Prefix, item.XmlNamespace);
    }

    //Dump();
  }

  public void AssignPrefix(XmlNamespaceInfo item, string prefix)
  {
    XmlNamespaceToPrefix.Add(item.XmlNamespace, prefix);
    PrefixToXmlNamespace.Add(prefix, item.XmlNamespace);
  }

  private string NamespaceToPrefix(string nspace)
  {
    if (nspace == "System")
      return "sys";
    var sb = new StringBuilder();
    foreach (var ch in nspace)
    {
      if (Char.IsUpper(ch))
        sb.Append(Char.ToLower(ch));
    }
    return sb.ToString();
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
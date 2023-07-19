using System.Text;

namespace Qhta.Xml.Reflection;

/// <summary>
/// Named collection of xml namespace info
/// </summary>
public class KnownNamespacesCollection : ICollection<XmlNamespaceInfo>
{

  /// <summary>
  /// String indexed items
  /// </summary>
  public Dictionary<string, XmlNamespaceInfo> Items = new();

  /// <summary>
  /// Conversion dictionary from C# language namespace to xml namespace
  /// </summary>
  public Dictionary<string, string> ClrToXmlNamespace { get; set; } = new();

  /// <summary>
  /// Conversion dictionary from xml namespace to xml prefix
  /// </summary>
  public Dictionary<string, string> XmlNamespaceToPrefix { get; set; } = new();

  /// <summary>
  /// Conversion dictionary from xml prefix to xml namespace
  /// </summary>
  public Dictionary<string, string> PrefixToXmlNamespace { get; set; } = new();

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public void Add(XmlNamespaceInfo item)
  {
    Items.Add(item.XmlNamespace, item);
    if (!string.IsNullOrEmpty(item.Prefix))
    {
      XmlNamespaceToPrefix.Add(item.XmlNamespace, item.Prefix ?? "");
      PrefixToXmlNamespace.Add(item.Prefix ?? "", item.XmlNamespace);
    }
  }

  public void Clear()
  {
    Items.Clear();
    XmlNamespaceToPrefix.Clear();
    PrefixToXmlNamespace.Clear();
  }

  public bool Contains(XmlNamespaceInfo item)
  {
    return Items.Values.Contains(item);
  }


  public void CopyTo(XmlNamespaceInfo[] array, int arrayIndex)
  {
    Items.Values.CopyTo(array, arrayIndex);
  }

  public bool Remove(XmlNamespaceInfo item)
  {
    var ok0 = Items.Remove(item.XmlNamespace);
    var ok1 = XmlNamespaceToPrefix.Remove(item.XmlNamespace);
    var ok2 = PrefixToXmlNamespace.Remove(item.Prefix ?? "");
    return ok0 || ok1 || ok2;
  }

  public int Count => Items.Count;

  public bool IsReadOnly => false;

  public IEnumerator<XmlNamespaceInfo> GetEnumerator()
  {
    foreach (var item in Items.Values)
      yield return item;
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

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
    if (xmlNamespace.Contains('_'))
      return false;
    var hasXmlNamespace = Items.ContainsKey(xmlNamespace);
    var hasClrNamespace = Items.Values.Any(item => item.ClrNamespace == clrNamespace);
    if (hasXmlNamespace && hasClrNamespace)
      return false;
    if (hasClrNamespace)
      return false;
    //throw new InvalidOperationException($"ClrNamespace {clrNamespace} is already assigned to XmlNamespace {xmlNamespace}");
    var xmlNamespaceInfo = new XmlNamespaceInfo { ClrNamespace = clrNamespace, XmlNamespace = xmlNamespace };
    Add(xmlNamespaceInfo);
    return true;
  }

  public void Add(XmlNamespaceInfo item, string prefix)
  {
    Items.Add(item.XmlNamespace, item);
    XmlNamespaceToPrefix.Add(item.XmlNamespace, prefix);
    PrefixToXmlNamespace.Add(prefix, item.XmlNamespace);
  }

  public bool ContainsXmlNamespace(string xmlNamespace)
  {
    return Items.ContainsKey(xmlNamespace);
  }

  public bool ContainsNamespace(string xmlNamespace)
  {
    return XmlNamespaceToPrefix.TryGetValue(xmlNamespace, out _);
  }

  public bool ContainsPrefix(string prefix)
  {
    return PrefixToXmlNamespace.TryGetValue(prefix, out _);
  }

  public XmlNamespaceInfo this[string xmlNamespace]
  {
    get => Items[xmlNamespace];
  }

  #region Prefixes setting methods
  public void AutoSetPrefixes(string? defaultNamespace = null)
  {
    foreach (var item in Items.Values)
    {
      if (item.Prefix == null)
      {
        var pfx = item.XmlNamespace == defaultNamespace ? "" : NamespaceToPrefix(item.XmlNamespace);
        var prefix = pfx;
        int n = 1;
        while (PrefixToXmlNamespace.ContainsKey(prefix))
          prefix = pfx + (n++).ToString();
        item.Prefix = prefix;
        XmlNamespaceToPrefix.Add(item.XmlNamespace, item.Prefix);
        PrefixToXmlNamespace.Add(item.Prefix, item.XmlNamespace);
      }
    }
  }

  public void AssignPrefixes(string? defaultNamespace, XmlSerializerNamespaces? namespaces = null)
  {
    XmlNamespaceToPrefix.Clear();
    PrefixToXmlNamespace.Clear();
    foreach (var item in Items.Values)
    {
      if (item.Prefix == null)
      {
        if (item.XmlNamespace == defaultNamespace)
        {
          item.Prefix = "";
        }
        else
        {
          string? prefix = null;
          if (namespaces != null)
            prefix = namespaces.ToArray().FirstOrDefault(item1 => item1.Namespace == item.ClrNamespace)?.Name;
          if (prefix == null)
            prefix = item.ClrNamespace != null ? NamespaceToPrefix(item.ClrNamespace) : "n";
          var i = 2;
          var pfx = prefix;
          while (PrefixToXmlNamespace.ContainsKey(prefix)) prefix = pfx + (i++);
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
      if (Char.IsUpper(ch))
        sb.Append(Char.ToLower(ch));
      else if (Char.IsDigit(ch))
        sb.Append(ch);
    return sb.ToString();
  }
  #endregion
  public void Dump()
  {
    Debug.WriteLine("KnownNamespaces:");
    foreach (var nspace in this)
      Debug.WriteLine($" xmlns:{nspace.Prefix} = \"{nspace.XmlNamespace}\"");
  }
}
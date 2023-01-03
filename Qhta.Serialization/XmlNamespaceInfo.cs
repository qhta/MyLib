namespace Qhta.Xml.Serialization;

public class XmlNamespaceInfo
{
  public XmlNamespaceInfo()
  {
  }

  public XmlNamespaceInfo(XmlQualifiedName qname)
  {
    XmlNamespace = qname.Namespace;
    Prefix = qname.Name;
  }

  public string XmlNamespace { get; set; } = string.Empty;

  public string? ClrNamespace { get; set; }

  public string? Prefix { get; set; }
}
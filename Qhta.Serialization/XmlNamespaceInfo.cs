namespace Qhta.Xml.Serialization;

public class XmlNamespaceInfo
{
  public string XmlNamespace {get; set;} = string.Empty;

  public string? ClrNamespace { get; set; }

  public string? Prefix { get; set; }
}
namespace Qhta.Xml.Reflection;

/// <summary>
/// Compounf information for xml namespace. 
/// Contains the xml namespace (uri), C# namespace, xml prefix.
/// </summary>
public class XmlNamespaceInfo
{
  /// <summary>
  /// Initializes a new instance of the <see cref="XmlNamespaceInfo"/> class.
  /// </summary>
  public XmlNamespaceInfo()
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="XmlNamespaceInfo"/> class
  /// using xml qualified name
  /// </summary>
  /// <param name="qname">Xml qualified name.</param>
  public XmlNamespaceInfo(XmlQualifiedName qname)
  {
    XmlNamespace = qname.Namespace;
    Prefix = qname.Name;
  }

  /// <summary>
  /// Gets or sets the XML namespace (uri)
  /// </summary>
  /// <value>
  /// The XML namespace.
  /// </value>
  public string XmlNamespace { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the C# namespace.
  /// </summary>
  /// <value>
  /// The C# namespace.
  /// </value>
  public string? ClrNamespace { get; set; }

  /// <summary>
  /// Gets or sets the XML prefix.
  /// </summary>
  /// <value>
  /// The prefix.
  /// </value>
  public string? Prefix { get; set; }

  /// <summary>
  /// Gets or sets a flag indicating whether this instance is used.
  /// </summary>
  /// <value>
  ///   <c>true</c> if this instance is used; otherwise, <c>false</c>.
  /// </value>
  public bool IsUsed { get; set; }

}
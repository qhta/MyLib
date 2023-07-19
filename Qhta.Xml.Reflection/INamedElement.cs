namespace Qhta.Xml.Reflection;

/// <summary>
/// Interface for an element that provides XML name, XML namespace, and ClrNamespace
/// </summary>
public interface INamedElement
{
  /// <summary>
  /// Gets the name of the XML element.
  /// </summary>
  string XmlName { get; }

  /// <summary>
  /// Gets the namespace of the XML element.
  /// </summary>
  string? XmlNamespace { get; }

  /// <summary>
  /// Gets the programming language namespace of the element.
  /// </summary>
  string? ClrNamespace { get; }

  /// <summary>
  /// Gets the the qualified name (XmlName, XmlNamespace) of the element
  /// </summary>
#if NET6_0_OR_GREATER
  public QualifiedName QualifiedName => new(XmlName, XmlNamespace);
#else
  public QualifiedName QualifiedName { get; }
#endif
}
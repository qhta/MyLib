namespace Qhta.Xml.Reflection;

public interface INamedElement
{
  string XmlName { get; }

  string? XmlNamespace { get; }

  string? ClrNamespace { get; }

  public QualifiedName QualifiedName => new(XmlName, XmlNamespace);
}
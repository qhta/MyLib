namespace Qhta.Xml.Serialization;

public interface INamedElement
{
  string XmlName { get; }

  string? XmlNamespace { get; }

  string? ClrNamespace { get; }

  public QualifiedName QualifiedName => new QualifiedName(XmlName, XmlNamespace);

}
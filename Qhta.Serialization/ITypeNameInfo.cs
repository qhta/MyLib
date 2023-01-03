namespace Qhta.Xml.Serialization;

public interface ITypeNameInfo : INamedElement
{
  //string? XmlName { get; }

  //string? XmlNamespace { get; }

  //string? ClrNamespace { get; }

  Type Type { get; }
}
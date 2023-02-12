namespace Qhta.Xml.Reflection;

public interface ITypeNameInfo : INamedElement
{
  //string? XmlName { get; }

  //string? XmlNamespace { get; }

  //string? ClrNamespace { get; }

  Type Type { get; }
}
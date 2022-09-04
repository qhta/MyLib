namespace Qhta.Xml.Serialization;

public interface ITypeInfo
{
  QualifiedName Name { get; }

  Type Type { get; }
}
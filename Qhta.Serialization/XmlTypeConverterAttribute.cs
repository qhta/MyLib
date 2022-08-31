namespace Qhta.Xml.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
public class XmlTypeConverterAttribute : Attribute
{
  public XmlTypeConverterAttribute(Type type)
  {
    ConverterType = type;
  }

  public Type ConverterType { get;}
}
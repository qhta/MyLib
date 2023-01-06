namespace Qhta.Xml.Serialization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
public class XmlConverterAttribute : Attribute
{
  public XmlConverterAttribute(Type converterType, params object[] args)
  {
    ConverterType = converterType;
    Args = args;
  }

  public Type ConverterType { get; }

  public object[] Args { get; }
}
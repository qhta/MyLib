using System;

namespace Qhta.Xml.Serialization
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
  public class XmlConverterAttribute : Attribute
  {
    public XmlConverterAttribute(Type type)
    {
      ConverterType = type;
    }

    public Type ConverterType { get; init; }
  }
}

using System;

namespace Qhta.Xml.Serialization
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class XmlTypeConverterAttribute : Attribute
  {
    public XmlTypeConverterAttribute(Type type)
    {
      ConverterType = type;
    }

    public Type ConverterType { get; init; }
  }
}

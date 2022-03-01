using System;

namespace Qhta.Xml.Serialization
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
  public class XmlConverterAttribute : Attribute
  {
    public XmlConverterAttribute(Type converterType, params object[] args)
    {
      ConverterType = converterType;
      Args = args;
    }

    public Type ConverterType { get; init; }

    public object[] Args { get; init; }

  }
}

using System;
#nullable enable

namespace Qhta.Serialization
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class XmlDictionaryAttribute : System.Xml.Serialization.XmlArrayAttribute
  {
    public XmlDictionaryAttribute(string? elementName) : base(elementName) { }

    public XmlDictionaryAttribute(string? elementName, Type keyType, Type? valueType = null) : base(elementName) 
    { 
      KeyType = keyType;
      ValueType = valueType;  
    }

    public Type? KeyType { get; init; }

    public Type? ValueType { get; init; }

  }
}

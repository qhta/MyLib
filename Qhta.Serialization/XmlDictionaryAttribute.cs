using System;
#nullable enable

namespace Qhta.Serialization
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class XmlDictionaryAttribute : System.Xml.Serialization.XmlArrayAttribute
  {

    public XmlDictionaryAttribute(string? elementName) : base(elementName) { }

    public Type? KeyType { get; init; }

    public string? KeyName { get; init; }

    public Type? ValueType { get; init; }

  }
}

using System;
#nullable enable

namespace Qhta.Xml.Serialization
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
  public class XmlDictionaryAttribute : XmlCollectionAttribute
  {

    public XmlDictionaryAttribute(string? elementName) : base(elementName) { }

    public Type? KeyType { get; init; }

    public string? KeyName { get; init; }

    public Type? ValueType { get; init; }

    public bool AttributesAreKeys { get; set; }

    public bool ElementsAreKeys { get; set; }
  }
}

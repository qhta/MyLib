using System;
#nullable enable

namespace Qhta.Xml.Serialization
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
  public class XmlDictionaryAttribute : XmlCollectionAttribute
  {

    public XmlDictionaryAttribute(string? elementName) : base(elementName) { }

    public Type? KeyType { get;}

    public string? KeyName { get;}

    public Type? ValueType { get;}

    public bool AttributesAreKeys { get; set; }

    public bool ElementsAreKeys { get; set; }
  }
}

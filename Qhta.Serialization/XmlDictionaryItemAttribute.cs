using System;
#nullable enable

namespace Qhta.Serialization
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class XmlDictionaryItemAttribute : System.Xml.Serialization.XmlArrayItemAttribute
  {
    public XmlDictionaryItemAttribute(string elementName) : base(elementName)
    {
    }

    public XmlDictionaryItemAttribute(string elementName, string keyAttribute, Type? type = null) : base(elementName, type) 
    { 
      KeyAttribute = keyAttribute;
    }

    public string? KeyAttribute { get; init; }

  }
}

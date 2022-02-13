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

    public XmlDictionaryItemAttribute(string elementName, string keyName, Type? type = null) : base(elementName, type) 
    { 
      KeyName = keyName;
    }

    public string? KeyName { get; init; }

  }
}

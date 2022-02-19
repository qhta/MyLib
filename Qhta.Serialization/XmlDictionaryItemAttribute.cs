using System;
#nullable enable

namespace Qhta.Serialization
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
  public class XmlDictionaryItemAttribute : XmlItemElementAttribute
  {
    public XmlDictionaryItemAttribute(string elementName) : base(elementName)
    {
    }

    public XmlDictionaryItemAttribute(string elementName, string keyName, Type? itemType = null) : base(elementName, itemType) 
    { 
      KeyName = keyName;
    }

    public string? KeyName { get; init; }

  }
}

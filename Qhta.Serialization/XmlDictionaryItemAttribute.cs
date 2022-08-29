using System;
#nullable enable

namespace Qhta.Xml.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
public class XmlDictionaryItemAttribute : XmlItemElementAttribute
{
  public XmlDictionaryItemAttribute(string elementName) : base(elementName)
  {
  }

  public XmlDictionaryItemAttribute(string elementName, string keyName, Type? itemType = null) : base(elementName, itemType) 
  { 
    KeyAttributeName = keyName;
  }

  public XmlDictionaryItemAttribute(string elementName, Type? itemType, string? keyName = null ) : base(elementName, itemType)
  {
    KeyAttributeName = keyName;
  }
  public string? KeyAttributeName { get;}

  public string? ValueAttributeName { get;}

}
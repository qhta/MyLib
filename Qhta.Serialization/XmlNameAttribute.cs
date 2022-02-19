using System;
#nullable enable

namespace Qhta.Xml.Serialization
{
  public enum XmlAttributeNameType
  {
    Element,
    Property,
    Method,
  }

  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class XmlNameAttribute: System.Attribute
  {
    public XmlNameAttribute() { }

    public XmlNameAttribute(string? elementName)
    {
      ElementName = elementName;
    }

    private string? Name {get; set; }

    public XmlAttributeNameType NameType { get; set; }

    public string? ElementName 
    { 
      get => (NameType == XmlAttributeNameType.Element) ? Name : null;
      set { Name = value; NameType = XmlAttributeNameType.Element; }
    }

    public string? PropertyName
    {
      get => (NameType == XmlAttributeNameType.Property) ? Name : null;
      set { Name = value; NameType = XmlAttributeNameType.Property; }
    }

    public string? MethodName
    {
      get => (NameType == XmlAttributeNameType.Method) ? Name : null;
      set { Name = value; NameType = XmlAttributeNameType.Method; }
    }

  }
}

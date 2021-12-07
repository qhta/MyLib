using System;

namespace Qhta.Serialization
{
  public class XmlOrderedAttribAttribute : System.Xml.Serialization.XmlAttributeAttribute
  {
    public XmlOrderedAttribAttribute() : base() { }

    public XmlOrderedAttribAttribute(string attributeName) : base(attributeName) { }

    public XmlOrderedAttribAttribute(string attributeName, Type type) : base(attributeName, type) { }

    public XmlOrderedAttribAttribute(Type type) { }

    public int Order { get; set; }
  }
}

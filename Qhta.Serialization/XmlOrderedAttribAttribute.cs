using System;

namespace Qhta.Xml.Serialization
{
  public class XmlOrderedAttribAttribute : System.Xml.Serialization.XmlAttributeAttribute
  {
    public XmlOrderedAttribAttribute() : base() { }

    public XmlOrderedAttribAttribute(string attributeName) : base(attributeName) { }

    public XmlOrderedAttribAttribute(string attributeName, Type type) : base(attributeName, type) { }

    public XmlOrderedAttribAttribute(Type type) { }

    public XmlOrderedAttribAttribute(int order) : base() { Order = order; }

    public XmlOrderedAttribAttribute(string attributeName, int order) : base(attributeName) { Order = order; }

    public XmlOrderedAttribAttribute(string attributeName, Type type, int order) : base(attributeName, type) { Order = order; }

    public XmlOrderedAttribAttribute(Type type, int order) { Order = order; }

    public int? Order { get; set; }
  }
}

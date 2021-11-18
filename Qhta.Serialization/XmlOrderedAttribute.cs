using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Serialization
{
  public class XmlOrderedAttribute: System.Xml.Serialization.XmlAttributeAttribute
  {
    public XmlOrderedAttribute(): base() { }

    public XmlOrderedAttribute(string attributeName): base(attributeName) { }

    public XmlOrderedAttribute(string attributeName, Type type): base (attributeName, type) { }

    public XmlOrderedAttribute(Type type) { }

    public int Order { get; set; }
  }
}

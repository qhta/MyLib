using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Serialization
{
  public class XmlAttribute2Attribute: System.Xml.Serialization.XmlAttributeAttribute
  {
    public XmlAttribute2Attribute(): base() { }

    public XmlAttribute2Attribute(string attributeName): base(attributeName) { }

    public XmlAttribute2Attribute(string attributeName, Type type): base (attributeName, type) { }

    public XmlAttribute2Attribute(Type type) { }

    public int Order { get; set; }
  }
}

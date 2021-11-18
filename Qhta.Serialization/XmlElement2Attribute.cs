using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Xml.Serialization
{
  public class XmlElementAttribute: System.Xml.Serialization.XmlElementAttribute
  {
    public XmlElementAttribute(): base() { }

    public XmlElementAttribute(string attributeName): base(attributeName) { }

    public XmlElementAttribute(string attributeName, Type type): base (attributeName, type) { }

    public XmlElementAttribute(Type type) { }

    public new int Order { get; set; }
  }
}

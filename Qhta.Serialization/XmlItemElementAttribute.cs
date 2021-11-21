using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable enable

namespace Qhta.Serialization
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class XmlItemElementAttribute: System.Xml.Serialization.XmlArrayItemAttribute
  {
    public XmlItemElementAttribute(): base(){}

    public XmlItemElementAttribute(string? elementName): base(elementName)  { }

    public XmlItemElementAttribute(string? elementName, Type? type): base(elementName, type) { }

    public XmlItemElementAttribute(Type? type): base(type) 
    { 
//      NoItemElement = true;
    }

//    public bool NoItemElement { get; set; }
  }
}

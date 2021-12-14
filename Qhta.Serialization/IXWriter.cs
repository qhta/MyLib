using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Serialization
{
  public interface IXWriter
  {
    void WriteStartElement(string tag);

    void WriteEndElement(string tag);

    void WriteAttributeString(string attrName, string str);

    void WriteValue(string tagName, string str);

    void WriteValue(string str);
  }
}

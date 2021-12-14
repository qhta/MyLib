using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Qhta.Serialization
{
  public class QXmlWriter: IXWriter, IDisposable
  {
    private XmlWriter _writer;

    public QXmlWriter (TextWriter textWriter)
    {
      _writer = XmlWriter.Create(textWriter, new XmlWriterSettings { Indent = true });
    }

    public void Dispose()
    {
      ((IDisposable)_writer).Dispose();
    }

    public void WriteStartElement(string tag) => _writer.WriteStartElement(tag);
    public void WriteEndElement(string tag) =>  _writer.WriteEndElement();
    public void WriteAttributeString(string attrName, string str) => _writer.WriteAttributeString(attrName, str);
    public void WriteValue(string str) => _writer.WriteValue(str);

    public void WriteValue(string tagName, string str)
    { 
      _writer.WriteStartElement(tagName);
      _writer.WriteValue(str);
      _writer.WriteEndElement();
    }

  }
}

using System;
using System.IO;
using System.Xml;

namespace Qhta.Xml.Serialization;

public class QXmlWriter: IXWriter, IDisposable
{
  private XmlWriter _writer { get; set; }
  private XmlSpace _spaceBehavior { get; set; }

  public QXmlWriter(XmlWriter xmlWriter)
  {
    _writer = xmlWriter;
    _spaceBehavior = XmlSpace.Preserve;
  }

  public QXmlWriter (TextWriter textWriter)
  {
    _writer = XmlWriter.Create(textWriter, new XmlWriterSettings { Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates });
    _spaceBehavior = XmlSpace.Preserve;
  }

  public void Dispose()
  {
    ((IDisposable)_writer).Dispose();
  }

  public void WriteStartElement(string tag) 
  {
    _writer.WriteStartElement(tag);
  }

  public void WriteEndElement(string tag) =>  _writer.WriteEndElement();

  public void WriteAttributeString(string? prefix, string attrName, string? ns, string? str) 
    => _writer.WriteAttributeString(prefix, attrName, ns, str);

  public void WriteAttributeString(string attrName, string? ns, string? str)
    => _writer.WriteAttributeString(attrName, ns, str);

  public void WriteAttributeString(string attrName, string? str)
    => _writer.WriteAttributeString(attrName, str);

  public void WriteValue(string str)
  {
    if (_spaceBehavior == XmlSpace.Preserve)
    {
      if (str.StartsWith(" ") || str.EndsWith(" ") || str.Contains('\n') || str.Contains('\r') || str.Contains('\t'))
        WriteSignificantSpaces(true);
    }
    _writer.WriteValue(str);
  }

  public void WriteElementString(string tagName, string str)
  { 
    _writer.WriteStartElement(tagName);
    _writer.WriteValue(str);
    _writer.WriteEndElement();
  }

  public void WriteSignificantSpaces(bool value)
  {
    _writer.WriteAttributeString("xml", "space", null, (value) ? "preserve" : "default");
  }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Qhta.TestHelper
{

  public class TraceXmlWriter : TraceWriter, ITraceXmlWriter
  {
    protected new XmlWriter _writer { get; private set; }

    protected Stack<string> _stack { get; private set; }


    private XmlSpace _spaceBehavior { get; set; } = XmlSpace.None;

    public TraceXmlWriter(): base()
    {
      _writer = XmlWriter.Create(base._writer, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
      _stack = new Stack<string>();
    }

    public TraceXmlWriter(string filename): base(filename)
    {
      _writer = XmlWriter.Create(base._writer, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
      _stack = new Stack<string>();
    }

    public override void Flush()
    {
      if (!Enabled) return;
      _writer.Flush();
      base.Flush();
    }

    [DebuggerStepThrough]
    public void WriteStartElement(string tag)
    {
      _writer.WriteStartElement(tag);
      _stack.Push(tag);
      if (AutoFlush)
        Flush();
    }

    [DebuggerStepThrough]
    public void WriteEndElement(string tag)
    {
      if (_stack.TryPeek(out var _tag) && tag == _tag)
      {
        _stack.Pop();
        _writer.WriteEndElement();
        if (AutoFlush)
          Flush();
      }
      else
        throw new InternalException($"TraceXmlWriter tried to write end of \"{tag}\" element but \"{_tag}\" expected");
    }

    public void WriteAttributeString(string? prefix, string attrName, string? ns, string? str)
      => _writer.WriteAttributeString(prefix, attrName, ns, str);

    public void WriteValue(string? str)
    {
      if (str == null) return;

      if (_spaceBehavior == XmlSpace.Preserve)
      {
        if (str.StartsWith(' ') || str.EndsWith(' ') || str.Contains('\n') || str.Contains('\r') || str.Contains('\t'))
          WriteSignificantSpaces(true);
      }
      _writer.WriteValue(str);
    }

    public void WriteElementString(string tagName, string? str)
    {
      if (str==null) return;

      _writer.WriteStartElement(tagName);
      _writer.WriteValue(str);
      _writer.WriteEndElement();
    }

    public void WriteSignificantSpaces(bool value)
    {
      _writer.WriteAttributeString("xml", "space", null, (value) ? "preserve" : "default");
    }
  }
}

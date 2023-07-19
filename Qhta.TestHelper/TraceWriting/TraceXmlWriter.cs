using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Qhta.TestHelper;

/// <summary>
/// Main implementation of <see cref="ITraceXmlWriter"/>
/// </summary>
/// <seealso cref="Qhta.TestHelper.TraceTextWriter" />
/// <seealso cref="Qhta.TestHelper.ITraceXmlWriter" />
public class TraceXmlWriter : TraceTextWriter, ITraceXmlWriter
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

  protected new XmlWriter _writer { get; private set; }

  protected Stack<string> _stack { get; private set; } = new Stack<string>();

  private XmlSpace _spaceBehavior { get; set; } = XmlSpace.None;

  private List<string> _suppressedPrefixes = new List<string>();


  /// <summary>
  /// Creates monitor for a file, console or trace output stream is created on flush.
  /// </summary>
  /// <param name="filename">name of output file</param>
  /// <param name="consoleOutputEnabled">if should output to console</param>
  /// <param name="traceOutputEnabled">if should output to trace</param>
  public TraceXmlWriter(string? filename, bool consoleOutputEnabled = false, bool traceOutputEnabled = false) :
    base(filename, consoleOutputEnabled, traceOutputEnabled)
  {
    if (OutputStream == null)
      throw new InvalidOperationException($"Output stream not created for \"{filename}\"");
    _writer = XmlWriter.Create(OutputStream, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
    _stack = new Stack<string>();
  }

  public override void Flush()
  {
    if (!Enabled) return;
    base.Flush();
  }

  protected override void FlushBuffer()
  {
    base.FlushBuffer();
    _writer?.Flush();
  }

  public void WriteNamespacePrefix(string prefix, string ns)
  {
    _writer.WriteAttributeString("xmlns", prefix, null, ns);
  }

  public void SuppressPrefix(string prefix, bool value = true)
  {
    if (value)
      _suppressedPrefixes.Add(prefix);
    else
      _suppressedPrefixes.Remove(prefix);
  }

  [DebuggerStepThrough]
  public void WriteStartElement(string tag) => WriteStartElement(null, tag, null);

  [DebuggerStepThrough]
  public void WriteStartElement(string? prefix, string localName) => WriteStartElement(prefix, localName, null);

  [DebuggerStepThrough]
  public void WriteStartElement(string? prefix, string localName, string? ns)
  {
    if (prefix != null && _suppressedPrefixes.Contains(prefix)) return;

    _writer.WriteStartElement(prefix, localName, ns);
    var xmlName = localName;
    if (prefix != null)
      xmlName = prefix + ":" + localName;
    _stack.Push(xmlName);
    if (AutoFlush)
      Flush();
  }

  [DebuggerStepThrough]
  public void WriteEndElement(string tag) => WriteEndElement(null, tag, null);

  [DebuggerStepThrough]
  public void WriteEndElement(string? prefix, string localName) => WriteEndElement(prefix, localName, null);

  [DebuggerStepThrough]
  public void WriteEndElement(string? prefix, string localName, string? ns)
  {
    if (prefix != null && _suppressedPrefixes.Contains(prefix)) return;

    var xmlName = localName;
    if (prefix != null)
      xmlName = prefix + ":" + localName;
    if (_stack.Count > 0)
    {
      var _storedName = _stack.Peek();
      if (xmlName == _storedName)
      {
        _stack.Pop();
        _writer.WriteEndElement();
        if (AutoFlush)
          Flush();
        return;
      }
      throw new InvalidOperationException($"TraceXmlWriter tried to write end of \"{xmlName}\" element but \"{_storedName}\" expected");
    }
      throw new InvalidOperationException($"TraceXmlWriter tried to write end of \"{xmlName}\" element but element stack was empty");
  }

  public void WriteAttributeString(string attrName, string? value) => WriteAttributeString(null, attrName, null, value);

  public void WriteAttributeString(string? prefix, string localName, string? value) => WriteAttributeString(prefix, localName, null, value);

  public void WriteAttributeString(string? prefix, string attrName, string? ns, string? str)
  {
    if (prefix != null && _suppressedPrefixes.Contains(prefix)) return;

    if (str == null) return;
    _writer.WriteAttributeString(prefix, attrName, ns, str);
  }

  public void WriteValue(string? str) => WriteValue(null, str);

  public void WriteValue(string? prefix, string? str)
  {
    if (prefix != null && _suppressedPrefixes.Contains(prefix)) return;
    if (str == null) return;

    if (_spaceBehavior == XmlSpace.Preserve)
    {
      if (str.StartsWith(" ") || str.EndsWith(" ") || str.Contains('\n') || str.Contains('\r') || str.Contains('\t'))
        WriteSignificantSpaces(true);
    }
    _writer.WriteValue(str);
  }

  public void WriteElementString(string tagName, string? value) => WriteElementString(null, tagName, null, value);

  public void WriteElementString(string? prefix, string localName, string? value) => WriteElementString(prefix, localName, null, value);

  public void WriteElementString(string? prefix, string localName, string? ns, string? value)
  {
    if (prefix != null && _suppressedPrefixes.Contains(prefix)) return;

    if (value == null) return;

    _writer.WriteStartElement(prefix, localName, ns);
    _writer.WriteValue(value);
    _writer.WriteEndElement();
  }

  public void WriteEmptyElement(string tagName) => WriteEmptyElement(null, tagName, null);

  public void WriteEmptyElement(string? prefix, string localName) => WriteEmptyElement(prefix, localName, null);

  public void WriteEmptyElement(string? prefix, string localName, string? ns)
  {
    if (prefix != null && _suppressedPrefixes.Contains(prefix)) return;

    _writer.WriteStartElement(prefix, localName, ns);
    _writer.WriteEndElement();
  }

  public void WriteSignificantSpaces(bool value)
  {
    _writer.WriteAttributeString("xml", "space", null, (value) ? "preserve" : "default");
  }

  public void WriteComment(string? text) => WriteComment(null, text);

  public void WriteComment(string? prefix, string? str)
  {
    if (prefix != null && _suppressedPrefixes.Contains(prefix)) return;

    if (str == null) return;
    _writer.WriteComment(str);
  }

  public void WriteStartComment(string? prefix)
  {
    if (prefix != null && _suppressedPrefixes.Contains(prefix)) return;

    _writer.WriteRaw("<!--");
  }

  public void WriteEndComment(string? prefix)
  {
    if (prefix != null && _suppressedPrefixes.Contains(prefix)) return;

    _writer.WriteRaw("-->");
  }
}
using System.Reflection;
using System.Xml.XPath;

namespace Qhta.Xml.Serialization;

public partial class QXmlWriter : IXmlWriter, IDisposable
{
  public QXmlWriter(XmlWriter xmlWriter)
  {
    _writer = xmlWriter;
    _spaceBehavior = XmlSpace.Preserve;
  }

  private XmlWriter _writer { get; }

  public static implicit operator XmlWriter(QXmlWriter writer) => writer._writer;

  private XmlSpace _spaceBehavior { get; } 

  public bool XsiNamespaceUsed { get; private set; }

  public bool XsdNamespaceUsed { get; private set; }

  public SortedSet<string> NamespacesUsed { get; } = new();

  public Stack<XmlQualifiedTagName> ElementStack { get; private set; } = new();

  public Stack<XmlQualifiedTagName> AttributeStack { get; private set; } = new();

  public bool EmitNamespaces { get; set; } = true;

  public XmlWriterSettings? Settings => _writer.Settings;

  public void WriteStartDocument()
  {
    _writer.WriteStartDocument();
  }

  public void WriteStartDocument(bool standalone)
  {
    _writer.WriteStartDocument(standalone);
  }

  public void WriteEndDocument()
  {
    _writer.WriteEndDocument();
  }

  public void WriteDocType(string name, string? pubid, string? sysid, string? subset)
  {
    _writer.WriteDocType(name, pubid, sysid, subset);
  }

  public void WriteStartElement(XmlQualifiedTagName tag)
  {
     //if (tag.Name == "LatentStyles")
     // TestTools.Stop();
    if (tag.Namespace != "" && EmitNamespaces)
    {
      if (!String.IsNullOrEmpty(tag.Prefix))
        _writer.WriteStartElement(tag.Prefix, tag.Name, tag.Namespace);
      else
        _writer.WriteStartElement(tag.Name, tag.Namespace);
      if (!NamespacesUsed.Contains(tag.Namespace))
        NamespacesUsed.Add(tag.Namespace);
    }
    else
      _writer.WriteStartElement(tag.Name);
    ElementStack.Push(tag);
  }

  public void WriteStartElement(string localName)
  {
    var fullName = new XmlQualifiedTagName(localName, "");
    _writer.WriteStartElement(localName);
    ElementStack.Push(fullName);
  }

  public void WriteEndElement(XmlQualifiedTagName fullName)
  {
    if (ElementStack.Count == 0)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as element stack is empty");
    var elementTag = ElementStack.Pop();
    if (fullName != elementTag)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as current element tag is \"{elementTag}\"");
    _writer.WriteEndElement();
  }

  public void WriteEndElement(string localName)
  {
    var fullName = new XmlQualifiedTagName(localName, "");
    if (ElementStack.Count == 0)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as element stack is empty");
    var elementTag = ElementStack.Pop();
    if (fullName != elementTag)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as current element tag is \"{elementTag}\"");
    _writer.WriteEndElement();
  }

  public void WriteFullEndElement(XmlQualifiedTagName fullName)
  {
    if (ElementStack.Count == 0)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as element stack is empty");
    var elementTag = ElementStack.Pop();
    if (fullName != elementTag)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as current element tag is \"{elementTag}\"");
    _writer.WriteFullEndElement();
  }

  public void WriteFullEndElement(string localName)
  {
    var fullName = new XmlQualifiedTagName(localName, "");
    if (ElementStack.Count == 0)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as element stack is empty");
    var elementTag = ElementStack.Pop();
    if (fullName != elementTag)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as current element tag is \"{elementTag}\"");
    _writer.WriteFullEndElement();
  }

  public void WriteStartAttribute(XmlQualifiedTagName fullName)
  {
    if (fullName.Namespace != "" && EmitNamespaces)
    {
      _writer.WriteStartAttribute(fullName.Name, fullName.Namespace);
      if (!NamespacesUsed.Contains(fullName.Namespace))
        NamespacesUsed.Add(fullName.Namespace);
    }
    else
      _writer.WriteStartAttribute(fullName.Name, fullName.Namespace);
    AttributeStack.Push(fullName);
  }

  public void WriteStartAttribute(string localName)
  {
    var fullName = new XmlQualifiedTagName(localName, "");
    _writer.WriteStartAttribute(localName);
    AttributeStack.Push(fullName);
  }

  public void WriteEndAttribute(XmlQualifiedTagName fullName)
  {
    if (AttributeStack.Count == 0)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as element stack is empty");
    var attributeTag = AttributeStack.Pop();
    if (fullName != attributeTag)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as current element tag is \"{attributeTag}\"");
    _writer.WriteEndAttribute();
  }

  public void WriteEndAttribute(string localName)
  {
    var fullName = new XmlQualifiedTagName(localName, "");
    if (AttributeStack.Count == 0)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as element stack is empty");
    var attributeTag = AttributeStack.Pop();
    if (fullName != attributeTag)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as current element tag is \"{attributeTag}\"");
    _writer.WriteEndAttribute();
  }

  //public void WriteCData(string? text)
  //{
  //  _writer.WriteCData(text);
  //}

  //public void WriteComment(string? text)
  //{
  //  _writer.WriteComment(text);
  //}

  //public void WriteProcessingInstruction(string name, string? text)
  //{
  //  _writer.WriteProcessingInstruction(name, text);
  //}

  //public void WriteEntityRef(string name)
  //{
  //  _writer.WriteEntityRef(name);
  //}

  //public void WriteCharEntity(char ch)
  //{
  //  _writer.WriteCharEntity(ch);
  //}

  public void WriteWhitespace(string? ws)
  {
    _writer.WriteWhitespace(ws);
  }

  public void WriteString(string? text)
  {
    _writer.WriteString(text);
  }

  public WriteState WriteState => _writer.WriteState;

  public void Close()
  {
    _writer.Close();
  }

  public void Flush()
  {
    _writer.Flush();
  }

  public XmlSpace XmlSpace => _writer.XmlSpace;

  public string? XmlLang => _writer.XmlLang;

  public void WriteValue(object value)
  {
    _writer.WriteValue(value);
  }

  public void WriteAttributeString(XmlQualifiedTagName fullName, string? str)
  {
    if (fullName.Namespace != "" && EmitNamespaces)
      _writer.WriteAttributeString(fullName.Name, str);
    else
      _writer.WriteAttributeString(fullName.Name, fullName.Namespace, str);
  }

  public void WriteAttributeString(string attrName, string? str)
  {
    _writer.WriteAttributeString(attrName, str);
  }

  public void WriteNamespaceDef(string prefix, string ns)
  {
    _writer.WriteAttributeString("xmlns", prefix, null, ns);
  }

  public void WriteNilAttribute()
  {
    _writer.WriteAttributeString(null, "nil", QXmlSerializationHelper.xsiNamespace, "true");
    XsiNamespaceUsed = true;
  }

  public void WriteValue(string? str)
  {
    if (str==null) return;
    if (_spaceBehavior == XmlSpace.Preserve)
      if (str.StartsWith(" ") || str.EndsWith(" ") || str.Contains('\n') || str.Contains('\r') || str.Contains('\t'))
        WriteSignificantSpaces(true);
    _writer.WriteValue(str);
  }

  //public void WriteElementString(string tagName, string? str)
  //{
  //  if (str == null) return;
  //  _writer.WriteStartElement(tagName);
  //  _writer.WriteValue(str);
  //  _writer.WriteEndElement();
  //}

  public void WriteSignificantSpaces(bool value)
  {
    _writer.WriteAttributeString("xml", "space", null, value ? "preserve" : "default");
  }

  private bool disposedValue;

  protected virtual void Dispose(bool disposing)
  {
    if (!disposedValue)
    {
      if (disposing)
      {
        Close();
      }

      // TODO: free unmanaged resources (unmanaged objects) and override finalizer
      // TODO: set large fields to null
      disposedValue = true;
    }
  }

  // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
  // ~QXmlWriter()
  // {
  //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
  //     Dispose(disposing: false);
  // }

  public void Dispose()
  {
    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
}
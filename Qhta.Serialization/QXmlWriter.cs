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

  
  public Stack<XmlQualifiedTagName> ElementStack { get; private set; } = new();
  public Stack<XmlQualifiedTagName> AttributeStack { get; private set; } = new();

  public void Dispose()
  {
    ((IDisposable)_writer).Dispose();
  }

  public bool EmitNamespaces
  {
    get
    {
      if (_writer is XmlTextWriter xmlTextWriter)
        return xmlTextWriter.Namespaces;
      return false;
    }
    set
    {
      if (_writer is XmlTextWriter xmlTextWriter)
        xmlTextWriter.Namespaces = value;
    }
  }

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


  public void WriteStartElement(XmlQualifiedTagName fullName)
  {
    if (fullName.Namespace != "")
      _writer.WriteStartElement(fullName.Name, fullName.Namespace);
    else
      _writer.WriteStartElement(fullName.Name);
    ElementStack.Push(fullName);
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
    _writer.WriteAttributeString(fullName.Name, fullName.Namespace);
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

  //public void WriteSurrogateCharEntity(char lowChar, char highChar)
  //{
  //  _writer.WriteSurrogateCharEntity(lowChar, highChar);
  //}

  //public void WriteChars(char[] buffer, int index, int count)
  //{
  //  _writer.WriteChars(buffer, index, count);
  //}

  //public void WriteRaw(char[] buffer, int index, int count)
  //{
  //  _writer.WriteRaw(buffer, index, count);
  //}

  //public void WriteRaw(string data)
  //{
  //  _writer.WriteRaw(data);
  //}

  //public void WriteBase64(byte[] buffer, int index, int count)
  //{
  //  _writer.WriteBase64(buffer, index, count);
  //}

  //public void WriteBinHex(byte[] buffer, int index, int count)
  //{
  //  _writer.WriteBinHex(buffer, index, count);
  //}

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

  //public string? LookupPrefix(string ns)
  //{
  //  return _writer.LookupPrefix(ns);
  //}



  //public void WriteNmToken(string name)
  //{
  //  _writer.WriteNmToken(name);
  //}

  //public void WriteName(string name)
  //{
  //  _writer.WriteName(name);
  //}

  //public void WriteQualifiedName(string localName, string? ns)
  //{
  //  _writer.WriteQualifiedName(localName, ns);
  //}

  public void WriteValue(object value)
  {
    _writer.WriteValue(value);
  }

  //public void WriteValue(bool value)
  //{
  //  _writer.WriteValue(value);
  //}

  //public void WriteValue(DateTime value)
  //{
  //  _writer.WriteValue(value);
  //}

  //public void WriteValue(DateTimeOffset value)
  //{
  //  _writer.WriteValue(value);
  //}

  //public void WriteValue(double value)
  //{
  //  _writer.WriteValue(value);
  //}

  //public void WriteValue(float value)
  //{
  //  _writer.WriteValue(value);
  //}

  //public void WriteValue(decimal value)
  //{
  //  _writer.WriteValue(value);
  //}

  //public void WriteValue(int value)
  //{
  //  _writer.WriteValue(value);
  //}

  //public void WriteValue(long value)
  //{
  //  _writer.WriteValue(value);
  //}

  //public void WriteAttributes(XmlReader reader, bool defattr)
  //{
  //  _writer.WriteAttributes(reader, defattr);
  //}

  //public void WriteNode(XmlReader reader, bool defattr)
  //{
  //  _writer.WriteNode(reader, defattr);
  //}

  //public void WriteNode(XPathNavigator navigator, bool defattr)
  //{
  //  _writer.WriteNode(navigator, defattr);
  //}

  //public void WriteElementString(string localName, string? ns, string? value)
  //{
  //  _writer.WriteElementString(localName, ns, value);
  //}

  //public void WriteElementString(string? prefix, string localName, string? ns, string? value)
  //{
  //  _writer.WriteElementString(prefix, localName, ns, value);
  //}

  public void WriteAttributeString(XmlQualifiedTagName fullName, string? str)
  {
    _writer.WriteAttributeString(fullName.Name, fullName.Namespace, str);
  }

  public void WriteAttributeString(string attrName, string? str)
  {
    _writer.WriteAttributeString(attrName, str);
  }

  public void WriteNamespaceDef(string prefix, string ns)
  {
    _writer.WriteAttributeString("xmlns", "xsi", null, ns);
  }

  public void WriteNilAttribute(string xsiNamespace)
  {
    _writer.WriteAttributeString(null, "nil", xsiNamespace, "true");
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
}
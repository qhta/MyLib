using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Xml.XPath;

namespace Qhta.Xml.Serialization;

/// <summary>
/// Wrapper for system XmlWriter used by QXmlSerializer
/// </summary>
public partial class QXmlWriter : IXmlWriter, IDisposable
{
  /// <summary>
  /// Initializing constructor
  /// </summary>
  public QXmlWriter(XmlWriter xmlWriter)
  {
    _writer = xmlWriter;
    _spaceBehavior = XmlSpace.Preserve;
  }

  private XmlWriter _writer { get; }

  /// <summary>
  /// Get wrapped Xml writer.
  /// </summary>
  /// <param name="writer"></param>
  public static implicit operator XmlWriter(QXmlWriter writer) => writer._writer;

  #region Reader state
  /// <summary>
  /// Wrapper for WriteState property.
  /// It gets the writer state.
  /// </summary>
  public WriteState WriteState => _writer.WriteState;

  /// <summary>
  /// Specifies whether starting and ending spaces should be preserved.
  /// It is needed as XmlSpace property is read-only.
  /// </summary>
  private XmlSpace _spaceBehavior { get; } 

  /// <summary>
  /// Wrapper for XmlSpace property.
  /// It gets the writer space behavior.
  /// </summary>
  public XmlSpace XmlSpace => _writer.XmlSpace;

  /// <summary>
  /// Wrapper for XmlLang property.
  /// The language information is communicated by writing an xml:lang attribute.
  /// </summary>
  public string? XmlLang => _writer.XmlLang;

  /// <summary>
  /// Option that specifies whether the reader used xsi namespace.
  /// </summary>
  public bool XsiNamespaceUsed { get; private set; }

  /// <summary>
  /// Option that specifies whether the reader used xsd namespace.
  /// </summary>
  public bool XsdNamespaceUsed { get; private set; }

  /// <summary>
  /// Sorted list of used namespaces.
  /// </summary>
  public SortedSet<string> NamespacesUsed { get; } = new();

  /// <summary>
  /// Element stack to help to pair write start element - write end element operations.
  /// </summary>
  public Stack<XmlQualifiedTagName> ElementStack { get; private set; } = new();

  /// <summary>
  /// Attribute stack to help to pair write start attribute - write end attribute operations.
  /// </summary>
  public Stack<XmlQualifiedTagName> AttributeStack { get; private set; } = new();
  #endregion

  #region settings
  /// <summary>
  /// System-defined XmlWriterSettings
  /// </summary>
  public XmlWriterSettings? Settings => _writer.Settings;

  /// <summary>
  /// Additional setting that specifies whether the writer should emit namespaces.
  /// </summary>
  public bool EmitNamespaces { get; set; } = true;
  #endregion

  /// <summary>
  /// Wrapper for WriteStartDocument operation. 
  /// It writes the XML declaration with the version "1.0".
  /// </summary>
  public void WriteStartDocument()
  {
    _writer.WriteStartDocument();
  }

    /// <summary>
  /// Wrapper for WriteStartDocument(standalone) operation. 
  /// It writes the XML declaration with the version "1.0" and the standalone attribute.
  /// </summary>
  public void WriteStartDocument(bool standalone)
  {
    _writer.WriteStartDocument(standalone);
  }

  /// <summary>
  /// Wrapper for WriteEndDocument operation. 
  /// It closes any open elements or attributes and puts the writer back in the Start state.
  /// </summary>
  public void WriteEndDocument()
  {
    _writer.WriteEndDocument();
  }

  /// <summary>
  /// Wrapper for WriteDocType operation. 
  /// It writes the DOCTYPE declaration with the specified name and optional attributes.
  /// </summary>
  /// <param name="name">The name of the DOCTYPE. This must be non-empty.</param>
  /// <param name="pubid">If non-null it also writes PUBLIC "pubid" "sysid" 
  /// where pubid and sysid are replaced with the value of the given arguments.</param>
  /// <param name="sysid">If pubid is null and sysid is non-null it writes SYSTEM "sysid" 
  /// where sysid is replaced with the value of this argument.</param>
  /// <param name="subset">If non-null it writes [subset] 
  /// where subset is replaced with the value of this argument.</param>
  public void WriteDocType(string name, string? pubid, string? sysid, string? subset)
  {
    _writer.WriteDocType(name, pubid, sysid, subset);
  }

  /// <summary>
  /// Wrapper for WriteStartElement operation. 
  /// It writes the specified start tag using tag namespace according to <see cref="EmitNamespaces"/> setting.
  /// Adds the tag namespace to <see cref="NamespacesUsed"/> list.
  /// </summary>
  public void WriteStartElement(XmlQualifiedTagName tag)
  {
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

  /// <summary>
  /// Wrapper for WriteStartElement operation. 
  /// Only local name (without namespace) is used as a parameter.
  /// </summary>
  public void WriteStartElement(string localName)
  {
    var fullName = new XmlQualifiedTagName(localName, "");
    _writer.WriteStartElement(localName);
    ElementStack.Push(fullName);
  }

  /// <summary>
  /// Wrapper for WriteEndElement operation.
  /// Must be paired with WriteStartElement operation.
  /// </summary>
  public void WriteEndElement(XmlQualifiedTagName fullName)
  {
    if (ElementStack.Count == 0)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as element stack is empty");
    var elementTag = ElementStack.Pop();
    if (fullName != elementTag)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as current element tag is \"{elementTag}\"");
    _writer.WriteEndElement();
  }

  /// <summary>
  /// Wrapper for WriteEndElement operation.
  /// Must be paired with WriteStartElement operation.
  /// </summary>
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

  /// <summary>
  /// Wrapper for WriteFullEndElement operation.
  /// It always writes XML end element event the start element has no content.
  /// It must be paired with WriteStartElement operation.
  /// </summary>
  public void WriteFullEndElement(XmlQualifiedTagName fullName)
  {
    if (ElementStack.Count == 0)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as element stack is empty");
    var elementTag = ElementStack.Pop();
    if (fullName != elementTag)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as current element tag is \"{elementTag}\"");
    _writer.WriteFullEndElement();
  }

  /// <summary>
  /// Wrapper for WriteFullEndElement operation.
  /// It always writes XML end element event the start element has no content.
  /// It must be paired with WriteStartElement operation.
  /// </summary>
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

  /// <summary>
  /// Wrapper for WriteStartAttribute operation. 
  /// It writes the specified start tag using tag namespace according to <see cref="EmitNamespaces"/> setting.
  /// Adds the tag namespace to <see cref="NamespacesUsed"/> list.
  /// </summary>
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

  /// <summary>
  /// Wrapper for WriteStartAttribute operation. 
  /// Only local name (without namespace) is used as a parameter.
  /// </summary>
  public void WriteStartAttribute(string localName)
  {
    var fullName = new XmlQualifiedTagName(localName, "");
    _writer.WriteStartAttribute(localName);
    AttributeStack.Push(fullName);
  }

  /// <summary>
  /// Wrapper for WriteEndAttribute operation.
  /// Must be paired with WriteStartAttribute operation.
  /// </summary>
  public void WriteEndAttribute(XmlQualifiedTagName fullName)
  {
    if (AttributeStack.Count == 0)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as element stack is empty");
    var attributeTag = AttributeStack.Pop();
    if (fullName != attributeTag)
      throw new InvalidOperationException($"Can't write end element \"{fullName}\" as current element tag is \"{attributeTag}\"");
    _writer.WriteEndAttribute();
  }

  /// <summary>
  /// Wrapper for WriteEndAttribute operation.
  /// Must be paired with WriteStartAttribute operation.
  /// </summary>
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

  #region unneeded operations
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
  #endregion

  /// <summary>
  /// Wrapper for WriteWhitespace operation.
  /// </summary>
  /// <param name="ws">The string of white space characters.</param>
  public void WriteWhitespace(string? ws)
  {
    _writer.WriteWhitespace(ws);
  }

  /// <summary>
  /// Wrapper for WriteString operation.
  /// </summary>
  /// <param name="text">The text to write.</param>
  public void WriteString(string? text)
  {
    _writer.WriteString(text);
  }

  /// <summary>
  /// Wrapper for WriteValue operation.
  /// It writes a single simple-typed value.
  /// </summary>
  /// <param name="value">Single simple-typed value to write.</param>
  public void WriteValue(object value)
  {
    _writer.WriteValue(value);
  }

  /// <summary>
  /// Wrapper for WriteAttributeString operation.
  /// Writes out the attribute with the specified prefix, local name, namespace URI, and value.
  /// </summary>
  public void WriteAttributeString(XmlQualifiedTagName fullName, string? str)
  {
    if (fullName.Namespace != "" && EmitNamespaces)
      _writer.WriteAttributeString(fullName.Name, fullName.Namespace, str);
    else
      _writer.WriteAttributeString(fullName.Name, str);
  }

  /// <summary>
  /// Wrapper for WriteAttributeString operation.
  /// Writes out the attribute with the specified local name and value.
  /// </summary>
  public void WriteAttributeString(string attrName, string? str)
  {
    _writer.WriteAttributeString(attrName, str);
  }

  /// <summary>
  /// Writes the specified namespace definition usint xmlns prefix.
  /// </summary>
  public void WriteNamespaceDef(string prefix, string ns)
  {
    _writer.WriteAttributeString("xmlns", prefix, null, ns);
  }

  /// <summary>
  /// Writes a "xsi:nil='true'" attribute
  /// </summary>
  public void WriteNilAttribute()
  {
    _writer.WriteAttributeString(null, "nil", QXmlSerializationHelper.xsiNamespace, "true");
    XsiNamespaceUsed = true;
  }

  /// <summary>
  /// Writes out a string with starting or ending whitespaces.
  /// </summary>
  /// <param name="str"></param>
  public void WriteValue(string? str)
  {
    if (str==null) return;
    if (_spaceBehavior == XmlSpace.Preserve)
      if (str.StartsWith(" ") || str.EndsWith(" ") || str.Contains('\n') || str.Contains('\r') || str.Contains('\t'))
        WriteSignificantSpaces(true);
    _writer.WriteValue(str);
  }


  /// <summary>
  /// Writes out a "xml:space=..." attribute.
  /// </summary>
  /// <param name="value"></param>
  public void WriteSignificantSpaces(bool value)
  {
    _writer.WriteAttributeString("xml", "space", null, value ? "preserve" : "default");
  }


  /// <summary>
  /// Wrapper for Close operation
  /// </summary>
  public void Close()
  {
    _writer.Close();
  }

  /// <summary>
  /// Wrapper for Flush operation
  /// </summary>
  public void Flush()
  {
    _writer.Flush();
  }


  private bool disposedValue;

  /// <summary>
  /// Wrapper for Dispose operation
  /// </summary>
  public void Dispose()
  {
    _writer.Dispose();
  }
}
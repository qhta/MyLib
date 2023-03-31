namespace Qhta.Xml.Serialization;

/// <summary>
/// Wrapper for system XmlReader used by QXmlSerializer
/// </summary>
public partial class QXmlReader : IXmlReader, IDisposable
{
  /// <summary>
  /// Initializing constructor
  /// </summary>
  public QXmlReader(XmlReader xmlReader)
  {
    _reader = xmlReader;
  }

  private XmlReader _reader { get; }


  /// <summary>
  /// Get wrapped XML reader.
  /// </summary>
  public static implicit operator XmlReader(QXmlReader reader) => reader._reader;

  #region Reader state

  /// <summary>
  /// Getting LineNumber and LinePosition of the reader
  /// </summary>
  public (int line, int pos) GetPosition()
  {
    if (_reader is XmlTextReader xmlTextReader)
    {
      var lineNumber = xmlTextReader.LineNumber;
      var linePosition = xmlTextReader.LinePosition;
      return (lineNumber, linePosition);
    }
    return default; 
  }


  /// <summary>
  /// Limit divides text to separate lines.
  /// </summary>
  public int LineLengthLimit { get; set; } = 0;

  /// <summary>
  /// Wrapped EOF property.
  /// </summary>
  public bool EOF => _reader.EOF;

  /// <summary>
  /// Wrapped ReadState property.
  /// </summary>
  public ReadState ReadState => _reader.ReadState;

  /// <summary>
  /// Wrapped NodeType property
  /// </summary>
  public XmlNodeType NodeType => _reader.NodeType;

  /// <summary>
  /// Qualified name (local name and namespace URI)
  /// </summary>
  public XmlQualifiedTagName Name
  {
    get
    {
      var localName = _reader.LocalName;
      var uri = _reader.NamespaceURI;
      if (!String.IsNullOrEmpty(uri))
        return new XmlQualifiedTagName(localName, uri);
      return new XmlQualifiedTagName(localName);
    }
  }

  /// <summary>
  /// Wrapper for LocalName property.
  /// </summary>
  public string LocalName => _reader.LocalName;

  /// <summary>
  /// Wrapper for NamespaceURI property.
  /// </summary>
  public string NamespaceURI => _reader.NamespaceURI;

  /// <summary>
  /// Wrapper for Prefix property.
  /// </summary>
  public string Prefix => _reader.Prefix;

  /// <summary>
  /// Wrapper for HasValue property.
  /// </summary>
  public bool HasValue => _reader.HasValue;

  /// <summary>
  /// Wrapper for ValueType property.
  /// </summary>
  public Type ValueType => _reader.ValueType;

  /// <summary>
  /// Wrapper for Value property.
  /// </summary>
  public string Value => _reader.Value;

  /// <summary>
  /// Wrapper for Depth property.
  /// </summary>
  public int Depth => _reader.Depth;

  /// <summary>
  /// Wrapper for BaseURI property.
  /// </summary>
  public string BaseURI => _reader.BaseURI;

  /// <summary>
  /// Wrapper for IsEmptyElement property.
  /// </summary>
  public bool IsEmptyElement => _reader.IsEmptyElement;

  /// <summary>
  /// Wrapper for IsDefault property.
  /// </summary>
  public bool IsDefault => _reader.IsDefault;

  /// <summary>
  /// Wrapper for QuoteChar property.
  /// </summary>
  public char QuoteChar => _reader.QuoteChar;

  /// <summary>
  /// Wrapper for XmlSpace property.
  /// </summary>
  public XmlSpace XmlSpace => _reader.XmlSpace;

  /// <summary>
  /// Wrapper for XmlLang property.
  /// </summary>
  public string XmlLang => _reader.XmlLang;

  #endregion

  #region Settings

  /// <summary>
  /// Wrapper for Settings property.
  /// </summary>
  public XmlReaderSettings? Settings => _reader.Settings;

  /// <summary>
  /// Additional setting that specifies how whitespaces are handled.
  /// </summary>
  public WhitespaceHandling? WhitespaceHandling
  {
    get { return (_reader as XmlTextReader)?.WhitespaceHandling; }
    set
    {
      if (_reader is XmlTextReader xmlTextReader && value != null)
        xmlTextReader.WhitespaceHandling = (WhitespaceHandling)value;
    }
  }
  #endregion

  #region Attribute accessors

  /// <summary>
  /// Wrapper for HasAttributes property.
  /// Gets a value indicating whether the current node has any attributes.
  /// </summary>
  public bool HasAttributes => _reader.HasAttributes;

  /// <summary>
  /// Wrapper for AttributeCount property.
  /// Gets the number of attributes on the current node.
  /// </summary>
  public int AttributeCount => _reader.AttributeCount;

  /// <summary>
  /// Wrapper for int indexed item accessors.
  /// Gets the value of the attribute with the specified index.
  /// </summary>
  public string this[int i] => _reader[i];

  /// <summary>
  /// Wrapper for local name indexed item accessors.
  /// Gets the value of the attribute with the specified Name.
  /// </summary>
  public string? this[string name] => _reader[name];

  /// <summary>
  /// Wrapper for tag name indexed item accessors.
  /// Gets the value of the attribute with the specified LocalName and NamespaceURI.
  /// </summary>
  public string? this[XmlQualifiedTagName fullName]
    => _reader[fullName.Name, fullName.Namespace];


  /// <summary>
  /// Wrapper for GetAttribute operation.
  /// Gets the value of the attribute with the specified Name.
  /// </summary>
  public string? GetAttribute(string name)
  {
    return _reader.GetAttribute(name);
  }

  /// <summary>
  /// Wrapper for GetAttribute operation.
  /// Gets the value of the attribute with the specified LocalName and NamespaceURI.
  /// </summary>
  public string? GetAttribute(XmlQualifiedTagName fullName)
  {
    return _reader.GetAttribute(fullName.Name, fullName.Namespace);
  }

  /// <summary>
  /// Wrapper for GetAttribute operation.
  /// Gets the value of the attribute with the specified index.
  /// </summary>
  public string GetAttribute(int i)
  {
    return _reader.GetAttribute(i);
  }

  
  /// <summary>
  /// Wrapper for ReadAttributeValue operation.
  /// Parses the attribute value into one or more Text, EntityReference, or EndEntity nodes.
  /// </summary>
  /// <returns>True if there are nodes to return. False otherwise.</returns>
  public bool ReadAttributeValue()
  {
    return _reader.ReadAttributeValue();
  }

  #endregion

  #region Content accessors

  /// <summary>
  /// Gets a string from element content or gets content as string.
  /// </summary>
  public string ReadString()
  {
    string str;
    if (_reader.NodeType == XmlNodeType.Element)
      str = _reader.ReadElementContentAsString();
    else
      str = _reader.ReadContentAsString();
    //if (str == " ")
    //  Debugger.Break();
    return str;
  }

  /// <summary>
  /// Wrapper for ReadContentAs operation.
  /// Reads the content as an object of the type specified.
  /// </summary>
  public object ReadContentAs(Type returnType)
  {
    return _reader.ReadContentAs(returnType, null);
  }

  /// <summary>
  /// Wrapper for ReadElementContentAsString operation.
  /// Reads the current element and returns the contents as a String object.
  /// </summary>
  public string ReadElementContentAsString()
  {
    return _reader.ReadElementContentAsString();
  }

  /// <summary>
  /// Wrapper for ReadElementContentAsString operation.
  /// Checks that the specified local name and namespace URI matches that of the current element, 
  /// then reads the current element and returns the contents as a String object.
  /// </summary>
  public string ReadElementContentAsString(XmlQualifiedTagName fullName)
  {
    return _reader.ReadElementContentAsString(fullName.Name, fullName.Namespace);
  }
  #endregion

  #region Element accessors
  /// <summary>
  /// Wrapper for ReadElementString operation.
  /// Reads a text-only element.
  /// </summary>
  public string ReadElementString()
  {
    return _reader.ReadElementString();
  }

  /// <summary>
  /// Wrapper for ReadElementString operation. 
  /// However, we recommend that you use the ReadElementContentAsString() method instead, 
  /// because it provides a more straightforward way to handle this operation.
  /// </summary>
  public string ReadElementString(string name)
  {
    return _reader.ReadElementString(name);
  }

  /// <summary>
  /// Wrapper for ReadElementString operation.
  /// Checks that the Name property of the element found matches the given string before reading a text-only element. 
  /// However, we recommend that you use the ReadElementContentAsString() method instead, because it provides a more straightforward way to handle this operation.
  /// </summary>
  public string ReadElementString(XmlQualifiedTagName fullName)
  {
    return _reader.ReadElementString(fullName.Name, fullName.Namespace);
  }
  #endregion

  #region Movement
  /// <summary>
  /// Wrapper for MoveToAttribute operation.
  /// Moves to the attribute with the specified Name.
  /// </summary>
  public bool MoveToAttribute(string name)
  {
    return _reader.MoveToAttribute(name);
  }

  /// <summary>
  /// Wrapper for MoveToAttribute operation.
  /// Moves to the attribute with the specified LocalName and NamespaceURI.
  /// </summary>
  public bool MoveToAttribute(XmlQualifiedTagName fullName)
  {
    return _reader.MoveToAttribute(fullName.Name, fullName.Namespace);
  }

  /// <summary>
  /// Wrapper for MoveToAttribute operation.
  /// Moves to the attribute with the specified index.
  /// </summary>
  public void MoveToAttribute(int i)
  {
    _reader.MoveToAttribute(i);
  }

  /// <summary>
  /// Wrapper for MoveToFirstAttribute operation.
  /// Moves to the first attribute.
  /// </summary>
  public bool MoveToFirstAttribute()
  {
    return _reader.MoveToFirstAttribute();
  }

  /// <summary>
  /// Wrapper for MoveToNextAttribute operation.
  /// Moves to the next attribute.
  /// </summary>
  public bool MoveToNextAttribute()
  {
    return _reader.MoveToNextAttribute();
  }

  /// <summary>
  /// Wrapper for MoveToElement operation.
  /// Moves to the element that contains the current attribute node.
  /// </summary>
  public bool MoveToElement()
  {
    return _reader.MoveToElement();
  }

  /// <summary>
  /// Wrapper for MoveToContent operation.
  /// Checks whether the current node is a content (non-white space text, CDATA, Element, EndElement, EntityReference, or EndEntity) node. 
  /// If the node is not a content node, the reader skips ahead to the next content node or end of file. 
  /// It skips over nodes of the following type: ProcessingInstruction, DocumentType, Comment, Whitespace, or SignificantWhitespace.
  /// </summary>
  public XmlNodeType MoveToContent()
  {
    return _reader.MoveToContent();
  }

  /// <summary>
  /// Wrapper for Skip operation.
  /// Skips the children of the current node.
  /// </summary>
  public void Skip()
  {
    _reader.Skip();
  }
  #endregion

  #region Read operations

  /// <summary>
  /// Wrapper for Read operation.
  /// Reads the next node from the stream.
  /// </summary>
  public void Read()
  {
    _reader.Read();
  }

  /// <summary>
  /// Returns a new readed instance that can be used to read the current node, and all its descendants.
  /// </summary>
  /// <returns>new QXmlReader</returns>
  /// 
  public QXmlReader ReadSubtree ()
  {
    return new QXmlReader(_reader.ReadSubtree());
  }
  #endregion

  #region start element checking and read

  /// <summary>
  /// Wrapper for IsStartElement operation.
  /// Calls MoveToContent() and tests if the current content node is a start tag or empty element tag.
  /// </summary>
  public bool IsStartElement()
  {
    return _reader.IsStartElement();
  }

  /// <summary>
  /// Wrapper for IsStartElement operation.
  /// Calls MoveToContent() and tests if the current content node is a start tag or empty element tag 
  /// and if the Name property of the element found matches the given argument.
  /// </summary>
  public bool IsStartElement(string name)
  {
    return _reader.IsStartElement(name);
  }

  /// <summary>
  /// Wrapper for IsStartElement operation.
  /// Calls MoveToContent() and tests if the current content node is a start tag or empty element tag 
  /// and if the LocalName and NamespaceURI properties of the element found match the given strings.
  /// </summary>
  public bool IsStartElement(XmlQualifiedTagName tag)
  {
    return _reader.IsStartElement(tag.Name, tag.Namespace);
  }

  /// <summary>
  /// Wrapper for ReadStartElement operation.
  /// Checks that the current node is an element and advances the reader to the next node.
  /// </summary>
  public void ReadStartElement()
  {
    _reader.ReadStartElement();
  }

  /// <summary>
  /// Wrapper for ReadStartElement operation.
  /// Checks that the current content node is an element with the given Name and advances the reader to the next node.
  /// </summary>
  public void ReadStartElement(string name)
  {
    _reader.ReadStartElement(name);
  }

  /// <summary>
  /// Wrapper for ReadStartElement operation.
  /// Checks that the current content node is an element with the given LocalName 
  /// and NamespaceURI and advances the reader to the next node.
  /// </summary>
  public void ReadStartElement(XmlQualifiedTagName tag)
  {
    _reader.ReadStartElement(tag.Name, tag.Namespace);
  }
  #endregion

  #region end element checking and read
  /// <summary>
  /// Checks if reader node type is EndElement.
  /// </summary>
  public bool IsEndElement()
  {
    return _reader.NodeType == XmlNodeType.EndElement;
  }

  /// <summary>
  /// Checks if reader node type is EndElement and reade name is a specified string
  /// </summary>
  public bool IsEndElement(string name)
  {
    return _reader.NodeType == XmlNodeType.EndElement && _reader.Name == name;
  }

  /// <summary>
  /// Checks if reader node type is EndElement and reade name is a specified local name and namespaceURI is a specified namespace.
  /// </summary>
  public bool IsEndElement(XmlQualifiedTagName tag)
  {
    return _reader.NodeType == XmlNodeType.EndElement && _reader.LocalName == tag.Name && _reader.NamespaceURI == tag.Namespace;
  }

  /// <summary>
  /// Wrapper for ReadEndElement operation.
  /// Checks that the current content node is an end tag and advances the reader to the next node.
  /// </summary>
  public void ReadEndElement()
  {
    _reader.ReadEndElement();
  }

  /// <summary>
  /// Wrapper for ReadEndElement operation.
  /// Checks that the current content node is an end tag with a specified name and advances the reader to the next node.
  /// </summary>
  /// <exception cref="XmlInternalException">Thrown if node is not end element or name does not match</exception>
  public void ReadEndElement(string name)
  {
    var ok = _reader.NodeType == XmlNodeType.EndElement && _reader.Name == name;
    if (!ok)
      throw new XmlInternalException($"End element \"{name}\" expected but {_reader.NodeType} \"{_reader.Name}\" found", _reader);
    _reader.ReadEndElement();
  }

  /// <summary>
  /// Wrapper for ReadEndElement operation.
  /// Checks that the current content node is an end tag with a specified name and namespaceURI and advances the reader to the next node.
  /// </summary>
  /// <exception cref="XmlInternalException">Thrown if node is not end element or name and namespaceURI does not match</exception>
  public void ReadEndElement(XmlQualifiedTagName tag)
  {
    var ok = _reader.NodeType == XmlNodeType.EndElement && _reader.LocalName == tag.Name && _reader.NamespaceURI == tag.Namespace;
    if (!ok)
      throw new XmlInternalException($"End element \"{tag}\" expected but {_reader.NodeType} \"{_reader.Name}\" found", _reader);
    _reader.ReadEndElement();
  }
  #endregion

  #region closing methods
  /// <summary>
  /// Wrapper for Close operation.
  /// </summary>
  public void Close()
  {
    _reader.Close();
  }

  /// <summary>
  /// Wrapper for Dispose operation.
  /// </summary>
  public void Dispose()
  {
    _reader.Dispose();
  }
  #endregion
}
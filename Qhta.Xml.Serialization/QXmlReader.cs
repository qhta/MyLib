namespace Qhta.Xml.Serialization;

/// <summary>
/// Wrapper for system XmlReader used by QXmlSerializer.
/// </summary>
public partial class QXmlReader : IXmlReader, IDisposable
{
  /// <summary>
  /// Initializing constructor
  /// </summary>
  public QXmlReader(XmlReader xmlReader)
  {
    BaseXmlReader = xmlReader;
  }

  /// <summary>
  /// Base XmlReader reference.
  /// </summary>
  public XmlReader BaseXmlReader { get; }


  /// <summary>
  /// Get wrapped XML reader.
  /// </summary>
  public static implicit operator XmlReader(QXmlReader reader) => reader.BaseXmlReader;

  #region Reader state

  /// <summary>
  /// Getting LineNumber and LinePosition of the reader
  /// </summary>
  public (int line, int pos) GetPosition()
  {
    if (BaseXmlReader is XmlTextReader xmlTextReader)
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
  public bool EOF => BaseXmlReader.EOF;

  /// <summary>
  /// Wrapped ReadState property.
  /// </summary>
  public ReadState ReadState => BaseXmlReader.ReadState;

  /// <summary>
  /// Wrapped NodeType property
  /// </summary>
  public XmlNodeType NodeType => BaseXmlReader.NodeType;

  /// <summary>
  /// Qualified name (local name and namespace URI)
  /// </summary>
  public XmlQualifiedTagName Name
  {
    get
    {
      var localName = BaseXmlReader.LocalName;
      var uri = BaseXmlReader.NamespaceURI;
      if (!String.IsNullOrEmpty(uri))
        return new XmlQualifiedTagName(localName, uri);
      return new XmlQualifiedTagName(localName);
    }
  }

  /// <summary>
  /// Wrapper for LocalName property.
  /// </summary>
  public string LocalName => BaseXmlReader.LocalName;

  /// <summary>
  /// Wrapper for NamespaceURI property.
  /// </summary>
  public string NamespaceURI => BaseXmlReader.NamespaceURI;

  /// <summary>
  /// Wrapper for Prefix property.
  /// </summary>
  public string Prefix => BaseXmlReader.Prefix;

  /// <summary>
  /// Wrapper for HasValue property.
  /// </summary>
  public bool HasValue => BaseXmlReader.HasValue;

  /// <summary>
  /// Wrapper for ValueType property.
  /// </summary>
  public Type ValueType => BaseXmlReader.ValueType;

  /// <summary>
  /// Wrapper for Value property.
  /// </summary>
  public string Value => BaseXmlReader.Value;

  /// <summary>
  /// Wrapper for Depth property.
  /// </summary>
  public int Depth => BaseXmlReader.Depth;

  /// <summary>
  /// Wrapper for BaseURI property.
  /// </summary>
  public string BaseURI => BaseXmlReader.BaseURI;

  /// <summary>
  /// Wrapper for IsEmptyElement property.
  /// </summary>
  public bool IsEmptyElement => BaseXmlReader.IsEmptyElement;

  /// <summary>
  /// Wrapper for IsDefault property.
  /// </summary>
  public bool IsDefault => BaseXmlReader.IsDefault;

  /// <summary>
  /// Wrapper for QuoteChar property.
  /// </summary>
  public char QuoteChar => BaseXmlReader.QuoteChar;

  /// <summary>
  /// Wrapper for XmlSpace property.
  /// </summary>
  public XmlSpace XmlSpace => BaseXmlReader.XmlSpace;

  /// <summary>
  /// Wrapper for XmlLang property.
  /// </summary>
  public string XmlLang => BaseXmlReader.XmlLang;

  /// <summary>
  /// Gets line number of XML text file where the exception occured.
  /// </summary>
  public int? LineNumber
  {
    get
    {
      // We cant simply typecast BaseXmlReader to XmlTextReader as it is of XmlTextReaderImpl type.
      var type = BaseXmlReader.GetType();
      if (type != null)
      {
        var prop = type.GetProperty("LineNumber");
        if (prop != null)
        {
          var result = prop.GetValue(BaseXmlReader);
          if (result is int n)
            return n;
        }
      }
      return null;
    }
  }

  /// <summary>
  /// Gets the position in line of XML text file where the exception occured.
  /// </summary>
  public int? LinePosition
  {
    get
    {
      // We cant simply typecast BaseXmlReader to XmlTextReader as it is of XmlTextReaderImpl type.
      var type = BaseXmlReader.GetType();
      if (type != null)
      {
        var prop = type.GetProperty("LinePosition");
        if (prop != null)
        {
          var result = prop.GetValue(BaseXmlReader);
          if (result is int n)
            return n;
        }
      }
      return null;
    }
  }

  #endregion

  #region Settings

  /// <summary>
  /// Wrapper for Settings property.
  /// </summary>
  public XmlReaderSettings? Settings => BaseXmlReader.Settings;

  /// <summary>
  /// Additional setting that specifies how whitespaces are handled.
  /// </summary>
  public WhitespaceHandling? WhitespaceHandling
  {
    get { return (BaseXmlReader as XmlTextReader)?.WhitespaceHandling; }
    set
    {
      if (BaseXmlReader is XmlTextReader xmlTextReader && value != null)
        xmlTextReader.WhitespaceHandling = (WhitespaceHandling)value;
    }
  }
  #endregion

  #region Attribute accessors

  /// <summary>
  /// Wrapper for HasAttributes property.
  /// Gets a value indicating whether the current node has any attributes.
  /// </summary>
  public bool HasAttributes => BaseXmlReader.HasAttributes;

  /// <summary>
  /// Wrapper for AttributeCount property.
  /// Gets the number of attributes on the current node.
  /// </summary>
  public int AttributeCount => BaseXmlReader.AttributeCount;

  /// <summary>
  /// Wrapper for int indexed item accessors.
  /// Gets the value of the attribute with the specified index.
  /// </summary>
  public string this[int i] => BaseXmlReader[i];

  /// <summary>
  /// Wrapper for local name indexed item accessors.
  /// Gets the value of the attribute with the specified Name.
  /// </summary>
  public string? this[string name] => BaseXmlReader[name];

  /// <summary>
  /// Wrapper for tag name indexed item accessors.
  /// Gets the value of the attribute with the specified LocalName and NamespaceURI.
  /// </summary>
  public string? this[XmlQualifiedTagName fullName]
    => BaseXmlReader[fullName.LocalName, fullName.Namespace];


  /// <summary>
  /// Wrapper for GetAttribute operation.
  /// Gets the value of the attribute with the specified Name.
  /// </summary>
  public string? GetAttribute(string name)
  {
    return BaseXmlReader.GetAttribute(name);
  }

  /// <summary>
  /// Wrapper for GetAttribute operation.
  /// Gets the value of the attribute with the specified LocalName and NamespaceURI.
  /// </summary>
  public string? GetAttribute(XmlQualifiedTagName fullName)
  {
    return BaseXmlReader.GetAttribute(fullName.LocalName, fullName.Namespace);
  }

  /// <summary>
  /// Wrapper for GetAttribute operation.
  /// Gets the value of the attribute with the specified index.
  /// </summary>
  public string GetAttribute(int i)
  {
    return BaseXmlReader.GetAttribute(i);
  }


  /// <summary>
  /// Wrapper for ReadAttributeValue operation.
  /// Parses the attribute value into one or more Text, EntityReference, or EndEntity nodes.
  /// </summary>
  /// <returns>True if there are nodes to return. False otherwise.</returns>
  public bool ReadAttributeValue()
  {
    return BaseXmlReader.ReadAttributeValue();
  }

  #endregion

  #region Content accessors

  /// <summary>
  /// Gets a string from element content or gets content as string.
  /// </summary>
  public string ReadString()
  {
    string str;
    if (BaseXmlReader.NodeType == XmlNodeType.Element)
      str = BaseXmlReader.ReadElementContentAsString();
    else
      str = BaseXmlReader.ReadContentAsString();
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
    return BaseXmlReader.ReadContentAs(returnType, null);
  }

  /// <summary>
  /// Wrapper for ReadElementContentAsString operation.
  /// Reads the current element and returns the contents as a String object.
  /// </summary>
  public string ReadElementContentAsString()
  {
    return BaseXmlReader.ReadElementContentAsString();
  }

  /// <summary>
  /// Wrapper for ReadElementContentAsString operation.
  /// Checks that the specified local name and namespace URI matches that of the current element, 
  /// then reads the current element and returns the contents as a String object.
  /// </summary>
  public string ReadElementContentAsString(XmlQualifiedTagName fullName)
  {
    return BaseXmlReader.ReadElementContentAsString(fullName.LocalName, fullName.Namespace);
  }
  #endregion

  #region Element accessors
  /// <summary>
  /// Wrapper for ReadElementString operation.
  /// Reads a text-only element.
  /// </summary>
  public string ReadElementString()
  {
    return BaseXmlReader.ReadElementString();
  }

  /// <summary>
  /// Wrapper for ReadElementString operation. 
  /// However, we recommend that you use the ReadElementContentAsString() method instead, 
  /// because it provides a more straightforward way to handle this operation.
  /// </summary>
  public string ReadElementString(string name)
  {
    return BaseXmlReader.ReadElementString(name);
  }

  /// <summary>
  /// Wrapper for ReadElementString operation.
  /// Checks that the Name property of the element found matches the given string before reading a text-only element. 
  /// However, we recommend that you use the ReadElementContentAsString() method instead, because it provides a more straightforward way to handle this operation.
  /// </summary>
  public string ReadElementString(XmlQualifiedTagName fullName)
  {
    return BaseXmlReader.ReadElementString(fullName.LocalName, fullName.Namespace);
  }
  #endregion

  #region Movement
  /// <summary>
  /// Wrapper for MoveToAttribute operation.
  /// Moves to the attribute with the specified Name.
  /// </summary>
  public bool MoveToAttribute(string name)
  {
    return BaseXmlReader.MoveToAttribute(name);
  }

  /// <summary>
  /// Wrapper for MoveToAttribute operation.
  /// Moves to the attribute with the specified LocalName and NamespaceURI.
  /// </summary>
  public bool MoveToAttribute(XmlQualifiedTagName fullName)
  {
    return BaseXmlReader.MoveToAttribute(fullName.LocalName, fullName.Namespace);
  }

  /// <summary>
  /// Wrapper for MoveToAttribute operation.
  /// Moves to the attribute with the specified index.
  /// </summary>
  public void MoveToAttribute(int i)
  {
    BaseXmlReader.MoveToAttribute(i);
  }

  /// <summary>
  /// Wrapper for MoveToFirstAttribute operation.
  /// Moves to the first attribute.
  /// </summary>
  public bool MoveToFirstAttribute()
  {
    return BaseXmlReader.MoveToFirstAttribute();
  }

  /// <summary>
  /// Wrapper for MoveToNextAttribute operation.
  /// Moves to the next attribute.
  /// </summary>
  public bool MoveToNextAttribute()
  {
    return BaseXmlReader.MoveToNextAttribute();
  }

  /// <summary>
  /// Wrapper for MoveToElement operation.
  /// Moves to the element that contains the current attribute node.
  /// </summary>
  public bool MoveToElement()
  {
    return BaseXmlReader.MoveToElement();
  }

  /// <summary>
  /// Wrapper for MoveToContent operation.
  /// Checks whether the current node is a content (non-white space text, CDATA, Element, EndElement, EntityReference, or EndEntity) node. 
  /// If the node is not a content node, the reader skips ahead to the next content node or end of file. 
  /// It skips over nodes of the following type: ProcessingInstruction, DocumentType, Comment, Whitespace, or SignificantWhitespace.
  /// </summary>
  public XmlNodeType MoveToContent()
  {
    return BaseXmlReader.MoveToContent();
  }

  /// <summary>
  /// Wrapper for Skip operation.
  /// Skips the children of the current node.
  /// </summary>
  public void Skip()
  {
    BaseXmlReader.Skip();
  }
  #endregion

  #region Read operations

  /// <summary>
  /// Wrapper for Read operation.
  /// Reads the next node from the stream.
  /// </summary>
  public void Read()
  {
    BaseXmlReader.Read();
  }

  /// <summary>
  /// Returns a new readed instance that can be used to read the current node, and all its descendants.
  /// </summary>
  /// <returns>new QXmlReader</returns>
  /// 
  public QXmlReader ReadSubtree()
  {
    return new QXmlReader(BaseXmlReader.ReadSubtree());
  }
  #endregion

  #region start element checking and read

  /// <summary>
  /// Wrapper for IsStartElement operation.
  /// Calls MoveToContent() and tests if the current content node is a start tag or empty element tag.
  /// </summary>
  public bool IsStartElement()
  {
    return BaseXmlReader.IsStartElement();
  }

  /// <summary>
  /// Wrapper for IsStartElement operation.
  /// Calls MoveToContent() and tests if the current content node is a start tag or empty element tag 
  /// and if the Name property of the element found matches the given argument.
  /// </summary>
  public bool IsStartElement(string name)
  {
    return BaseXmlReader.IsStartElement(name);
  }

  /// <summary>
  /// Wrapper for IsStartElement operation.
  /// Calls MoveToContent() and tests if the current content node is a start tag or empty element tag 
  /// and if the LocalName and NamespaceURI properties of the element found match the given strings.
  /// </summary>
  public bool IsStartElement(XmlQualifiedTagName tag)
  {
    return BaseXmlReader.IsStartElement(tag.LocalName, tag.Namespace);
  }

  /// <summary>
  /// Wrapper for ReadStartElement operation.
  /// Checks that the current node is an element and advances the reader to the next node.
  /// </summary>
  public void ReadStartElement()
  {
    BaseXmlReader.ReadStartElement();
  }

  /// <summary>
  /// Wrapper for ReadStartElement operation.
  /// Checks that the current content node is an element with the given Name and advances the reader to the next node.
  /// </summary>
  public void ReadStartElement(string name)
  {
    BaseXmlReader.ReadStartElement(name);
  }

  /// <summary>
  /// Wrapper for ReadStartElement operation.
  /// Checks that the current content node is an element with the given LocalName 
  /// and NamespaceURI and advances the reader to the next node.
  /// </summary>
  public void ReadStartElement(XmlQualifiedTagName tag)
  {
    BaseXmlReader.ReadStartElement(tag.LocalName, tag.Namespace);
  }
  #endregion

  #region end element checking and read
  /// <summary>
  /// Checks if reader node type is EndElement.
  /// </summary>
  public bool IsEndElement()
  {
    return BaseXmlReader.NodeType == XmlNodeType.EndElement;
  }

  /// <summary>
  /// Checks if reader node type is EndElement and reade name is a specified string
  /// </summary>
  public bool IsEndElement(string name)
  {
    return BaseXmlReader.NodeType == XmlNodeType.EndElement && BaseXmlReader.Name == name;
  }

  /// <summary>
  /// Checks if reader node type is EndElement and reade name is a specified local name and namespaceURI is a specified namespace.
  /// </summary>
  public bool IsEndElement(XmlQualifiedTagName tag)
  {
    return BaseXmlReader.NodeType == XmlNodeType.EndElement && BaseXmlReader.LocalName == tag.LocalName && BaseXmlReader.NamespaceURI == tag.Namespace;
  }

  /// <summary>
  /// Wrapper for ReadEndElement operation.
  /// Checks that the current content node is an end tag and advances the reader to the next node.
  /// </summary>
  public void ReadEndElement()
  {
    BaseXmlReader.ReadEndElement();
  }

  /// <summary>
  /// Wrapper for ReadEndElement operation.
  /// Checks that the current content node is an end tag with a specified name and advances the reader to the next node.
  /// </summary>
  /// <exception cref="XmlInvalidOperationException">Thrown if node is not end element or name does not match</exception>
  public void ReadEndElement(string name)
  {
    var ok = BaseXmlReader.NodeType == XmlNodeType.EndElement && BaseXmlReader.Name == name;
    if (!ok)
      throw new XmlInvalidOperationException($"End element \"{name}\" expected but {BaseXmlReader.NodeType} \"{BaseXmlReader.Name}\" found", this);
    BaseXmlReader.ReadEndElement();
  }

  /// <summary>
  /// Wrapper for ReadEndElement operation.
  /// Checks that the current content node is an end tag with a specified name and namespaceURI and advances the reader to the next node.
  /// </summary>
  /// <exception cref="XmlInvalidOperationException">Thrown if node is not end element or name and namespaceURI does not match</exception>
  public void ReadEndElement(XmlQualifiedTagName tag)
  {
    var ok = BaseXmlReader.NodeType == XmlNodeType.EndElement && BaseXmlReader.LocalName == tag.LocalName && BaseXmlReader.NamespaceURI == tag.Namespace;
    if (!ok)
      throw new XmlInvalidOperationException($"End element \"{tag}\" expected but {BaseXmlReader.NodeType} \"{BaseXmlReader.Name}\" found", this);
    BaseXmlReader.ReadEndElement();
  }
  #endregion

  #region closing methods
  /// <summary>
  /// Wrapper for Close operation.
  /// </summary>
  public void Close()
  {
    BaseXmlReader.Close();
  }

  /// <summary>
  /// Wrapper for Dispose operation.
  /// </summary>
  public void Dispose()
  {
    BaseXmlReader.Dispose();
  }
  #endregion
}
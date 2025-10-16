using System.Xml.Linq;

namespace Qhta.Xml.Serialization;

/// <summary>
/// Wrapper for system XmlReader used by QXmlSerializer.
/// </summary>
public partial class QXDocReader : IXmlReader, IDisposable
{
  /// <summary>
  /// Initializing constructor
  /// </summary>
  public QXDocReader(XmlReader xmlReader)
  {
    BaseXmlReader = xmlReader;
    Document = XDocument.Load(xmlReader);
    CurrentNode = Document.Root;
  }

  /// <summary>
  /// Base XmlReader reference.
  /// </summary>
  public XmlReader BaseXmlReader { get; }

  /// <summary>
  /// Loaded XDocument;
  /// </summary>
  public XDocument Document { get; }

  /// <summary>
  /// Current node of the loaded XDocument. See also <see cref="ParsedValue"/>
  /// </summary>
  public object? CurrentNode
  {
    get;
    private set;
  }

  /// <summary>
  /// Class "XEndElement" does not exist in System.Xml.Linq and XElement does not have type of XmlNodeType.EndElement
  /// </summary>
  class XEndElement
  {
    public XEndElement(XElement startElement)
    {
      StartElement = startElement;
    }

    private XElement StartElement;

    public XmlNodeType NodeType => XmlNodeType.EndElement;

    public XName Name => StartElement.Name;

    public XElement? Parent => StartElement.Parent;

    public XNode? NextNode => StartElement.NextNode;

    public string BaseUri => StartElement.BaseUri;

    public string Namespace => StartElement.Name.NamespaceName;

    public override string ToString()
    {
      return $"</{StartElement.Name}>";
    }
  }

  /// <summary>
  /// Parsed attribute of the loaded XDocument
  /// </summary>
  public XAttribute? ParsedAttribute { get; private set; }

  /// <summary>
  /// Parsed value of the <see cref="ParsedAttribute"/> of the loaded XDocument
  /// </summary>
  public object? ParsedValue { get; private set; }

  /// <summary>
  /// Get wrapped XML reader.
  /// </summary>
  public static implicit operator XmlReader(QXDocReader reader) => reader.BaseXmlReader;

  #region Reader state

  /// <summary>
  /// Getting LineNumber and LinePosition of the reader
  /// </summary>
  public (int line, int pos) GetPosition() => (0, 0);


  /// <summary>
  /// Limit divides text to separate lines.
  /// </summary>
  public int LineLengthLimit { get; set; } = 0;

  /// <summary>
  /// Wrapped EOF property.
  /// </summary>
  public bool EOF => CurrentNode == null;

  /// <summary>
  /// Wrapped ReadState property.
  /// </summary>
  public ReadState ReadState => (CurrentNode != null) ? ReadState.Interactive : ReadState.EndOfFile;

  /// <summary>
  /// Wrapped NodeType property
  /// </summary>
  public XmlNodeType NodeType => (CurrentNode is XObject xObject) ? xObject.NodeType
    : (CurrentNode is XEndElement xEndElement) ? xEndElement.NodeType : XmlNodeType.None;

  /// <summary>
  /// Qualified name (local name and namespace URI)
  /// </summary>
  public XmlQualifiedTagName Name
  {
    get
    {
      if (CurrentNode == null)
        return new XmlQualifiedTagName("", null);
      string localName;
      if (CurrentNode is XElement xElement)
      {
        localName = xElement.Name.LocalName;
        var uri = xElement.Name.NamespaceName;
        if (!String.IsNullOrEmpty(uri))
        {
          if (uri.StartsWith("System."))
            return new XmlQualifiedTagName(localName, "System");
          return new XmlQualifiedTagName(localName, uri);
        }
        return new XmlQualifiedTagName(localName);
      }
      else
      if (CurrentNode is XEndElement xEndElement)
      {
        localName = xEndElement.Name.LocalName;
        var uri = xEndElement.Name.NamespaceName;
        if (!String.IsNullOrEmpty(uri))
        {
          if (uri.StartsWith("System."))
            return new XmlQualifiedTagName(localName, "System");
          return new XmlQualifiedTagName(localName, uri);
        }
        return new XmlQualifiedTagName(localName);
      }
      else
      if (CurrentNode is XAttribute xAttribute)
      {
        localName = xAttribute.Name.LocalName;
        var uri = xAttribute.Name.NamespaceName;
        if (!String.IsNullOrEmpty(uri))
          return new XmlQualifiedTagName(localName, uri);
        return new XmlQualifiedTagName(localName);
      }
      else
      if (CurrentNode is XText xText)
      {
        localName = "#Text";
        return new XmlQualifiedTagName(localName);
      }
      else
        throw new InvalidOperationException($"XNode of type {CurrentNode.GetType()} does not have a name");
    }
  }

  /// <summary>
  /// Wrapper for LocalName property.
  /// </summary>
  public string LocalName
  {
    get
    {
      if (CurrentNode == null)
        return null!;
      if (CurrentNode is XElement xElement)
        return xElement.Name.LocalName;
      if (CurrentNode is XAttribute xAttribute)
        return xAttribute.Name.LocalName;
      if (CurrentNode is XText xText)
        return "#Text";
      throw new InvalidOperationException($"XNode of type {CurrentNode.GetType()} does not have a name");
    }
  }

  /// <summary>
  /// Wrapper for NamespaceURI property.
  /// </summary>
  public string NamespaceURI
  {
    get
    {
      if (CurrentNode == null)
        return null!;
      if (CurrentNode is XElement xElement)
        return xElement.Name.NamespaceName;
      else
      if (CurrentNode is XAttribute xAttribute)
        return xAttribute.Name.NamespaceName;
      else
      if (CurrentNode is XText xText)
        return null!;
      else
        throw new InvalidOperationException($"XNode of type {CurrentNode.GetType()} does not have a namespace");
    }
  }

  /// <summary>
  /// Wrapper for Prefix property.
  /// </summary>
  public string Prefix
  {
    get
    {
      if (CurrentNode == null)
        return null!;
      if (CurrentNode is XElement xElement)
        return xElement.GetPrefixOfNamespace(xElement.Name.NamespaceName) ?? "";
      else
      if (CurrentNode is XAttribute xAttribute)
      {
        var text = xAttribute.ToString();
        var k = text.IndexOfAny(new char[] { ':', '=' });
        if (k != -1)
        {
          if (text[k] == ':')
            return text.Substring(0, k);
        }
        return string.Empty;
      }
      else
      if (CurrentNode is XText xText)
        return null!;
      else
        throw new InvalidOperationException($"XNode of type {CurrentNode.GetType()} does not have a namespace");
    }
  }

  /// <summary>
  /// Wrapper for HasValue property.
  /// </summary>
  public bool HasValue
  {
    get
    {
      if (CurrentNode == null)
        return false;
      if (CurrentNode is XElement xElement)
      {
        return xElement.Value != null;
      }
      else if (CurrentNode is XAttribute xAttribute)
      {
        return xAttribute.Value != null;
      }
      else if (CurrentNode is XText xText)
      {
        return xText.Value != null;
      }
      else
        throw new InvalidOperationException($"XNode of type {CurrentNode.GetType()} cannot have any value");
    }
  }

  /// <summary>
  /// Wrapper for ValueType property.
  /// </summary>
  public Type ValueType => typeof(string);

  /// <summary>
  /// Wrapper for Value property.
  /// </summary>
  public string Value
  {
    get
    {
      if (CurrentNode == null)
        return null!;
      if (CurrentNode is XElement xElement)
      {
        return xElement.Value;
      }
      else if (CurrentNode is XAttribute xAttribute)
      {
        return xAttribute.Value;
      }
      else if (CurrentNode is XText xText)
      {
        return xText.Value;
      }
      else
        throw new InvalidOperationException($"XNode of type {CurrentNode.GetType()} cannot have any value");
    }
  }

  /// <summary>
  /// Wrapper for Depth property.
  /// </summary>
  public int Depth
  {
    get
    {
      if (CurrentNode == null)
        return 0;
      if (CurrentNode is XElement xElement)
      {
        var depth = 0;
        while (xElement != null)
        {
          depth++;
          xElement = xElement.Parent!;
        }
        return depth;
      }
      else if (CurrentNode is XAttribute xAttribute)
      {
        xElement = xAttribute.Parent!;
        var depth = 1;
        while (xElement != null)
        {
          depth++;
          xElement = xElement.Parent!;
        }
        return depth;
      }
      else if (CurrentNode is XText xText)
      {
        xElement = xText.Parent!;
        var depth = 1;
        while (xElement != null)
        {
          depth++;
          xElement = xElement.Parent!;
        }
        return depth;
      }
      else
        throw new InvalidOperationException($"XNode of type {CurrentNode.GetType()} cannot have any depth");
    }
  }


  /// <summary>
  /// Wrapper for BaseURI property.
  /// </summary>
  public string BaseURI => NamespaceURI;

  /// <summary>
  /// Wrapper for IsEmptyElement property.
  /// </summary>
  public bool IsEmptyElement
  {
    get
    {
      if (CurrentNode is XElement xElement)
        return xElement.IsEmpty;
      return false;
    }
  }

  /// <summary>
  /// Wrapper for IsDefault property.
  /// </summary>
  public bool IsDefault =>
    throw new System.NotImplementedException();

  /// <summary>
  /// Wrapper for QuoteChar property.
  /// </summary>
  public char QuoteChar =>
    throw new System.NotImplementedException();

  /// <summary>
  /// Wrapper for XmlSpace property.
  /// </summary>
  public XmlSpace XmlSpace =>
    throw new System.NotImplementedException();

  /// <summary>
  /// Wrapper for XmlLang property.
  /// </summary>
  public string XmlLang =>
    throw new System.NotImplementedException();

  /// <summary>
  /// Gets line number of XML text file where the exception occured.
  /// </summary>
  public int? LineNumber
  {
    get
    {
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
      return null;
    }
  }

  #endregion

  #region Settings

  /// <summary>
  /// Wrapper for Settings property.
  /// </summary>
  public XmlReaderSettings? Settings =>
    BaseXmlReader.Settings;

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
  public bool HasAttributes
  {
    get
    {
      if (CurrentNode is XElement xElement)
      {
        return xElement.HasAttributes;
      }
      if (CurrentNode != null)
        throw new InvalidOperationException($"Can't get HasAttributes from node of {CurrentNode.GetType()} type");
      else
        throw new InvalidOperationException($"Can't get HasAttributes from empty node");
    }
  }

  /// <summary>
  /// Wrapper for AttributeCount property.
  /// Gets the number of attributes on the current node.
  /// </summary>
  public int AttributeCount
  {
    get
    {
      if (CurrentNode is XElement xElement)
        return xElement.Attributes().Count();
      if (CurrentNode is XAttribute xAttribute)
      {
        return xAttribute.Parent?.Attributes().Count() ?? 0;
      }
      if (CurrentNode != null)
        throw new InvalidOperationException($"Can't get AttributeCount from node of {CurrentNode.GetType()} type");
      else
        throw new InvalidOperationException($"Can't get AttributeCount from empty node");
    }
  }

  ///// <summary>
  ///// Wrapper for int indexed item accessors.
  ///// Gets the value of the attribute with the specified index.
  ///// </summary>
  //public string this[int i] => BaseXmlReader[i];

  ///// <summary>
  ///// Wrapper for local name indexed item accessors.
  ///// Gets the value of the attribute with the specified Name.
  ///// </summary>
  //public string? this[string name] => BaseXmlReader[name];

  ///// <summary>
  ///// Wrapper for tag name indexed item accessors.
  ///// Gets the value of the attribute with the specified LocalName and NamespaceURI.
  ///// </summary>
  //public string? this[XmlQualifiedTagName fullName]
  //  => BaseXmlReader[fullName.Name, fullName.Namespace];


  /// <summary>
  /// Wrapper for GetAttribute operation.
  /// Gets the value of the attribute with the specified Name.
  /// </summary>
  public string? GetAttribute(string name)
  {
    if (CurrentNode is XElement xElement)
    {
      return xElement.Attributes().FirstOrDefault(item => item.Name == name)?.Value;
    }
    if (CurrentNode is XAttribute xAttribute)
    {
      if (xAttribute.Name == name)
        return xAttribute.Value;
      CurrentNode = xAttribute.Parent;
      return GetAttribute(name);
    }
    if (CurrentNode != null)
      throw new InvalidOperationException($"Can't get Attribute from node of {CurrentNode.GetType()} type");
    else
      throw new InvalidOperationException($"Can't get Attribute from empty node");
  }

  /// <summary>
  /// Wrapper for GetAttribute operation.
  /// Gets the value of the attribute with the specified LocalName and NamespaceURI.
  /// </summary>
  public string? GetAttribute(XmlQualifiedTagName fullName)
  {
    if (CurrentNode is XElement xElement)
    {
      return xElement.Attributes().FirstOrDefault(item => item.Name == fullName.Name && item.BaseUri == fullName.Namespace)?.Value;
    }
    if (CurrentNode is XAttribute xAttribute)
    {
      if (xAttribute.Name == fullName.Name && xAttribute.BaseUri == fullName.Namespace)
        return xAttribute.Value;
      CurrentNode = xAttribute.Parent;
      return GetAttribute(fullName);
    }
    if (CurrentNode != null)
      throw new InvalidOperationException($"Can't get Attribute from node of {CurrentNode.GetType()} type");
    else
      throw new InvalidOperationException($"Can't get Attribute from empty node");
  }

  /// <summary>
  /// Wrapper for GetAttribute operation.
  /// Gets the value of the attribute with the specified index.
  /// </summary>
  public string GetAttribute(int i)
  {
    if (CurrentNode is XElement xElement)
    {
      return xElement.Attributes().ToArray()[i].Value;
    }
    if (CurrentNode is XAttribute xAttribute)
    {
      CurrentNode = xAttribute.Parent;
      return GetAttribute(i);
    }
    if (CurrentNode != null)
      throw new InvalidOperationException($"Can't get Attribute from node of {CurrentNode.GetType()} type");
    else
      throw new InvalidOperationException($"Can't get Attribute from empty node");
  }


  /// <summary>
  /// Wrapper for ReadAttributeValue operation.
  /// Parses the attribute value into one or more Text, EntityReference, or EndEntity nodes.
  /// </summary>
  /// <returns>True if there are nodes to return. False otherwise.</returns>
  public bool ReadAttributeValue()
  {
    if (CurrentNode is XAttribute xAttribute)
    {
      var str = xAttribute.Value;
      if (int.TryParse(str, out int n))
        ParsedValue = n;
      else
        ParsedValue = str;
      ParsedAttribute = xAttribute;
      return true;
    }
    if (CurrentNode != null)
      throw new InvalidOperationException($"Can't get AttributeValue from node of {CurrentNode.GetType()} type");
    else
      throw new InvalidOperationException($"Can't get AttributeValue from empty node");
  }

  #endregion

  #region Content accessors

  /// <summary>
  /// Gets a string from element content or gets content as string.
  /// </summary>
  public string ReadString()
  {
    if (CurrentNode is XElement xElement)
      return ReadElementContentAsString();
    else
    if (CurrentNode is XAttribute xAttribute)
      return xAttribute.Value;
    else
    if (CurrentNode is XText xText)
      return xText.Value;
    else
    if (CurrentNode is XEndElement)
      return String.Empty;
    else
    if (CurrentNode != null)
      throw new InvalidOperationException($"Can't ReadString from node of {CurrentNode.GetType()} type");
    else
      throw new InvalidOperationException($"Can't ReadString from empty node");
  }

  /// <summary>
  ///  Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references,
  ///  and converts the content to the requested type. Stops at start tags and end tags.
  /// </summary>
  public object ReadContentAs(Type returnType)
  {
    throw new NotImplementedException();
  }

  ///// <summary>
  ///// Wrapper for ReadContentAs operation.
  ///// Reads the content as an object of the type specified.
  ///// </summary>
  //public object ReadContentAs(Type returnType)
  //{
  //  return BaseXmlReader.ReadContentAs(returnType, null);
  //}

  /// <summary>
  /// Wrapper for ReadElementContentAsString operation.
  /// Reads the current element and returns the contents as a String object.
  /// </summary>
  public string ReadElementContentAsString()
  {
    if (CurrentNode is XElement xElement)
      return xElement.Value;
    return String.Empty;
  }

  ///// <summary>
  ///// Wrapper for ReadElementContentAsString operation.
  ///// Checks that the specified local name and namespace URI matches that of the current element, 
  ///// then reads the current element and returns the contents as a String object.
  ///// </summary>
  //public string ReadElementContentAsString(XmlQualifiedTagName fullName)
  //{
  //  return BaseXmlReader.ReadElementContentAsString(fullName.Name, fullName.Namespace);
  //}
  #endregion

  #region Element accessors
  ///// <summary>
  ///// Wrapper for ReadElementString operation.
  ///// Reads a text-only element.
  ///// </summary>
  //public string ReadElementString()
  //{
  //  return BaseXmlReader.ReadElementString();
  //}

  ///// <summary>
  ///// Wrapper for ReadElementString operation. 
  ///// However, we recommend that you use the ReadElementContentAsString() method instead, 
  ///// because it provides a more straightforward way to handle this operation.
  ///// </summary>
  //public string ReadElementString(string name)
  //{
  //  return BaseXmlReader.ReadElementString(name);
  //}

  ///// <summary>
  ///// Wrapper for ReadElementString operation.
  ///// Checks that the Name property of the element found matches the given string before reading a text-only element. 
  ///// However, we recommend that you use the ReadElementContentAsString() method instead, because it provides a more straightforward way to handle this operation.
  ///// </summary>
  //public string ReadElementString(XmlQualifiedTagName fullName)
  //{
  //  return BaseXmlReader.ReadElementString(fullName.Name, fullName.Namespace);
  //}
  #endregion

  #region Movement

  private static object? NextNode(XElement xElement)
  {
    var aNode = xElement.DescendantNodes().FirstOrDefault();
    if (aNode != null)
      return aNode;
    if (!xElement.IsEmpty)
      return new XEndElement(xElement);
    aNode = xElement.NextNode;
    if (aNode != null)
      return aNode;
    xElement = xElement.Parent!;
    if (xElement != null)
    {
      if (xElement.IsEmpty)
        return NextNode(xElement);
      return new XEndElement(xElement);
    }
    return null;
  }

  private static object? NextNode(XAttribute xAttribute)
  {
    XObject? aNode = xAttribute.NextAttribute;
    if (aNode != null)
      return aNode;
    var xElement = xAttribute.Parent!;
    if (xElement != null)
    {
      aNode = xElement.DescendantNodes().FirstOrDefault();
      if (aNode != null)
        return aNode;

      if (xElement.IsEmpty)
        return NextNode(xElement);
      return new XEndElement(xElement);
    }
    return null;
  }

  private static object? NextNode(XText xText)
  {
    var aNode = xText.NextNode;
    if (aNode != null)
      return aNode;
    var xElement = xText.Parent!;
    if (xElement != null)
    {
      if (xElement.IsEmpty)
        return NextNode(xElement);
      return new XEndElement(xElement);
    }
    return null;
  }

  private static object? NextNode(XEndElement xEndElement)
  {
    if (xEndElement.Name.LocalName == "CompatibilitySettings")
      Debug.Assert(true);
    var aNode = xEndElement.NextNode;
    if (aNode != null)
      return aNode;
    var xElement = xEndElement.Parent!;
    if (xElement != null)
    {
      if (xElement.IsEmpty)
        return NextNode(xElement);
      return new XEndElement(xElement);
    }
    return null;
  }

  ///// <summary>
  ///// Wrapper for MoveToAttribute operation.
  ///// Moves to the attribute with the specified Name.
  ///// </summary>
  //public bool MoveToAttribute(string name)
  //{
  //  return BaseXmlReader.MoveToAttribute(name);
  //}

  ///// <summary>
  ///// Wrapper for MoveToAttribute operation.
  ///// Moves to the attribute with the specified LocalName and NamespaceURI.
  ///// </summary>
  //public bool MoveToAttribute(XmlQualifiedTagName fullName)
  //{
  //  return BaseXmlReader.MoveToAttribute(fullName.Name, fullName.Namespace);
  //}

  ///// <summary>
  ///// Wrapper for MoveToAttribute operation.
  ///// Moves to the attribute with the specified index.
  ///// </summary>
  //public void MoveToAttribute(int i)
  //{
  //  BaseXmlReader.MoveToAttribute(i);
  //}

  /// <summary>
  /// Wrapper for MoveToFirstAttribute operation.
  /// Moves to the first attribute.
  /// </summary>
  public bool MoveToFirstAttribute()
  {
    if (CurrentNode is XElement xElement)
    {
      var attrib = xElement.FirstAttribute;
      if (attrib != null)
      {
        CurrentNode = attrib;
        return true;
      }
    }
    return false;
  }

  /// <summary>
  /// Wrapper for MoveToNextAttribute operation.
  /// Moves to the next attribute.
  /// </summary>
  public bool MoveToNextAttribute()
  {
    if (CurrentNode is XAttribute xAttribute)
    {
      var attrib = xAttribute.NextAttribute;
      if (attrib != null)
        CurrentNode = attrib;
      else
        CurrentNode = xAttribute.Parent;
      return true;
    }
    return false;
  }

  /// <summary>
  /// Wrapper for MoveToElement operation.
  /// Moves to the element that contains the current attribute node.
  /// </summary>
  public bool MoveToElement()
  {
    if (CurrentNode is XAttribute xAttribute)
    {
      CurrentNode = xAttribute.Parent;
    }
    return false;
  }

  ///// <summary>
  ///// Wrapper for MoveToContent operation.
  ///// Checks whether the current node is a content (non-white space text, CDATA, Element, EndElement, EntityReference, or EndEntity) node. 
  ///// If the node is not a content node, the reader skips ahead to the next content node or end of file. 
  ///// It skips over nodes of the following type: ProcessingInstruction, DocumentType, Comment, Whitespace, or SignificantWhitespace.
  ///// </summary>
  //public XmlNodeType MoveToContent()
  //{
  //  return BaseXmlReader.MoveToContent();
  //}

  /// <summary>
  /// Wrapper for Skip operation.
  /// Skips the children of the current node.
  /// </summary>
  public void Skip()
  {
    if (CurrentNode is XElement xElement)
      CurrentNode = NextNode(xElement);
    else
    if (CurrentNode is XAttribute xAttribute)
      CurrentNode = NextNode(xAttribute);
    else
    if (CurrentNode is XText xText)
      CurrentNode = NextNode(xText);
    else
    if (CurrentNode is XEndElement xEndElement)
      CurrentNode = NextNode(xEndElement);
    else
    if (CurrentNode != null)
      throw new InvalidOperationException($"Can't Skip on node of {CurrentNode.GetType()} type");
    else
      throw new InvalidOperationException($"Can't Skip on empty node");
  }
  #endregion

  #region Read operations

  /// <summary>
  /// Wrapper for Read operation.
  /// Reads the next node from the stream.
  /// </summary>
  public void Read()
  {
    if (CurrentNode is XElement xElement)
      CurrentNode = NextNode(xElement);
    else
    if (CurrentNode is XAttribute xAttribute)
      CurrentNode = NextNode(xAttribute);
    else
    if (CurrentNode is XText xText)
      CurrentNode = NextNode(xText);
    else
    if (CurrentNode is XEndElement xEndElement)
      CurrentNode = NextNode(xEndElement);
    else
    if (CurrentNode != null)
      throw new InvalidOperationException($"Can't Read on node of {CurrentNode.GetType()} type");
    else
      throw new InvalidOperationException($"Can't Read on empty node");
  }

  ///// <summary>
  ///// Returns a new readed instance that can be used to read the current node, and all its descendants.
  ///// </summary>
  ///// <returns>new QXmlReader</returns>
  ///// 
  //public QXmlReader ReadSubtree()
  //{
  //  return new QXmlReader(BaseXmlReader.ReadSubtree());
  //}
  #endregion

  #region start element checking and read

  /// <summary>
  /// Wrapper for IsStartElement operation.
  /// Calls MoveToContent() and tests if the current content node is a start tag or empty element tag.
  /// </summary>
  public bool IsStartElement()
  {
    return CurrentNode is XElement xElement && xElement.NodeType == XmlNodeType.Element;
  }

  /// <summary>
  /// Wrapper for IsStartElement operation.
  /// Calls MoveToContent() and tests if the current content node is a start tag or empty element tag 
  /// and if the Name property of the element found matches the given argument.
  /// </summary>
  public bool IsStartElement(string name)
  {
    return CurrentNode is XElement xElement && xElement.NodeType == XmlNodeType.Element
      && xElement.Name.LocalName == name;
  }

  /// <summary>
  /// Wrapper for IsStartElement operation.
  /// Calls MoveToContent() and tests if the current content node is a start tag or empty element tag 
  /// and if the LocalName and NamespaceURI properties of the element found match the given strings.
  /// </summary>
  public bool IsStartElement(XmlQualifiedTagName tag)
  {
    return CurrentNode is XElement xElement && xElement.NodeType == XmlNodeType.Element
      && xElement.Name.LocalName == tag.Name && xElement.BaseUri == tag.Namespace;
  }

  /// <summary>
  /// Wrapper for ReadStartElement operation.
  /// Checks that the current node is an element and advances the reader to the next node.
  /// </summary>
  public void ReadStartElement()
  {
    if (CurrentNode is XElement xElement && xElement.NodeType == XmlNodeType.Element)
    {
      var aNode = xElement.DescendantNodes().FirstOrDefault();
      if (aNode != null)
        CurrentNode = aNode;
      else
        CurrentNode = xElement.NextNode;
    }
    else
      throw new XmlInvalidOperationException($"StartElement expected but {CurrentNode} found", this);
  }

  /// <summary>
  /// Wrapper for ReadStartElement operation.
  /// Checks that the current content node is an element with the given Name and advances the reader to the next node.
  /// </summary>
  public void ReadStartElement(string name)
  {
    if (CurrentNode is XElement xElement && xElement.NodeType == XmlNodeType.Element)
    {
      if (xElement.Name.LocalName == name)
      {
        var aNode = xElement.DescendantNodes().FirstOrDefault();
        if (aNode != null)
          CurrentNode = aNode;
        else
          CurrentNode = xElement.NextNode;
      }
      else
        throw new XmlInvalidOperationException($"StartElement of \"{name}\" expected but {xElement.Name.LocalName} found", this);
    }
    else
      throw new XmlInvalidOperationException($"StartElement expected but {CurrentNode} found", this);
  }

  /// <summary>
  /// Wrapper for ReadStartElement operation.
  /// Checks that the current content node is an element with the given LocalName 
  /// and NamespaceURI and advances the reader to the next node.
  /// </summary>
  public void ReadStartElement(XmlQualifiedTagName tag)
  {
    if (CurrentNode is XElement xElement && xElement.NodeType == XmlNodeType.Element)
    {
      if (xElement.Name.LocalName == tag.Name && (xElement.BaseUri == tag.Namespace || xElement.BaseUri == ""))
      {
        var aNode = xElement.DescendantNodes().FirstOrDefault();
        if (aNode != null)
          CurrentNode = aNode;
        else
          CurrentNode = xElement.NextNode;
      }
      else
        throw new XmlInvalidOperationException($"StartElement of \"{tag}\" expected but {xElement.Name} found", this);
    }
    else
      throw new XmlInvalidOperationException($"StartElement expected but {CurrentNode} found", this);
  }
  #endregion

  #region end element checking and read
  /// <summary>
  /// Checks if reader node type is EndElement.
  /// </summary>
  public bool IsEndElement()
  {
    return CurrentNode is XEndElement;
  }

  /// <summary>
  /// Checks if reader node type is EndElement and reade name is a specified string
  /// </summary>
  public bool IsEndElement(string name)
  {
    return CurrentNode is XEndElement xElement
      && xElement.Name.LocalName == name;
  }

  /// <summary>
  /// Checks if reader node type is EndElement and reade name is a specified local name and namespaceURI is a specified namespace.
  /// </summary>
  public bool IsEndElement(XmlQualifiedTagName tag)
  {
    return CurrentNode is XEndElement xElement
      && xElement.Name.LocalName == tag.Name && xElement.BaseUri == tag.Namespace;
  }

  /// <summary>
  /// Wrapper for ReadEndElement operation.
  /// Checks that the current content node is an end tag and advances the reader to the next node.
  /// </summary>
  public void ReadEndElement()
  {
    if (CurrentNode is XEndElement xElement)
      CurrentNode = NextNode(xElement);
    else
    if (CurrentNode != null)
      throw new XmlInvalidOperationException($"EndElement expected but {CurrentNode} found", this);
  }

  /// <summary>
  /// Wrapper for ReadEndElement operation.
  /// Checks that the current content node is an end tag with a specified name and advances the reader to the next node.
  /// </summary>
  /// <exception cref="XmlInvalidOperationException">Thrown if node is not end element or name does not match</exception>
  public void ReadEndElement(string name)
  {
    if (CurrentNode is XEndElement xElement)
    {
      if (xElement.Name.LocalName == name)
        CurrentNode = NextNode(xElement);
      else
        throw new XmlInvalidOperationException($"EndElement of \"{name}\" expected but {xElement.Name.LocalName} found", this);
    }
    else
    if (CurrentNode != null)
      throw new XmlInvalidOperationException($"EndElement expected but {CurrentNode} found", this);
  }

  /// <summary>
  /// Wrapper for ReadEndElement operation.
  /// Checks that the current content node is an end tag with a specified name and namespaceURI and advances the reader to the next node.
  /// </summary>
  /// <exception cref="XmlInvalidOperationException">Thrown if node is not end element or name and namespaceURI does not match</exception>
  public void ReadEndElement(XmlQualifiedTagName tag)
  {
    if (CurrentNode is XEndElement xElement)
    {
      if (xElement.Name.LocalName == tag.Name && GetNamespace(xElement) == tag.Namespace)
        CurrentNode = NextNode(xElement);
      else
        throw new XmlInvalidOperationException($"EndElement of \"{tag}\" expected but {xElement.Name} found", this);
    }
    else
    if (CurrentNode != null)
      throw new XmlInvalidOperationException($"EndElement expected but {CurrentNode.GetType()} {CurrentNode} found", this);

  }
  #endregion

  private string GetNamespace(XEndElement xElement)
  {
    var result = xElement.Namespace;
    if (result.StartsWith("System."))
      return "System";
    return result;
  }
  #region closing methods
  /// <summary>
  /// Wrapper for Close operation.
  /// </summary>
  public void Close()
  {
  }

  /// <summary>
  /// Wrapper for Dispose operation.
  /// </summary>
  public void Dispose()
  {
  }
  #endregion
}
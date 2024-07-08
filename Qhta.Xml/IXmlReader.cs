namespace Qhta.Xml;

/// <summary>
/// Defines methods that must be implemented in an XmlReader.
/// </summary>
public interface IXmlReader
{
  /////<summary>
  ///// IDisposable interface
  /////</summary>
  //public void Dispose();

  /////<summary>
  ///// Closes the stream/TextReader (if CloseInput==true), changes the ReadState to Closed, and sets all the properties back to zero/empty string.
  /////</summary>
  //public void Close();

  /// <summary>
  /// Needed to use IXmlSerializable interface.
  /// </summary>
  public XmlReader? BaseXmlReader { get; }

  //#region Settings
  /////<summary>
  ///// Predefined reader settings
  /////</summary>
  //public XmlReaderSettings? Settings { get; }

  /// <summary>
  /// Gets or sets the whitespace handling.
  /// Can return all Whitespace and SignificantWhitespace nodes,
  /// or just SignificantWhitespace, i.e. whitespace nodes that are in scope of xml:space="preserve",
  /// no Whitespace at all.
  /// </summary>
  public WhitespaceHandling? WhitespaceHandling { get; set; }
  //#endregion

  #region Reader state
  ///<summary>
  /// Returns true when the XmlReader is positioned at the end of the stream.
  ///</summary>
  public bool EOF { get; }

  /////<summary>
  ///// Returns the read state of the XmlReader.
  /////</summary>
  //public ReadState ReadState { get; }

  ///<summary>
  /// Get the type of the current node.
  ///</summary>
  public XmlNodeType NodeType { get; }

  ///<summary>
  /// Gets the name of the current node, including the namespace prefix.
  ///</summary>
  public XmlQualifiedTagName Name { get; }

  ///<summary>
  /// Gets the name of the current node without the namespace prefix.
  ///</summary>
  public string LocalName { get; }

  /////<summary>
  ///// Gets the namespace URN (as defined in the W3C Namespace Specification) of the current namespace scope.
  /////</summary>
  //public string NamespaceURI { get; }

  ///<summary>
  /// Gets the namespace prefix associated with the current node.
  ///</summary>
  public string Prefix { get; }

  /////<summary>
  ///// Gets a value indicating whether the current node can have a Value.
  /////</summary>
  //public bool HasValue { get; }

  ///<summary>
  /// returns the type of the current node
  ///</summary>
  public Type ValueType { get; }

  ///<summary>
  /// Gets the text value of the current node.
  ///</summary>
  public string Value { get; }

  /////<summary>
  ///// Gets the depth of the current node in the XML element stack.
  /////</summary>
  //public int Depth { get; }

  /////<summary>
  ///// Gets the base URI of the current node.
  /////</summary>
  //public string BaseURI { get; }

  ///<summary>
  /// Gets a value indicating whether the current node is an empty element (for example, <MyElement/>).
  ///</summary>
  public bool IsEmptyElement { get; }

  /////<summary>
  ///// Gets a value indicating whether the current node is an attribute that was generated from the default value defined in the DTD or schema.
  /////</summary>
  //public bool IsDefault { get; }

  /////<summary>
  ///// Gets the quotation mark character used to enclose the value of an attribute node.
  /////</summary>
  //public char QuoteChar { get; }

  /////<summary>
  ///// Gets the current xml:space scope.
  /////</summary>
  //public XmlSpace XmlSpace { get; }

  /////<summary>
  ///// Gets the current xml:lang scope.
  /////</summary>
  //public string XmlLang { get; }

  /// <summary>
  /// Gets line number of XML text file where the exception occured.
  /// </summary>
  public int? LineNumber { get; }

  /// <summary>
  /// Gets the position in line of XML text file where the exception occured.
  /// </summary>
  public int? LinePosition { get; }
  #endregion

  #region Attribute accessors

  ///<summary>
  /// Returns true when the current node has any attributes.
  ///</summary>
  public bool HasAttributes { get; }

  ///<summary>
  /// The number of attributes on the current node.
  ///</summary>
  public int AttributeCount { get; }

  /////<summary>
  ///// Gets the value of the attribute with the specified index.
  /////</summary>
  //public string this[int i] { get; }

  /////<summary>
  ///// Gets the value of the attribute with the specified Name.
  /////</summary>
  //public string? this[string name] { get; }

  /////<summary>
  ///// Gets the value of the attribute with the LocalName and NamespaceURI
  /////</summary>
  //public string? this[XmlQualifiedTagName fullName] { get; }

  ///<summary>
  /// Gets the value of the attribute with the specified Name
  ///</summary>
  public string? GetAttribute(string name);

  ///<summary>
  /// Gets the value of the attribute with the LocalName and NamespaceURI
  ///</summary>
  public string? GetAttribute(XmlQualifiedTagName fullName);

  ///<summary>
  /// Gets the value of the attribute with the specified index.
  ///</summary>
  public string GetAttribute(int i);

  #endregion

  #region Content accessors
  ///<summary>
  /// Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, and returns the content as a string. Stops at start tags and end tags.
  ///</summary>
  public string ReadString();

  ///<summary>
  /// Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references,
  /// and converts the content to the requested type. Stops at start tags and end tags.
  ///</summary>
  public object ReadContentAs(Type returnType);

  ///<summary>
  /// Returns the content of the current element as a string. Moves to the node following the element's end tag.
  ///</summary>
  public string ReadElementContentAsString();

  /////<summary>
  ///// Checks local name and namespace of the current element and returns its content as a string. Moves to the node following the element's end tag.
  /////</summary>
  //public string ReadElementContentAsString(XmlQualifiedTagName fullName);

  #endregion

  //#region Element accessors
  /////<summary>
  ///// Reads a text-only element.
  /////</summary>
  //public string ReadElementString();

  /////<summary>
  ///// Checks that the Name property of the element found matches the given string before reading a text-only element.
  /////</summary>
  //public string ReadElementString(string name);

  /////<summary>
  ///// Checks that the LocalName and NamespaceURI properties of the element found matches the given strings before reading a text-only element.
  /////</summary>
  //public string ReadElementString(XmlQualifiedTagName fullName);
  //#endregion

  //#region Movement
  /////<summary>
  ///// Moves to the attribute with the specified Name.
  /////</summary>
  //public bool MoveToAttribute(string name);

  /////<summary>
  ///// Moves to the attribute with the specified LocalName and NamespaceURI.
  /////</summary>
  //public bool MoveToAttribute(XmlQualifiedTagName fullName);

  /////<summary>
  ///// Moves to the attribute with the specified index.
  /////</summary>
  //public void MoveToAttribute(int i);

  ///<summary>
  /// Moves to the first attribute of the current node.
  ///</summary>
  public bool MoveToFirstAttribute();

  ///<summary>
  /// Moves to the next attribute.
  ///</summary>
  public bool MoveToNextAttribute();

  /////<summary>
  ///// Moves to the element that contains the current attribute node.
  /////</summary>
  //public bool MoveToElement();

  /////<summary>
  ///// Checks whether the current node is a content (non-whitespace text, CDATA, Element, EndElement, EntityReference or EndEntity) node.
  ///// If the node is not a content node, then the method skips ahead to the next content node or end of file.
  ///// Skips over nodes of type ProcessingInstruction, DocumentType, Comment, Whitespace and SignificantWhitespace.
  /////</summary>
  //public XmlNodeType MoveToContent();

  ///<summary>
  /// Skips to the end tag of the current element.
  ///</summary>
  public void Skip();

  //#endregion

  #region Read but return no significant value.

  ///<summary>
  /// Reads the next node from the stream.
  ///</summary>
  public void Read();

  ///<summary>
  /// Parses the attribute value into one or more Text and/or EntityReference node types.
  ///</summary>
  /// <returns>True if there are nodes to return. False otherwise.</returns>
  public bool ReadAttributeValue();

  #endregion

  #region Start/EndElement checking

  ///<summary>
  /// Calls MoveToContent and tests if the current content node is a start tag or empty element tag (XmlNodeType.Element).
  ///</summary>
  public bool IsStartElement();

  ///<summary>
  /// Calls MoveToContent and tests if the current content node is a start tag or empty element tag (XmlNodeType.Element)
  /// and if the Name property of the element found matches the given argument.
  ///</summary>
  public bool IsStartElement(string name);

  ///<summary>
  /// Calls MoveToContent and tests if the current content node is a start tag or empty element tag (XmlNodeType.Element)
  /// and if the LocalName and NamespaceURI properties of the element found match the given strings.
  ///</summary>
  public bool IsStartElement(XmlQualifiedTagName fullName);

  ///<summary>
  /// Checks that the current node is an element and advances the reader to the next node.
  ///</summary>
  public void ReadStartElement();

  ///<summary>
  /// Checks that the current content node is an element with the given Name and advances the reader to the next node.
  ///</summary>
  public void ReadStartElement(string name);

  ///<summary>
  /// Checks that the current content node is an element with the given LocalName and NamespaceURI and advances the reader to the next node.
  ///</summary>
  public void ReadStartElement(XmlQualifiedTagName fullName);

  /// <summary>
  /// Checks if reader node type is EndElement.
  /// </summary>
  public bool IsEndElement();

  /// <summary>
  /// Checks if reader node type is EndElement and reade name is a specified string
  /// </summary>
  public bool IsEndElement(string name);

  /// <summary>
  /// Checks if reader node type is EndElement and reade name is a specified local name and namespaceURI is a specified namespace.
  /// </summary>
  public bool IsEndElement(XmlQualifiedTagName tag);

  /// <summary>
  /// Wrapper for ReadEndElement operation.
  /// Checks that the current content node is an end tag and advances the reader to the next node.
  /// </summary>
  public void ReadEndElement();

  /// <summary>
  /// Wrapper for ReadEndElement operation.
  /// Checks that the current content node is an end tag with a specified name and advances the reader to the next node.
  /// </summary>
  public void ReadEndElement(string name);

  /// <summary>
  /// Wrapper for ReadEndElement operation.
  /// Checks that the current content node is an end tag with a specified name and namespaceURI and advances the reader to the next node.
  /// </summary>
  public void ReadEndElement(XmlQualifiedTagName tag);

  #endregion

}
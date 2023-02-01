using System.Xml;
using System.Xml.Schema;

namespace Qhta.Xml.Serialization;

public interface IXmlReader
{
  ///<summary>
  /// IDisposable interface
  ///</summary>
  public void Dispose();


  ///<summary>
  /// Settings
  ///</summary>
  public XmlReaderSettings? Settings { get; }

  #region Node Properties

  ///<summary>
  /// Get the type of the current node.
  ///</summary>
  public XmlNodeType NodeType { get; }

  ///<summary>
  /// Gets the name of the current node, including the namespace prefix.
  ///</summary>
  public string Name { get; }

  ///<summary>
  /// Gets the name of the current node without the namespace prefix.
  ///</summary>
  public string LocalName { get; }

  ///<summary>
  /// Gets the namespace URN (as defined in the W3C Namespace Specification) of the current namespace scope.
  ///</summary>
  public string NamespaceURI { get; }

  ///<summary>
  /// Gets the namespace prefix associated with the current node.
  ///</summary>
  public string Prefix { get; }

  ///<summary>
  /// Gets a value indicating whether
  ///</summary>
  public bool HasValue { get; }

  ///<summary>
  /// Gets the text value of the current node.
  ///</summary>
  public string Value { get; }

  ///<summary>
  /// Gets the depth of the current node in the XML element stack.
  ///</summary>
  public int Depth { get; }

  ///<summary>
  /// Gets the base URI of the current node.
  ///</summary>
  public string BaseURI { get; }

  ///<summary>
  /// Gets a value indicating whether the current node is an empty element (for example, <MyElement/>).
  ///</summary>
  public bool IsEmptyElement { get; }

  #endregion

  #region  Gets a value indicating whether the current node is an attribute that was generated from the default value defined

  ///<summary>
  /// in the DTD or schema.
  ///</summary>
  public bool IsDefault { get; }

  ///<summary>
  /// Gets the quotation mark character used to enclose the value of an attribute node.
  ///</summary>
  public char QuoteChar { get; }

  ///<summary>
  /// Gets the current xml:space scope.
  ///</summary>
  public XmlSpace XmlSpace { get; }

  ///<summary>
  /// Gets the current xml:lang scope.
  ///</summary>
  public string XmlLang { get; }

  ///<summary>
  /// returns the schema info interface of the reader
  ///</summary>
  public IXmlSchemaInfo? SchemaInfo { get; }

  ///<summary>
  /// returns the type of the current node
  ///</summary>
  public Type ValueType { get; }
  #endregion

  #region Attribute Accessors
  ///<summary>
  /// The number of attributes on the current node.
  ///</summary>
  public int AttributeCount { get; }

  ///<summary>
  /// Gets the value of the attribute with the specified index.
  ///</summary>
  public string this[int i] { get; }

  ///<summary>
  /// Gets the value of the attribute with the specified Name.
  ///</summary>
  public string? this[string name] { get; }

  ///<summary>
  /// Gets the value of the attribute with the LocalName and NamespaceURI
  ///</summary>
  public string? this[string name, string? namespaceURI] { get; }

  ///<summary>
  /// Returns true when the XmlReader is positioned at the end of the stream.
  ///</summary>
  public bool EOF { get; }

  ///<summary>
  /// Returns the read state of the XmlReader.
  ///</summary>
  public ReadState ReadState { get; }

  ///<summary>
  /// Gets the XmlNameTable associated with the XmlReader.
  ///</summary>
  public XmlNameTable NameTable { get; }

  ///<summary>
  /// Returns true if the XmlReader can expand general entities.
  ///</summary>
  public bool CanResolveEntity { get; }

  #endregion

  #region  Binary content access methods

  ///<summary>
  /// Returns true if the reader supports call to ReadContentAsBase64, ReadElementContentAsBase64, ReadContentAsBinHex and ReadElementContentAsBinHex.
  ///</summary>
  public bool CanReadBinaryContent { get; }

  ///<summary>
  /// Text streaming methods
  ///</summary>

  ///<summary>
  /// Returns true if the XmlReader supports calls to ReadValueChunk.
  ///</summary>
  public bool CanReadValueChunk { get; }

  ///<summary>
  /// Returns true when the current node has any attributes.
  ///</summary>
  public bool HasAttributes { get; }

  #endregion

  #region  Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references,

  ///<summary>
  /// and returns the content as the most appropriate type (by default as string). Stops at start tags and end tags.
  ///</summary>
  public object ReadContentAsObject();

  #endregion

  #region  Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references,

  ///<summary>
  /// and converts the content to a boolean. Stops at start tags and end tags.
  ///</summary>
  public bool ReadContentAsBoolean();

  #endregion

  #region  Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references,

  ///<summary>
  /// and converts the content to a DateTime. Stops at start tags and end tags.
  ///</summary>
  public DateTime ReadContentAsDateTime();

  #endregion

  #region  Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references,

  ///<summary>
  /// and converts the content to a DateTimeOffset. Stops at start tags and end tags.
  ///</summary>
  public DateTimeOffset ReadContentAsDateTimeOffset();

  #endregion

  #region  Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references,

  ///<summary>
  /// and converts the content to a double. Stops at start tags and end tags.
  ///</summary>
  public double ReadContentAsDouble();

  #endregion

  #region  Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references,

  ///<summary>
  /// and converts the content to a float. Stops at start tags and end tags.
  ///</summary>
  public float ReadContentAsFloat();

  #endregion

  #region  Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references,

  ///<summary>
  /// and converts the content to a decimal. Stops at start tags and end tags.
  ///</summary>
  public decimal ReadContentAsDecimal();

  #endregion

  #region  Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references,

  ///<summary>
  /// and converts the content to an int. Stops at start tags and end tags.
  ///</summary>
  public int ReadContentAsInt();

  #endregion

  #region  Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references,

  ///<summary>
  /// and converts the content to a long. Stops at start tags and end tags.
  ///</summary>
  public long ReadContentAsLong();

  #endregion

  #region  Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references,

  ///<summary>
  /// and returns the content as a string. Stops at start tags and end tags.
  ///</summary>
  public string ReadContentAsString();

  #endregion

  #region  Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references,

  ///<summary>
  /// and converts the content to the requested type. Stops at start tags and end tags.
  ///</summary>
  public object ReadContentAs(Type returnType, IXmlNamespaceResolver? namespaceResolver);

  ///<summary>
  /// Returns the content of the current element as the most appropriate type. Moves to the node following the element's end tag.
  ///</summary>
  public object ReadElementContentAsObject();

  ///<summary>
  /// Checks local name and namespace of the current element and returns its content as the most appropriate type. Moves to the node following the element's end tag.
  ///</summary>
  public object ReadElementContentAsObject(string localName, string namespaceURI);

  ///<summary>
  /// Returns the content of the current element as a boolean. Moves to the node following the element's end tag.
  ///</summary>
  public bool ReadElementContentAsBoolean();

  ///<summary>
  /// Checks local name and namespace of the current element and returns its content as a boolean. Moves to the node following the element's end tag.
  ///</summary>
  public bool ReadElementContentAsBoolean(string localName, string namespaceURI);

  ///<summary>
  /// Returns the content of the current element as a DateTime. Moves to the node following the element's end tag.
  ///</summary>
  public DateTime ReadElementContentAsDateTime();

  #endregion

  #region  Checks local name and namespace of the current element and returns its content as a DateTime.

  ///<summary>
  /// Moves to the node following the element's end tag.
  ///</summary>
  public DateTime ReadElementContentAsDateTime(string localName, string namespaceURI);

  ///<summary>
  /// Returns the content of the current element as a double. Moves to the node following the element's end tag.
  ///</summary>
  public double ReadElementContentAsDouble();

  #endregion

  #region  Checks local name and namespace of the current element and returns its content as a double.

  ///<summary>
  /// Moves to the node following the element's end tag.
  ///</summary>
  public double ReadElementContentAsDouble(string localName, string namespaceURI);

  ///<summary>
  /// Returns the content of the current element as a float. Moves to the node following the element's end tag.
  ///</summary>
  public float ReadElementContentAsFloat();

  #endregion

  #region  Checks local name and namespace of the current element and returns its content as a float.

  ///<summary>
  /// Moves to the node following the element's end tag.
  ///</summary>
  public float ReadElementContentAsFloat(string localName, string namespaceURI);

  ///<summary>
  /// Returns the content of the current element as a decimal. Moves to the node following the element's end tag.
  ///</summary>
  public decimal ReadElementContentAsDecimal();

  #endregion

  #region  Checks local name and namespace of the current element and returns its content as a decimal.

  ///<summary>
  /// Moves to the node following the element's end tag.
  ///</summary>
  public decimal ReadElementContentAsDecimal(string localName, string namespaceURI);

  ///<summary>
  /// Returns the content of the current element as an int. Moves to the node following the element's end tag.
  ///</summary>
  public int ReadElementContentAsInt()
  ;

  #endregion

  #region  Checks local name and namespace of the current element and returns its content as an int.

  ///<summary>
  /// Moves to the node following the element's end tag.
  ///</summary>
  public int ReadElementContentAsInt(string localName, string namespaceURI)
  ;

  ///<summary>
  /// Returns the content of the current element as a long. Moves to the node following the element's end tag.
  ///</summary>
  public long ReadElementContentAsLong()
  ;

  #endregion

  #region  Checks local name and namespace of the current element and returns its content as a long.

  ///<summary>
  /// Moves to the node following the element's end tag.
  ///</summary>
  public long ReadElementContentAsLong(string localName, string namespaceURI)
  ;

  ///<summary>
  /// Returns the content of the current element as a string. Moves to the node following the element's end tag.
  ///</summary>
  public string ReadElementContentAsString();

  #endregion

  #region  Checks local name and namespace of the current element and returns its content as a string.

  ///<summary>
  /// Moves to the node following the element's end tag.
  ///</summary>
  public string ReadElementContentAsString(string localName, string namespaceURI)
  ;

  ///<summary>
  /// Returns the content of the current element as the requested type. Moves to the node following the element's end tag.
  ///</summary>
  public object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver);

  #endregion

  #region  Checks local name and namespace of the current element and returns its content as the requested type.

  ///<summary>
  /// Moves to the node following the element's end tag.
  ///</summary>
  public object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver, string localName, string namespaceURI);

  ///<summary>
  /// Gets the value of the attribute with the specified Name
  ///</summary>
  public string? GetAttribute(string name);

  ///<summary>
  /// Gets the value of the attribute with the LocalName and NamespaceURI
  ///</summary>
  public string? GetAttribute(string name, string? namespaceURI);

  ///<summary>
  /// Gets the value of the attribute with the specified index.
  ///</summary>
  public string GetAttribute(int i);

  ///<summary>
  /// Moves to the attribute with the specified Name.
  ///</summary>
  public bool MoveToAttribute(string name);

  ///<summary>
  /// Moves to the attribute with the specified LocalName and NamespaceURI.
  ///</summary>
  public bool MoveToAttribute(string name, string? ns);

  ///<summary>
  /// Moves to the attribute with the specified index.
  ///</summary>
  public void MoveToAttribute(int i);

  ///<summary>
  /// Moves to the first attribute of the current node.
  ///</summary>
  public bool MoveToFirstAttribute();

  ///<summary>
  /// Moves to the next attribute.
  ///</summary>
  public bool MoveToNextAttribute();

  ///<summary>
  /// Moves to the element that contains the current attribute node.
  ///</summary>
  public bool MoveToElement();

  ///<summary>
  /// Parses the attribute value into one or more Text and/or EntityReference node types.
  ///</summary>

  public bool ReadAttributeValue();

  #endregion

  #region  Moving through the Stream

  ///<summary>
  /// Reads the next node from the stream.
  ///</summary>

  public bool Read();

  ///<summary>
  /// Closes the stream/TextReader (if CloseInput==true), changes the ReadState to Closed, and sets all the properties back to zero/empty string.
  ///</summary>
  public void Close();

  ///<summary>
  /// Skips to the end tag of the current element.
  ///</summary>
  public void Skip();

  ///<summary>
  /// Resolves a namespace prefix in the current element's scope.
  ///</summary>
  public string? LookupNamespace(string prefix);

  ///<summary>
  /// Resolves the entity reference for nodes of NodeType EntityReference.
  ///</summary>
  public void ResolveEntity();

  ///<summary>
  /// Returns decoded bytes of the current base64 text content. Call this methods until it returns 0 to get all the data.
  ///</summary>
  public int ReadContentAsBase64(byte[] buffer, int index, int count);

  ///<summary>
  /// Returns decoded bytes of the current base64 element content. Call this methods until it returns 0 to get all the data.
  ///</summary>
  public int ReadElementContentAsBase64(byte[] buffer, int index, int count);

  ///<summary>
  /// Returns decoded bytes of the current binhex text content. Call this methods until it returns 0 to get all the data.
  ///</summary>
  public int ReadContentAsBinHex(byte[] buffer, int index, int count);

  ///<summary>
  /// Returns decoded bytes of the current binhex element content. Call this methods until it returns 0 to get all the data.
  ///</summary>
  public int ReadElementContentAsBinHex(byte[] buffer, int index, int count);

  #endregion

  #region  Returns a chunk of the value of the current node. Call this method in a loop to get all the data.

  ///<summary>
  /// Use this method to get a streaming access to the value of the current node.
  ///</summary>
  public int ReadValueChunk(char[] buffer, int index, int count);

  #endregion

  #region  helper methods

  ///<summary>
  /// Reads the contents of an element as a string. Stops of comments, PIs or entity references.
  ///</summary>
  public string ReadString();

  #endregion

  #region  Checks whether the current node is a content (non-whitespace text, CDATA, Element, EndElement, EntityReference

  ///<summary>
  /// or EndEntity) node. If the node is not a content node, then the method skips ahead to the next content node or
  ///</summary>
  ///<summary>
  /// end of file. Skips over nodes of type ProcessingInstruction, DocumentType, Comment, Whitespace and SignificantWhitespace.
  ///</summary>
  public XmlNodeType MoveToContent();

  ///<summary>
  /// Checks that the current node is an element and advances the reader to the next node.
  ///</summary>
  public void ReadStartElement();

  ///<summary>
  /// Checks that the current content node is an element with the given Name and advances the reader to the next node.
  ///</summary>
  public void ReadStartElement(string name);

  #endregion

  #region  Checks that the current content node is an element with the given LocalName and NamespaceURI

  ///<summary>
  /// and advances the reader to the next node.
  ///</summary>
  public void ReadStartElement(string localname, string ns);

  ///<summary>
  /// Reads a text-only element.
  ///</summary>
  public string ReadElementString();

  ///<summary>
  /// Checks that the Name property of the element found matches the given string before reading a text-only element.
  ///</summary>
  public string ReadElementString(string name);

  #endregion

  #region  Checks that the LocalName and NamespaceURI properties of the element found matches the given strings

  ///<summary>
  /// before reading a text-only element.
  ///</summary>
  public string ReadElementString(string localname, string ns);

  ///<summary>
  /// Checks that the current content node is an end tag and advances the reader to the next node.
  ///</summary>
  public void ReadEndElement();

  ///<summary>
  /// Calls MoveToContent and tests if the current content node is a start tag or empty element tag (XmlNodeType.Element).
  ///</summary>
  public bool IsStartElement();

  #endregion

  #region  Calls MoveToContent and tests if the current content node is a start tag or empty element tag (XmlNodeType.Element) and if the

  ///<summary>
  /// Name property of the element found matches the given argument.
  ///</summary>
  public bool IsStartElement(string name);

  #endregion

  #region  Calls MoveToContent and tests if the current content node is a start tag or empty element tag (XmlNodeType.Element) and if

  ///<summary>
  /// the LocalName and NamespaceURI properties of the element found match the given strings.
  ///</summary>
  public bool IsStartElement(string localname, string ns)
  ;

  ///<summary>
  /// Reads to the following element with the given Name.
  ///</summary>
  public bool ReadToFollowing(string name);

  ///<summary>
  /// Reads to the following element with the given LocalName and NamespaceURI.
  ///</summary>
  public bool ReadToFollowing(string localName, string namespaceURI);

  ///<summary>
  /// Reads to the first descendant of the current element with the given Name.
  ///</summary>
  public bool ReadToDescendant(string name);

  ///<summary>
  /// Reads to the first descendant of the current element with the given LocalName and NamespaceURI.
  ///</summary>
  public bool ReadToDescendant(string localName, string namespaceURI);

  ///<summary>
  /// Reads to the next sibling of the current element with the given Name.
  ///</summary>
  public bool ReadToNextSibling(string name);
  ///<summary>
  /// Reads to the next sibling of the current element with the given LocalName and NamespaceURI.
  ///</summary>
  public bool ReadToNextSibling(string localName, string namespaceURI);

  #endregion

  #region  Returns true if the given argument is a valid NmToken.

  ///<summary>
  /// Returns the inner content (including markup) of an element or attribute as a string.
  ///</summary>
  public string ReadInnerXml();

  ///<summary>
  /// Returns the current element and its descendants or an attribute as a string.
  ///</summary>
  public string ReadOuterXml();

  ///<summary>
  /// Returns an XmlReader that will read only the current element and its descendants and then go to EOF state.
  ///</summary>
  public XmlReader ReadSubtree();

  #endregion

  public WhitespaceHandling? WhitespaceHandling{ get; set; }

}
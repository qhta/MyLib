using System.Xml;
using System.Xml.XPath;

namespace Qhta.Xml.Serialization;
public interface IXmlWriter
{
  /// <summary>
  /// Closes the XmlWriter and the underlying stream/TextReader (if Settings.CloseOutput is true).
  /// </summary>
  public void Close();

  /// <summary>
  /// Flushes data that is in the internal buffers into the underlying streams/TextReader and flushes the stream/TextReader.
  /// </summary>
  public void Flush();

  /// <summary>
  /// Gets or sets a value indicating whether namespaces will be written.
  /// </summary>
  public bool EmitNamespaces { get; set; }

  /// <summary>
  /// Returns the settings describing the features of the writer. Returns null for V1 XmlWriters (XmlTextWriter).
  /// </summary>
  public XmlWriterSettings? Settings { get; }

  #region Write methods

  /// <summary>
  /// Writes out the XML declaration with the version "1.0".
  /// </summary>
  public void WriteStartDocument();

  /// <summary>
  /// Writes out the XML declaration with the version "1.0" and the specified standalone attribute.
  /// </summary>
  public void WriteStartDocument(bool standalone);

  /// <summary>
  /// Closes any open elements or attributes and puts the writer back in the Start state.
  /// </summary>
  public void WriteEndDocument();

  /// <summary>
  /// Writes the nil attribute.
  /// </summary>
  /// <param name="xsiNamespace">The xsi namespace.</param>
  public void WriteNilAttribute(string xsiNamespace);

  ///// <summary>
  ///// Writes out the DOCTYPE declaration with the specified name and optional attributes.
  ///// </summary>
  //public void WriteDocType(string name, string? pubid = null, string? sysid = null, string? subset = null);


  /// <summary>
  /// Writes out the specified start tag with a specified tag name.
  /// </summary>
  public void WriteStartElement(XmlQualifiedTagName name);

  /// <summary>
  /// Writes out the specified start tag with a specified local name.
  /// </summary>
  public void WriteStartElement(string localName);

  /// <summary>
  /// Closes one element and pops the corresponding namespace scope.
  /// </summary>
  public void WriteEndElement(XmlQualifiedTagName name);

  /// <summary>
  /// Closes one element and pops the corresponding namespace scope.
  /// </summary>
  public void WriteEndElement(string localName);

  /// <summary>
  /// Closes one element and pops the corresponding namespace scope. Writes out a full end element tag, e.g.
  /// </summary>
  public void WriteFullEndElement(XmlQualifiedTagName fullName);

  /// <summary>
  /// Closes one element and pops the corresponding namespace scope. Writes out a full end element tag, e.g.
  /// </summary>
  public void WriteFullEndElement(string localName);

  /// <summary>
  /// Writes the namespace definition.
  /// </summary>
  public void WriteNamespaceDef(string prefix, string ns);

  /// <summary>
  /// Writes out the attribute with the specified tag name.
  /// </summary>
  public void WriteAttributeString(XmlQualifiedTagName fullName, string? value);

  /// <summary>
  /// Writes out the attribute with the specified local name.
  /// </summary>
  public void WriteAttributeString(string localName, string? value);


  /// <summary>
  /// Writes the start of an attribute with a specified tag name.
  /// </summary>
  public void WriteStartAttribute(XmlQualifiedTagName fullName);

  /// <summary>
  /// Writes the start of an attribute with a specified local name.
  /// </summary>
  public void WriteStartAttribute(string localName);

  /// <summary>
  /// Closes the attribute opened by WriteStartAttribute call.
  /// </summary>
  public void WriteEndAttribute(XmlQualifiedTagName fullName);

  /// <summary>
  /// Closes the attribute opened by WriteStartAttribute call.
  /// </summary>
  public void WriteEndAttribute(string localName);

  /// <summary>
  /// Writes out the given whitespace.
  /// </summary>
  public void WriteWhitespace(string? ws);

  /// <summary>
  /// Writes out the specified text content.
  /// </summary>
  public void WriteString(string? text);

  ///// <summary>
  ///// Writes out the specified text content.
  ///// </summary>
  //public void WriteChars(char[] buffer, int index, int count);

  ///// <summary>
  ///// Writes raw markup from the given character buffer.
  ///// </summary>
  //public void WriteRaw(char[] buffer, int index, int count);

  ///// <summary>
  ///// Writes raw markup from the given string.
  ///// </summary>
  //public void WriteRaw(string data);

  ///// <summary>
  ///// Encodes the specified binary bytes as base64 and writes out the resulting text.
  ///// </summary>
  //public void WriteBase64(byte[] buffer, int index, int count);

  ///// <summary>
  ///// Encodes the specified binary bytes as binhex and writes out the resulting text.
  ///// </summary>
  //public void WriteBinHex(byte[] buffer, int index, int count);
  //#endregion

  #region status
  /// <summary>
  /// Returns the state of the XmlWriter.
  /// </summary>
  public WriteState WriteState { get; }

  /// <summary>
  /// Gets an XmlSpace representing the current xml:space scope.
  /// </summary>
  public XmlSpace XmlSpace { get; }

  /// <summary>
  /// Gets the current xml:lang scope.
  /// </summary>
  public string? XmlLang { get; }
  #endregion

  ///// <summary>
  ///// Returns the closest prefix defined in the current namespace scope for the specified namespace URI.
  ///// </summary>
  //public string? LookupPrefix(string ns);

  //#region Scalar Value Methods

  ///// <summary>
  ///// Writes out the specified name, ensuring it is a valid NmToken according to the XML specification
  ///// </summary>
  //public void WriteNmToken(string name);

  ///// <summary>
  ///// Writes out the specified name, ensuring it is a valid Name according to the XML specification
  ///// </summary>
  //public void WriteName(string name);

  ///// <summary>
  ///// Writes out the specified namespace-qualified name by looking up the prefix that is in scope for the given namespace.
  ///// </summary>
  //public void WriteQualifiedName(string localName, string? ns = null);

  /// <summary>
  /// Writes out the specified value.
  /// </summary>
  public void WriteValue(object value);

  ///// <summary>
  ///// Writes out the specified value.
  ///// </summary>
  //public void WriteValue(string? value);

  ///// <summary>
  ///// Writes out the specified value.
  ///// </summary>
  //public void WriteValue(bool value);

  ///// <summary>
  ///// Writes out the specified value.
  ///// </summary>
  //public void WriteValue(DateTime value);

  ///// <summary>
  ///// Writes out the specified value.
  ///// </summary>
  //public void WriteValue(DateTimeOffset value);

  ///// <summary>
  ///// Writes out the specified value.
  ///// </summary>
  //public void WriteValue(double value);

  ///// <summary>
  ///// Writes out the specified value.
  ///// </summary>
  //public void WriteValue(float value);

  ///// <summary>
  ///// Writes out the specified value.
  ///// </summary>
  //public void WriteValue(decimal value);

  ///// <summary>
  ///// Writes out the specified value.
  ///// </summary>
  //public void WriteValue(int value);

  ///// <summary>
  ///// Writes out the specified value.
  ///// </summary>
  //public void WriteValue(long value);
  //#endregion

  //#region XmlReader Helper Methods

  ///// <summary>
  ///// Writes out all the attributes found at the current position in the specified XmlReader.
  ///// </summary>
  //public void WriteAttributes(XmlReader reader, bool defattr);

  ///// <summary>
  ///// Copies the current node from the given reader to the writer (including child nodes), and if called on an element moves the XmlReader
  ///// </summary>
  ///// <summary>
  ///// to the corresponding end element.
  ///// </summary>
  //public void WriteNode(XmlReader reader, bool defattr);

  ///// <summary>
  ///// Copies the current node from the given XPathNavigator to the writer (including child nodes).
  ///// </summary>
  //public void WriteNode(XPathNavigator navigator, bool defattr);
  //#endregion

  //#region Element Helper Methods

  ///// <summary>
  ///// Writes out an element with the specified name containing the specified string value.
  ///// </summary>
  //public void WriteElementString(string localName, string? value);

  ///// <summary>
  ///// Writes out an attribute with the specified name, namespace URI and string value.
  ///// </summary>
  //public void WriteElementString(string localName, string? ns, string? value);

  ///// <summary>
  ///// Writes out an attribute with the specified name, namespace URI, and string value.
  ///// </summary>
  //public void WriteElementString(string? prefix, string localName, string? ns, string? value);
  #endregion

  #region extra Methods
  public void WriteSignificantSpaces(bool value);

  #endregion
}

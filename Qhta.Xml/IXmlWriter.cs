using System.Xml;
using System.Xml.XPath;

namespace Qhta.Xml.Serialization;
public interface IXmlWriter
{
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

  //Closes any open elements or attributes and puts the writer back in the Start state.
  public void WriteEndDocument();

  /// <summary>
  /// Writes out the DOCTYPE declaration with the specified name and optional attributes.
  /// </summary>
  public void WriteDocType(string name, string? pubid = null, string? sysid = null, string? subset = null);

  /// <summary>
  /// Writes out the specified start tag and associates it with the given namespace.
  /// </summary>
  public void WriteStartElement(string localName, string? ns = null);

  /// <summary>
  /// Writes out the specified start tag and associates it with the given namespace and prefix.
  /// </summary>
  public void WriteStartElement(string? prefix, string localName, string? ns = null);


  /// <summary>
  /// Closes one element and pops the corresponding namespace scope.
  /// </summary>
  public void WriteEndElement(string localName, string? ns = null);

  /// <summary>
  /// Closes one element and pops the corresponding namespace scope. Writes out a full end element tag, e.g. </element>.
  /// </summary>
  public void WriteFullEndElement(string localName, string? ns = null);

  /// <summary>
  /// Writes out the attribute with the specified LocalName, value, and NamespaceURI.
  /// </summary>
  public void WriteAttributeString(string localName, string? ns, string? value);

  /// <summary>
  /// Writes out the attribute with the specified LocalName and value.
  /// </summary>
  public void WriteAttributeString(string localName, string? value);

  /// <summary>
  /// Writes out the attribute with the specified prefix, LocalName, NamespaceURI and value.
  /// </summary>
  public void WriteAttributeString(string? prefix, string localName, string? ns, string? value);

  /// <summary>
  /// Writes the start of an attribute.
  /// </summary>
  public void WriteStartAttribute(string localName, string? ns);

  /// <summary>
  /// Writes the start of an attribute.
  /// </summary>
  public void WriteStartAttribute(string? prefix, string localName, string? ns);

  /// <summary>
  /// Writes the start of an attribute.
  /// </summary>
  public void WriteStartAttribute(string localName);

  /// <summary>
  /// Closes the attribute opened by WriteStartAttribute call.
  /// </summary>
  public void WriteEndAttribute();

  /// <summary>
  /// Writes out a <![CDATA[...]]>; block containing the specified text.
  /// </summary>
  public void WriteCData(string? text);

  /// <summary>
  /// Writes out a comment <!--...-->; containing the specified text.
  /// </summary>
  public void WriteComment(string? text);

  /// <summary>
  /// Writes out a processing instruction with a space between the name and text as follows: <?name text?>
  /// </summary>
  public void WriteProcessingInstruction(string name, string? text);

  /// <summary>
  /// Writes out an entity reference as follows: "&"+name+";".
  /// </summary>
  public void WriteEntityRef(string name);

  /// <summary>
  /// Forces the generation of a character entity for the specified Unicode character value.
  /// </summary>
  public void WriteCharEntity(char ch);

  /// <summary>
  /// Writes out the given whitespace.
  /// </summary>
  public void WriteWhitespace(string? ws);

  /// <summary>
  /// Writes out the specified text content.
  /// </summary>
  public void WriteString(string? text);

  /// <summary>
  /// Write out the given surrogate pair as an entity reference.
  /// </summary>
  public void WriteSurrogateCharEntity(char lowChar, char highChar);

  /// <summary>
  /// Writes out the specified text content.
  /// </summary>
  public void WriteChars(char[] buffer, int index, int count);

  /// <summary>
  /// Writes raw markup from the given character buffer.
  /// </summary>
  public void WriteRaw(char[] buffer, int index, int count);

  /// <summary>
  /// Writes raw markup from the given string.
  /// </summary>
  public void WriteRaw(string data);

  /// <summary>
  /// Encodes the specified binary bytes as base64 and writes out the resulting text.
  /// </summary>
  public void WriteBase64(byte[] buffer, int index, int count);

  /// <summary>
  /// Encodes the specified binary bytes as binhex and writes out the resulting text.
  /// </summary>
  public void WriteBinHex(byte[] buffer, int index, int count);

  /// <summary>
  /// Returns the state of the XmlWriter.
  /// </summary>
  public WriteState WriteState { get; }

  /// <summary>
  /// Closes the XmlWriter and the underlying stream/TextReader (if Settings.CloseOutput is true).
  /// </summary>
  public void Close();

  /// <summary>
  /// Flushes data that is in the internal buffers into the underlying streams/TextReader and flushes the stream/TextReader.
  /// </summary>
  public void Flush();

  /// <summary>
  /// Returns the closest prefix defined in the current namespace scope for the specified namespace URI.
  /// </summary>
  public string? LookupPrefix(string ns);

  /// <summary>
  /// Gets an XmlSpace representing the current xml:space scope.
  /// </summary>
  public XmlSpace XmlSpace { get; }

  /// <summary>
  /// Gets the current xml:lang scope.
  /// </summary>
  public string? XmlLang { get; }
  #endregion

  #region Scalar Value Methods

  /// <summary>
  /// Writes out the specified name, ensuring it is a valid NmToken according to the XML specification
  /// </summary>
  public void WriteNmToken(string name);

  /// <summary>
  /// Writes out the specified name, ensuring it is a valid Name according to the XML specification
  /// </summary>
  public void WriteName(string name);

  /// <summary>
  /// Writes out the specified namespace-qualified name by looking up the prefix that is in scope for the given namespace.
  /// </summary>
  public void WriteQualifiedName(string localName, string? ns = null);

  /// <summary>
  /// Writes out the specified value.
  /// </summary>
  public void WriteValue(object value);

  /// <summary>
  /// Writes out the specified value.
  /// </summary>
  public void WriteValue(string? value);

  /// <summary>
  /// Writes out the specified value.
  /// </summary>
  public void WriteValue(bool value);

  /// <summary>
  /// Writes out the specified value.
  /// </summary>
  public void WriteValue(DateTime value);

  /// <summary>
  /// Writes out the specified value.
  /// </summary>
  public void WriteValue(DateTimeOffset value);

  /// <summary>
  /// Writes out the specified value.
  /// </summary>
  public void WriteValue(double value);

  /// <summary>
  /// Writes out the specified value.
  /// </summary>
  public void WriteValue(float value);

  /// <summary>
  /// Writes out the specified value.
  /// </summary>
  public void WriteValue(decimal value);

  /// <summary>
  /// Writes out the specified value.
  /// </summary>
  public void WriteValue(int value);

  /// <summary>
  /// Writes out the specified value.
  /// </summary>
  public void WriteValue(long value);
  #endregion

  #region XmlReader Helper Methods

  /// <summary>
  /// Writes out all the attributes found at the current position in the specified XmlReader.
  /// </summary>
  public void WriteAttributes(XmlReader reader, bool defattr);

  /// <summary>
  /// Copies the current node from the given reader to the writer (including child nodes), and if called on an element moves the XmlReader
  /// </summary>
  /// <summary>
  /// to the corresponding end element.
  /// </summary>
  public void WriteNode(XmlReader reader, bool defattr);

  /// <summary>
  /// Copies the current node from the given XPathNavigator to the writer (including child nodes).
  /// </summary>
  public void WriteNode(XPathNavigator navigator, bool defattr);
  #endregion

  #region Element Helper Methods

  /// <summary>
  /// Writes out an element with the specified name containing the specified string value.
  /// </summary>
  public void WriteElementString(string localName, string? value);

  /// <summary>
  /// Writes out an attribute with the specified name, namespace URI and string value.
  /// </summary>
  public void WriteElementString(string localName, string? ns, string? value);

  /// <summary>
  /// Writes out an attribute with the specified name, namespace URI, and string value.
  /// </summary>
  public void WriteElementString(string? prefix, string localName, string? ns, string? value);
  #endregion

  #region extra Methods
  public void WriteSignificantSpaces(bool value);

  #endregion
}

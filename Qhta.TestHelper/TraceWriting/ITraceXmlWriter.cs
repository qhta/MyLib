using System.Diagnostics;

namespace Qhta.TestHelper;

/// <summary>
/// Expands <see cref="ITraceTextWriter"/> to handle XML writting.
/// </summary>
/// <seealso cref="Qhta.TestHelper.ITraceTextWriter" />
public interface ITraceXmlWriter: ITraceTextWriter
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

  #region prefixes and namespaces
  void WriteNamespacePrefix(string prefix, string ns);
  #endregion

  #region WriteStartElement
  void WriteStartElement(string tag) => WriteStartElement(null, tag, null);

  void WriteStartElement(string? prefix, string localName) => WriteStartElement(prefix, localName, null);

  void WriteStartElement(string? prefix, string localName, string? ns);
  #endregion

  #region WriteEndElement
  [DebuggerStepThrough]
  void WriteEndElement(string tag) => WriteEndElement(null, tag, null);

  [DebuggerStepThrough]
  void WriteEndElement(string? prefix, string localName) => WriteEndElement(prefix, localName, null);

  [DebuggerStepThrough]
  void WriteEndElement(string? prefix, string localName, string? ns);
  #endregion

  #region WriteAttributeString
  void WriteAttributeString(string attrName, string? value) => WriteAttributeString(null, attrName, null, value);

  void WriteAttributeString(string? prefix, string localName, string? value) => WriteAttributeString(prefix, localName, null, value);

  void WriteAttributeString(string? prefix, string localName, string? ns, string? value);
  #endregion

  #region WriteElementString
  void WriteElementString(string tagName, string? value) => WriteElementString(null, tagName, null, value);

  void WriteElementString(string? prefix, string localName, string? value) => WriteElementString(prefix, localName, null, value);

  void WriteElementString(string? prefix, string localName, string? ns, string? value);
  #endregion

  #region WriteEmptyElement
  void WriteEmptyElement(string tagName) => WriteEmptyElement(null, tagName, null);

  void WriteEmptyElement(string? prefix, string localName) => WriteEmptyElement(prefix, localName, null);

  void WriteEmptyElement(string? prefix, string localName, string? ns);
  #endregion

  #region WriteValue
  void WriteValue(string? str) => WriteValue(null, str);

  /// <param name="prefix">needed only to enable suppression</param>
  /// <param name="str">Written string</param>
  void WriteValue(string? prefix, string? str);
  #endregion

  void WriteSignificantSpaces(bool value);

  /// <param name="prefix">needed only to enable suppression</param>
  void WriteStartComment(string? prefix);

  /// <param name="prefix">needed only to enable suppression</param>
  void WriteEndComment(string? prefix);

  void WriteComment(string? text) => WriteComment(null, text);

  /// <param name="prefix">needed only to enable suppression</param>
  /// <param name="text">Written comment text</param>
  void WriteComment(string? prefix, string? text);

  void SuppressPrefix(string prefix, bool value = true);
}
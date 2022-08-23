using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Qhta.Xml.Serialization
{
  /// <summary>
  /// This basic interface for a XML serializer or may by a JSON serializer.
  /// </summary>
  public interface IXSerializer
  {

    /// <summary>
    /// Collection of types registered at the beginning.
    /// </summary>
    KnownTypesDictionary KnownTypes { get; }

    /// <summary>
    /// Options used while serialization
    /// </summary>
    SerializationOptions Options { get; }

    #region Serialize methods
    /// <summary>
    /// Serialize any object to a stream
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="obj"></param>
    void Serialize(Stream stream, object obj);

    /// <summary>
    /// Serialize any object to a text writer
    /// </summary>
    /// <param name="writer">target writer</param>
    /// <param name="obj"></param>
    void Serialize(TextWriter writer, object obj);
    #endregion

    #region Write methods
    /// <summary>
    /// Write any object to the serialization target.
    /// </summary>
    /// <param name="writer">target writer</param>
    /// <param name="obj">Any object to serialize</param>
    void WriteObject(IXWriter writer, object obj);

    /// <summary>
    /// Write object interior to the serialization target.
    /// </summary>
    /// <param name="writer">target writer</param>
    /// <param name="tag">elementName used to write internal properties</param>
    /// <param name="obj">Any object to serialize</param>
    void WriteObjectInterior(IXWriter writer, string? tag, object obj);

    /// <summary>
    /// Write object properties as XML attributes.
    /// Writes only properties that are marked with <see cref="System.Xml.XmlAttribute"/>
    /// or <see cref="XmlOrderedAttribAttribute"/>
    /// and only that have simple values (string etc).
    /// </summary>
    /// <param name="writer">target writer</param>
    /// <param name="obj">Any object to serialize its properties as XML attributes</param>
    /// <returns></returns>
    int WriteAttributesBase(IXWriter writer, object obj);

    /// <summary>
    /// Write object properties as XML elements.
    /// Writes properties that are marked with <see cref="System.Xml.XmlElement"/>
    /// and those properties that are marked with <see cref="System.Xml.XmlAttribute"/>
    /// or <see cref="XmlOrderedAttribAttribute"/>
    /// and do not have simple values (string etc).
    /// </summary>
    /// <param name="writer">target writer</param>
    /// <param name="obj">Any object to serialize its properties as XML elements</param>
    /// <param name="elementTag">XML element name to place before each property name 
    /// when <see cref="SerializationOptions.PrecedePropertyNameWithElementName"/> is set</param>
    int WritePropertiesBase(IXWriter writer, string? elementTag, object obj);

    /// <summary>
    /// Write some collection as XML element with children.
    /// Each element is written as XML element.
    /// The elements that have simple value (string etc) are written as "item" elements
    /// (according to <see cref="SerializationOptions.ItemTag"/> option).
    /// </summary>
    /// <param name="writer">target writer</param>
    /// <param name="collection">Any object implementing <see cref="System.Collections.ICollection"/> interface</param>
    /// <param name="elementTag">XML element name to place before collection tag 
    /// when <see cref="SerializationOptions.PrecedePropertyNameWithElementName"/> is set</param>
    /// <param name="collectionTag">XML element name to place before the whole collection.</param>
    /// <param name="itemTypes">information about serializing items of the collection</param>
    int WriteCollectionBase(IXWriter writer, string? elementTag, string? collectionTag, ICollection collection, KnownItemTypesDictionary? itemTypes = null);

    /// <summary>
    /// Write XML element starting tag.
    /// </summary>
    /// <param name="writer">target writer</param>
    /// <param name="tag">XML element name </param>
    void WriteStartElement(IXWriter writer, string tag);

    /// <summary>
    /// Write XML element starting tag.
    /// </summary>
    /// <param name="writer">target writer</param>
    /// <param name="tag">XML element name </param>
    void WriteEndElement(IXWriter writer, string propTag);

    /// <summary>
    /// Write XML attribute value.
    /// </summary>
    /// <param name="writer">target writer</param>
    /// <param name="attrName">An XML attribute name</param>
    /// <param name="value"></param>
    void WriteAttributeString(IXWriter writer, string attrName, string value);

    /// <summary>
    /// Write XML element value
    /// </summary>
    /// <param name="writer">target writer</param>
    /// <param name="value">Any value to write as a content of XML element</param>
    void WriteValue(IXWriter writer, object? value);

    /// <summary>
    /// Write XML element with a value.
    /// First <see cref="WriteStartElement"/>, then <see cref="WriteValue"/>
    /// and finally <see cref="WriteEndElement"/> is called.
    /// </summary>
    /// <param name="writer">target writer</param>
    /// <param name="elementName"></param>
    /// <param name="value">Any value to write as a content of XML element</param>
    void WriteValue(IXWriter writer, string elementName, object value);
    #endregion

    #region Deserialize methods
    /// <summary>
    /// Deserialize any object from a stream
    /// </summary>
    /// <param name="stream"></param>
    object? Deserialize(Stream stream);

    /// <summary>
    /// Serialize any object from a text reader
    /// </summary>
    /// <param name="textReader"></param>
    object? Deserialize(TextReader textReader);
    #endregion

    #region Helper methods
    bool IsSimple(object value);

    string LowercaseName(string name);
    #endregion
  }
}

using System.Xml.XPath;

namespace Qhta.Xml.Serialization;

public partial class QXmlReader : IXmlReader, IDisposable
{
  public QXmlReader(XmlReader xmlReader)
  {
    _reader = xmlReader;
  }

  private XmlReader _reader { get; }


  public static implicit operator XmlReader(QXmlReader reader) => reader._reader;

  public void Dispose()
  {
    _reader.Dispose();
  }

  public XmlReaderSettings? Settings => _reader.Settings;

  public WhitespaceHandling? WhitespaceHandling
  {
    get { return (_reader as XmlTextReader)?.WhitespaceHandling; }
    set
    {
      if (_reader is XmlTextReader xmlTextReader && value != null)
        xmlTextReader.WhitespaceHandling = (WhitespaceHandling)value;
    }
  }

  public XmlNodeType NodeType => _reader.NodeType;

  public string Name => _reader.Name;

  public string LocalName => _reader.LocalName;

  public string NamespaceURI => _reader.NamespaceURI;

  public string Prefix => _reader.Prefix;

  public bool HasValue => _reader.HasValue;

  public string Value => _reader.Value;

  public int Depth => _reader.Depth;

  public string BaseURI => _reader.BaseURI;

  public bool IsEmptyElement => _reader.IsEmptyElement;

  public bool IsDefault => _reader.IsDefault;

  public char QuoteChar => _reader.QuoteChar;

  public XmlSpace XmlSpace => _reader.XmlSpace;

  public string XmlLang => _reader.XmlLang;

  public IXmlSchemaInfo? SchemaInfo => _reader.SchemaInfo;

  public Type ValueType => _reader.ValueType;

  public int AttributeCount => _reader.AttributeCount;

  public string this[int i] => _reader[i];

  public string? this[string name] => _reader[name];

  public string? this[string name, string? namespaceURI] => _reader[name, namespaceURI];

  public bool EOF => _reader.EOF;

  public ReadState ReadState => _reader.ReadState;

  public XmlNameTable NameTable => _reader.NameTable;

  public bool CanResolveEntity => _reader.CanResolveEntity;

  public bool CanReadBinaryContent => _reader.CanReadBinaryContent;

  public bool CanReadValueChunk => _reader.CanReadValueChunk;

  public bool HasAttributes => _reader.HasAttributes;

  public object ReadContentAsObject()
  {
    return _reader.ReadContentAsObject();
  }

  public bool ReadContentAsBoolean()
  {
    return _reader.ReadContentAsBoolean();
  }

  public DateTime ReadContentAsDateTime()
  {
    return _reader.ReadContentAsDateTime();
  }

  public DateTimeOffset ReadContentAsDateTimeOffset()
  {
    return _reader.ReadContentAsDateTimeOffset();
  }

  public double ReadContentAsDouble()
  {
    return _reader.ReadContentAsDouble();
  }

  public float ReadContentAsFloat()
  {
    return _reader.ReadContentAsFloat();
  }

  public decimal ReadContentAsDecimal()
  {
    return _reader.ReadContentAsDecimal();
  }

  public int ReadContentAsInt()
  {
    return _reader.ReadContentAsInt();
  }

  public long ReadContentAsLong()
  {
    return _reader.ReadContentAsLong();
  }

  public string ReadContentAsString()
  {
    return _reader.ReadContentAsString();
  }

  public object ReadContentAs(Type returnType, IXmlNamespaceResolver? namespaceResolver)
  {
    return _reader.ReadContentAs(returnType, namespaceResolver);
  }

  public object ReadElementContentAsObject()
  {
    return _reader.ReadElementContentAsObject();
  }

  public object ReadElementContentAsObject(string localName, string namespaceURI)
  {
    return _reader.ReadElementContentAsObject(localName, namespaceURI);
  }

  public bool ReadElementContentAsBoolean()
  {
    return _reader.ReadElementContentAsBoolean();
  }

  public bool ReadElementContentAsBoolean(string localName, string namespaceURI)
  {
    return _reader.ReadElementContentAsBoolean(localName, namespaceURI);
  }

  public DateTime ReadElementContentAsDateTime()
  {
    return _reader.ReadElementContentAsDateTime();
  }

  public DateTime ReadElementContentAsDateTime(string localName, string namespaceURI)
  {
    return _reader.ReadElementContentAsDateTime(localName, namespaceURI);
  }

  public double ReadElementContentAsDouble()
  {
    return _reader.ReadElementContentAsDouble();
  }

  public double ReadElementContentAsDouble(string localName, string namespaceURI)
  {
    return _reader.ReadElementContentAsDouble(localName, namespaceURI);
  }

  public float ReadElementContentAsFloat()
  {
    return _reader.ReadElementContentAsFloat();
  }

  public float ReadElementContentAsFloat(string localName, string namespaceURI)
  {
    return _reader.ReadElementContentAsFloat(localName, namespaceURI);
  }

  public decimal ReadElementContentAsDecimal()
  {
    return _reader.ReadElementContentAsDecimal();
  }

  public decimal ReadElementContentAsDecimal(string localName, string namespaceURI)
  {
    return _reader.ReadElementContentAsDecimal(localName, namespaceURI);
  }

  public int ReadElementContentAsInt()
  {
    return _reader.ReadElementContentAsInt();
  }

  public int ReadElementContentAsInt(string localName, string namespaceURI)
  {
    return _reader.ReadElementContentAsInt(localName, namespaceURI);
  }

  public long ReadElementContentAsLong()
  {
    return _reader.ReadElementContentAsLong();
  }

  public long ReadElementContentAsLong(string localName, string namespaceURI)
  {
    return _reader.ReadElementContentAsLong(localName, namespaceURI);
  }

  public string ReadElementContentAsString()
  {
    return _reader.ReadElementContentAsString();
  }

  public string ReadElementContentAsString(string localName, string namespaceURI)
  {
    return _reader.ReadElementContentAsString(localName, namespaceURI);
  }

  public object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
  {
    return _reader.ReadElementContentAs(returnType, namespaceResolver);
  }

  public object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver, string localName, string namespaceURI)
  {
    return _reader.ReadElementContentAs(returnType, namespaceResolver, localName, namespaceURI);
  }

  public string? GetAttribute(string name)
  {
    return _reader.GetAttribute(name);
  }

  public string? GetAttribute(string name, string? namespaceURI)
  {
    return _reader.GetAttribute(name, namespaceURI);
  }

  public string GetAttribute(int i)
  {
    return _reader.GetAttribute(i);
  }

  public bool MoveToAttribute(string name)
  {
    return _reader.MoveToAttribute(name);
  }

  public bool MoveToAttribute(string name, string? ns)
  {
    return _reader.MoveToAttribute(name, ns);
  }

  public void MoveToAttribute(int i)
  {
    _reader.MoveToAttribute(i);
  }

  public bool MoveToFirstAttribute()
  {
    return _reader.MoveToFirstAttribute();
  }

  public bool MoveToNextAttribute()
  {
    return _reader.MoveToNextAttribute();
  }

  public bool MoveToElement()
  {
    return _reader.MoveToElement();
  }

  public bool ReadAttributeValue()
  {
    return _reader.ReadAttributeValue();
  }

  public bool Read()
  {
    return _reader.Read();
  }

  public void Close()
  {
    _reader.Close();
  }

  public void Skip()
  {
    _reader.Skip();
  }

  public string? LookupNamespace(string prefix)
  {
    return _reader.LookupNamespace(prefix);
  }

  public void ResolveEntity()
  {
    _reader.ResolveEntity();
  }

  public int ReadContentAsBase64(byte[] buffer, int index, int count)
  {
    return _reader.ReadContentAsBase64(buffer, index, count);
  }

  public int ReadElementContentAsBase64(byte[] buffer, int index, int count)
  {
    return _reader.ReadElementContentAsBase64(buffer, index, count);
  }

  public int ReadContentAsBinHex(byte[] buffer, int index, int count)
  {
    return _reader.ReadContentAsBinHex(buffer, index, count);
  }

  public int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
  {
    return _reader.ReadElementContentAsBinHex(buffer, index, count);
  }

  public int ReadValueChunk(char[] buffer, int index, int count)
  {
    return _reader.ReadValueChunk(buffer, index, count);
  }

  public string ReadString()
  {
    return _reader.ReadString();
  }

  public XmlNodeType MoveToContent()
  {
    return _reader.MoveToContent();
  }

  public void ReadStartElement()
  {
    _reader.ReadStartElement();
  }

  public void ReadStartElement(string name)
  {
    _reader.ReadStartElement(name);
  }

  public void ReadStartElement(string localname, string ns)
  {
    _reader.ReadStartElement(localname, ns);
  }

  public string ReadElementString()
  {
    return _reader.ReadElementString();
  }

  public string ReadElementString(string name)
  {
    return _reader.ReadElementString(name);
  }

  public string ReadElementString(string localname, string ns)
  {
    return _reader.ReadElementString(localname, ns);
  }

  public void ReadEndElement()
  {
    _reader.ReadEndElement();
  }

  public bool IsStartElement()
  {
    return _reader.IsStartElement();
  }

  public bool IsStartElement(string name)
  {
    return _reader.IsStartElement(name);
  }

  public bool IsStartElement(string localname, string ns)
  {
    return _reader.IsStartElement(localname, ns);
  }

  public bool ReadToFollowing(string name)
  {
    return _reader.ReadToFollowing(name);
  }

  public bool ReadToFollowing(string localName, string namespaceURI)
  {
    return _reader.ReadToFollowing(localName, namespaceURI);
  }

  public bool ReadToDescendant(string name)
  {
    return _reader.ReadToDescendant(name);
  }

  public bool ReadToDescendant(string localName, string namespaceURI)
  {
    return _reader.ReadToDescendant(localName, namespaceURI);
  }

  public bool ReadToNextSibling(string name)
  {
    return _reader.ReadToNextSibling(name);
  }

  public bool ReadToNextSibling(string localName, string namespaceURI)
  {
    return _reader.ReadToNextSibling(localName, namespaceURI);
  }

  public string ReadInnerXml()
  {
    return _reader.ReadInnerXml();
  }

  public string ReadOuterXml()
  {
    return _reader.ReadOuterXml();
  }

  public XmlReader ReadSubtree()
  {
    return _reader.ReadSubtree();
  }
}
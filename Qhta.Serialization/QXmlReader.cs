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

  public void Close()
  {
    _reader.Close();
  }

  #region Settings
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
  #endregion

  #region Reader state

  public bool EOF => _reader.EOF;

  public ReadState ReadState => _reader.ReadState;

  public XmlNodeType NodeType => _reader.NodeType;

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

  public string LocalName => _reader.LocalName;

  public string NamespaceURI => _reader.NamespaceURI;

  public string Prefix => _reader.Prefix;

  public bool HasValue => _reader.HasValue;

  public Type ValueType => _reader.ValueType;

  public string Value => _reader.Value;

  public int Depth => _reader.Depth;

  public string BaseURI => _reader.BaseURI;

  public bool IsEmptyElement => _reader.IsEmptyElement;

  public bool IsDefault => _reader.IsDefault;

  public char QuoteChar => _reader.QuoteChar;

  public XmlSpace XmlSpace => _reader.XmlSpace;

  public string XmlLang => _reader.XmlLang;

  #endregion

  #region Attribute accessors

  public bool HasAttributes => _reader.HasAttributes;
  
  public int AttributeCount => _reader.AttributeCount;

  public string this[int i] => _reader[i];

  public string? this[string name] => _reader[name];

  public string? this[XmlQualifiedTagName fullName] 
    => _reader[fullName.Name, fullName.XmlNamespace];

  
  public string? GetAttribute(string name)
  {
    return _reader.GetAttribute(name);
  }

  public string? GetAttribute(XmlQualifiedTagName fullName)
  {
    return _reader.GetAttribute(fullName.Name, fullName.XmlNamespace);
  }

  public string GetAttribute(int i)
  {
    return _reader.GetAttribute(i);
  }

  #endregion

  #region Content accessors

  public string ReadContentAsString()
  {
    return _reader.ReadContentAsString();
  }

  public object ReadContentAs(Type returnType)
  {
    return _reader.ReadContentAs(returnType, null);
  }

  public string ReadElementContentAsString()
  {
    return _reader.ReadElementContentAsString();
  }

  public string ReadElementContentAsString(XmlQualifiedTagName fullName)
  {
    return _reader.ReadElementContentAsString(fullName.Name, fullName.XmlNamespace);
  }
  #endregion

  #region Element accessors
  public string ReadElementString()
  {
    return _reader.ReadElementString();
  }

  public string ReadElementString(string name)
  {
    return _reader.ReadElementString(name);
  }

  public string ReadElementString(XmlQualifiedTagName fullName)
  {
    return _reader.ReadElementString(fullName.Name, fullName.XmlNamespace);
  }
  #endregion

  #region Movement
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
  
  public XmlNodeType MoveToContent()
  {
    return _reader.MoveToContent();
  }

  public void Skip()
  {
    _reader.Skip();
  }
  #endregion


  #region Read but return no significant value.

  public void Read()
  {
    _reader.Read();
  }

  public void ReadAttributeValue()
  {
    _reader.ReadAttributeValue();
  }
  #endregion

  #region Start/EndElement checking

  public bool IsStartElement()
  {
    return _reader.IsStartElement();
  }

  public bool IsStartElement(string name)
  {
    return _reader.IsStartElement(name);
  }

  public bool IsStartElement(XmlQualifiedTagName fullName)
  {
    return _reader.IsStartElement(fullName.Name, fullName.XmlNamespace);
  }

  public void ReadStartElement()
  {
    _reader.ReadStartElement();
  }

  public void ReadStartElement(string name)
  {
    _reader.ReadStartElement(name);
  }

  public void ReadStartElement(XmlQualifiedTagName fullName)
  {
    _reader.ReadStartElement(fullName.Name, fullName.XmlNamespace);
  }
  public void ReadEndElement()
  {
    _reader.ReadEndElement();
  }
  #endregion

}
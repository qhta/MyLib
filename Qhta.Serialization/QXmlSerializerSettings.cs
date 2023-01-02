namespace Qhta.Xml.Serialization;

public class QXmlSerializerSettings
{
  public QXmlSerializerSettings(XmlSerializationInfoMapper mapper, SerializationOptions options, 
    XmlWriterSettings writeSettings, XmlReaderSettings readSettings)
  {
    Mapper = mapper;
    Options = options;
    XmlWriterSettings = writeSettings;
    XmlReaderSettings = readSettings;
  }

  public XmlSerializationInfoMapper Mapper { get; private set; }

  public SerializationOptions Options { get; private set; }

  public XmlWriterSettings XmlWriterSettings { get; private set; }

  public XmlReaderSettings XmlReaderSettings { get; private set; }
}
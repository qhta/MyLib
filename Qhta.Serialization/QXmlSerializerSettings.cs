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

  public XmlSerializationInfoMapper Mapper { get; }

  public SerializationOptions Options { get; }

  public XmlWriterSettings XmlWriterSettings { get; }

  public XmlReaderSettings XmlReaderSettings { get; }
}
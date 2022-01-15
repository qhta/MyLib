namespace Qhta.Serialization
{
  public interface IXWriter
  {
    void WriteStartElement(string tag);

    void WriteEndElement(string tag);

    void WriteAttributeString(string attrName, string str);

    void WriteElementString(string tagName, string str);

    void WriteValue(string str);
  }
}

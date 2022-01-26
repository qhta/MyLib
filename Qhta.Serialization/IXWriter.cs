namespace Qhta.Serialization
{
  public interface IXWriter
  {
    void WriteStartElement(string tag);

    void WriteEndElement(string tag);

    void WriteAttributeString(string? prefix, string localName, string? ns, string? value);

    void WriteAttributeString(string localName, string? ns, string? value) => WriteAttributeString(null, localName, ns, value);

    void WriteAttributeString(string attrName, string str) => WriteAttributeString(null, attrName, null, str);

    void WriteElementString(string tagName, string str);

    void WriteValue(string str);

    void WriteSignificantSpaces(bool value);
  }
}

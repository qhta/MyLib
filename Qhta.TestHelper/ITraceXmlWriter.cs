using System;
using System.Runtime.CompilerServices;
#nullable enable

namespace Qhta.TestHelper
{
  public interface ITraceXmlWriter: ITraceWriter
  {
    void WriteStartElement(string tag);

    void WriteEndElement(string tag);

    void WriteAttributeString(string? prefix, string localName, string? ns, string? value);

    void WriteAttributeString(string localName, string? ns, string? value) => WriteAttributeString(null, localName, ns, value);

    void WriteAttributeString(string attrName, string? value) => WriteAttributeString(null, attrName, null, value);

    void WriteElementString(string tagName, string? str);

    void WriteValue(string str);

    void WriteSignificantSpaces(bool value);
  }
}

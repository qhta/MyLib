using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Qhta.Xml.Serialization
{

  /// <summary>
  /// Xml equivalent of JsonConverter.
  /// Reads and writes object from/to XML.
  /// </summary>
  public abstract class XmlConverter
  {
    public virtual bool CanRead => true;

    public virtual bool CanWrite => true;

    public abstract void WriteXml(XmlWriter writer, object? value, XmlSerializer? serializer);

    public abstract object? ReadXml(XmlReader reader, SerializationTypeInfo objectTypeInfo, 
      SerializationPropertyInfo? propertyInfo, SerializationItemTypeInfo? itemInfo, XmlSerializer? serializer);


    public abstract bool CanConvert(Type objectType);
  }
}

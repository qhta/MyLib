using Qhta.TestHelper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Qhta.Xml.Serialization
{
  public class XmlAnyAttributeConverter : XmlConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType.GetInterface("IDictionary") != null && objectType.GetConstructor(new Type[0]) != null;
    }

    public override object? ReadXml(XmlReader reader, Type objectType, object? existingValue, XmlSerializer? serializer)
    {
      if (reader.EOF)
        return null;
      var constructor = objectType.GetConstructor(new Type[0]);

      if (constructor == null)
        throw new InternalException($"Type {objectType.Name} has no parameterless public constructor");
      var dict = constructor.Invoke(new object[0]) as Dictionary<string, string>;
      if (dict == null)
        throw new InternalException($"Type {objectType.Name} must be a Dictionary<string, string>");
      reader.MoveToFirstAttribute();
      for (int i = 0; i < reader.AttributeCount; i++)
      {
        dict.Add(reader.LocalName, reader.Value);
        reader.MoveToNextAttribute();
      }
      reader.Read();
      return dict;
    }

    public override bool CanWrite => false;

    public override void WriteXml(XmlWriter writer, object? value, XmlSerializer? serializer)
    {
      throw new NotImplementedException();
    }
  }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Qhta.TestHelper;

namespace Qhta.Xml.Serialization
{
  public partial class XmlSerializer
  {
    #region Serialize methods
    public void Serialize(Stream stream, object obj)
    {
      using (QXmlWriter xmlWriter = new QXmlWriter(new StreamWriter(stream)))
      {
        Serialize(xmlWriter, obj);
      }
    }


    public void Serialize(TextWriter textWriter, object obj)
    {
      using (QXmlWriter xmlWriter = new QXmlWriter(textWriter))
      {
        Serialize(xmlWriter, obj);
      }
    }

    public string Serialize(object obj)
    {
      using (var stream = new MemoryStream())
      {
        Serialize(stream, obj);
        stream.Flush();
        var bytes = stream.ToArray();
        return Encoding.UTF8.GetString(bytes);
      }
    }

    public void Serialize(IXWriter writer, object obj)
    {
      if (obj is IXSerializable serializableItem)
      {
        serializableItem.Serialize(this, writer);
      }
      else if (obj != null)
      {
        WriteObject(writer, obj);
      }
    }

    public void SerializeObject(IXWriter writer, object obj)
    {
      if (obj is IXSerializable qSerializable)
      {
        qSerializable.Serialize(this, writer);
      }
      //else if (obj is IXmlSerializable xmlSerializable)
      //{
      //  xmlSerializable.WriteXml(writer);
      //}
      else if (obj != null)
      {
        WriteObject(writer, obj);
      }
    }
    #endregion

    #region Write methods
    public void WriteObject(IXWriter writer, object obj)
    {
      var aType = obj.GetType();
      if (!KnownTypes.TryGetValue(aType, out var serializedTypeInfo))
        throw new InternalException($"Type \"{aType}\" not registered");
      var tag = KnownTypes.KnownTags(serializedTypeInfo.Type).FirstOrDefault();
      if (tag != null)
        writer.WriteStartElement(tag);
      WriteAttributesBase(writer, obj);
      WritePropertiesBase(writer, tag, obj);
      if (obj is ICollection collection)
        WriteCollectionBase(writer, tag, null, collection);
      if (tag != null)
        writer.WriteEndElement(tag);
    }

    public void WriteObjectInterior(IXWriter writer, string? tag, object obj)
    {
      WriteAttributesBase(writer, obj);
      WritePropertiesBase(writer, tag, obj);
      if (obj is ICollection collection)
        WriteCollectionBase(writer, tag, null, collection);
    }
    public int WriteAttributesBase(IXWriter writer, object obj)
    {
      var aType = obj.GetType();
      if (!KnownTypes.TryGetValue(aType, out var typeInfo))
        throw new InternalException($"Unknown type \"{aType.Name}\" while serializing object attributes");
      var propList = typeInfo.PropsAsAttributes;
      int attrsWritten = 0;
      foreach (var item in propList)
      {
        var propInfo = item.PropInfo;
        string? attrName = item.Name;
        var propValue = propInfo.GetValue(obj);
        if (propValue != null)
        {
          var defaultValue =
            (propInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false).FirstOrDefault() as DefaultValueAttribute)?.Value;
          if (defaultValue == null || !propValue.Equals(defaultValue))
          {
            string? str = propValue as string;
            if (str == null)
            {
              if (propValue is bool || propValue.GetType() == typeof(bool?))
                str = ((bool)propValue).ToString().ToLower();
              else if (propValue is int || propValue.GetType() == typeof(int?))
                str = ((int)propValue).ToString();
            }
            if (str != null && attrName != null)
            {
              writer.WriteAttributeString(attrName, str);
              attrsWritten++;
            }
          }
        }
      }
      return attrsWritten;
    }

    public int WritePropertiesBase(IXWriter writer, string? elementTag, object obj)
    {
      var aType = obj.GetType();
      if (!KnownTypes.TryGetValue(aType, out var typeInfo))
        throw new InternalException($"Unknown type \"{aType.Name}\" while serializing object attributes");
      var attribs = typeInfo.PropsAsAttributes;
      var props = typeInfo.PropsAsElements;

      int propsWritten = 0;
      foreach (var item in attribs)
      {
        var propInfo = item.PropInfo;
        string? propTag = item.Name;
        if (elementTag != null)
          if (Options?.PrecedePropertyNameWithElementName == true)
            propTag = elementTag + "." + propTag;
        var propValue = propInfo.GetValue(obj);
        if (propValue != null && !IsSimple(propValue))
        {
          WriteStartElement(writer, propTag);
          if (propValue is IXSerializable serializableValue)
            serializableValue.Serialize(this, writer);
          else
            WriteValue(writer, propValue?.ToString());
          WriteEndElement(writer, propTag);
          propsWritten++;
        }
      }

      foreach (var prop in props)
      {
        var propInfo = prop.PropInfo;
        string propTag = prop.Name;
        if (Options?.PrecedePropertyNameWithElementName == true)
          propTag = elementTag + "." + propTag;
        var propValue = propInfo.GetValue(obj);
        if (propValue != null)
        {
          if (!String.IsNullOrEmpty(propTag))
            WriteStartElement(writer, propTag);
          if (propValue is IXSerializable serializableValue)
            serializableValue.Serialize(this, writer);
          else if (propValue is ICollection collection)
          {
            var arrayInfo = prop as SerializationArrayInfo;
            foreach (var arrayItem in collection)
            {
              if (arrayItem != null)
              {
                var itemType = arrayItem.GetType();
                SerializationTypeInfo? itemTypeInfo = null;
                string? itemName = null;
                if (arrayInfo != null)
                {

                  var itemTypeInfoPair = arrayInfo.KnownItemTypes.FirstOrDefault(item => itemType == item.Type);
                  if (itemTypeInfoPair == null)
                    itemTypeInfoPair = arrayInfo.KnownItemTypes.FirstOrDefault(item => itemType.IsSubclassOf(item.Type));
                  if (itemTypeInfoPair != null)
                  {
                    itemTypeInfo = itemTypeInfoPair.TypeInfo;
                    itemName = itemTypeInfoPair.ElementName;
                  }
                }
                if (itemName == null)
                  itemName = arrayItem.GetType().Name;
                WriteStartElement(writer, itemName);
                if (arrayItem.GetType().Name == "SwitchCase")
                  TestUtils.Stop();
                WriteObjectInterior(writer, itemName, arrayItem);
                WriteEndElement(writer, itemName);
              }
            }
          }
          else if (propValue != null)
            WriteValue(writer, propValue.ToString());
          if (!String.IsNullOrEmpty(propTag))
            WriteEndElement(writer, propTag);
          propsWritten++;
        }
      }
      return propsWritten;
    }


    public int WriteCollectionBase(IXWriter writer, string? elementTag, string? propTag, ICollection collection, KnownTypesDictionary? itemTypes = null)
    {
      int itemsWritten = 0;
      if (collection != null && collection.Count > 0)
      {
        if (itemTypes == null)
        {
          var colType = collection.GetType();
          if (KnownTypes.TryGetValue(colType, out var colTypeInfo))
            itemTypes = colTypeInfo.KnownItems;
        }
        if (propTag != null && elementTag != null)
        {
          if (Options?.PrecedePropertyNameWithElementName == true)
            propTag = elementTag + "." + propTag;
        }

        if (!String.IsNullOrEmpty(propTag))
          writer.WriteStartElement(propTag);

        foreach (var item in collection)
        {
          if (item != null)
          {
            var itemTag = Options?.ItemTag ?? "item";
            //if (IsSimpleValue(item))
            //{
            //  if (aTag != null)
            //    WriteStartElement(aTag);
            //  WriteObject(item);
            //  if (aTag != null)
            //    WriteEndElement(aTag);
            //}
            //else if (item is IXSerializable serializableItem)
            //  serializableItem.Serialize(this);
            //else 
            if (item.GetType().Name.StartsWith("KeyValuePair`"))
            {
              var keyProp = item.GetType().GetProperty("Key");
              var valProp = item.GetType().GetProperty("Value");
              if (keyProp != null && valProp != null)
              {
                var key = keyProp.GetValue(item);
                var val = valProp.GetValue(item);
                if (key != null)
                {
                  WriteStartElement(writer, itemTag);
                  WriteAttributeString(writer, "key", key.ToString() ?? "");
                  //if (val is string strVal)
                  //  WriteValue(strVal);
                  //else 
                  if (val != null)
                    SerializeObject(writer, val);
                  WriteEndElement(writer, itemTag);
                }
              }
            }
            else
            {
              TypeConverter? typeConverter = null;
              if (itemTypes != null)
              {
                var itemType = item.GetType();
                if (!itemTypes.TryGetValue(itemType, out var itemTypeInfo))
                {
                  itemTypeInfo = itemTypes.FirstOrDefault(item => itemType.IsSubclassOf(item.Type));
                }
                if (itemTypeInfo != null)
                {
                  itemTag = itemTypes.KnownTags(itemTypeInfo.Type).FirstOrDefault();
                  if (itemTag == itemTypeInfo.ElementName)
                    itemTag = null;
                  typeConverter = itemTypeInfo.TypeConverter;
                }
              }
              if (!string.IsNullOrEmpty(itemTag))
                WriteStartElement(writer, itemTag);
              if (typeConverter != null)
                WriteValue(writer, typeConverter.ConvertToString(item) ?? "");
              else
                SerializeObject(writer, item);
              if (!string.IsNullOrEmpty(itemTag))
                WriteEndElement(writer, itemTag);
            }
            itemsWritten++;
          }
        }
        if (!String.IsNullOrEmpty(propTag))
          writer.WriteEndElement(propTag);
      }
      return itemsWritten;
    }

    public void WriteStartElement(IXWriter writer, string propTag)
    {
      writer.WriteStartElement(propTag);
    }

    public void WriteEndElement(IXWriter writer, string propTag)
    {
      writer.WriteEndElement(propTag);
    }

    public void WriteAttributeString(IXWriter writer, string attrName, string valStr)
    {
      writer.WriteAttributeString(attrName, valStr);
    }

    public void WriteValue(IXWriter writer, object? value)
    {
      if (value is string valStr)
        writer.WriteValue(valStr);
      else if (value is bool boolValue)
        writer.WriteValue(boolValue.ToString().ToLower());
      else if (value is int intValue)
        writer.WriteValue(intValue.ToString().ToLower());
      else if (value is IXSerializable qSerializable)
        qSerializable.Serialize(this, writer);
    }

    //public void WriteValue(IXWriter writer, string str)
    //{
    //  if (str.StartsWith(' ') || str.EndsWith(' '))
    //    writer.WriteSignificantSpaces(true);
    //  writer.WriteValue(str);
    //}

    public void WriteValue(IXWriter writer, string propTag, object value)
    {
      WriteStartElement(writer, propTag);
      WriteValue(writer, value);
      WriteEndElement(writer, propTag);
    }

    public string EncodeStringValue(string str)
    {
      var sb = new StringBuilder();
      foreach (var ch in str)
      {
        sb.Append(ch switch
        {
          '\\' => "\\\\",
          '\t' => "\\t",
          '\r' => "\\r",
          '\n' => "\\n",
          '\xA0' => "\\s",
          _ => ch
        });
      }
      return sb.ToString();
    }
    #endregion

  }
}

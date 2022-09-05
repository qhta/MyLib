//using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Qhta.TestHelper;
using Qhta.TypeUtils;


namespace Qhta.Xml.Serialization;

public partial class QXmlSerializer
{
  private const string xsi = @"http://www.w3.org/2001/XMLSchema-instance";

  public Dictionary<string, string> Namespaces { get; } = new Dictionary<string, string>();

  public XmlWriterSettings XmlWriterSettings { get; set; } = new XmlWriterSettings
  { Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates };

  #region Serialize methods

  protected partial void SerializeObject(XmlWriter xmlWriter, object? obj, XmlSerializerNamespaces? namespaces, string? encodingStyle, string? id)
  {
    SerializeObject(xmlWriter, obj);
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


  public void SerializeObject(XmlWriter writer, object? obj)
  {
    //if (obj is IXSerializable qSerializable)
    //{
    //  qSerializable.Serialize(this, writer);
    //}
    //else
    if (obj is IXmlSerializable xmlSerializable)
    {
      xmlSerializable.WriteXml(writer);
    }
    else if (obj != null)
    {
      WriteObject(writer, obj);
    }
  }
  #endregion

  #region Write methods

  public void WriteObject(XmlWriter writer, object obj)
  {
    var aType = obj.GetType();
    if (!KnownTypes.TryGetValue(aType, out var serializedTypeInfo))
      throw new InternalException($"Type \"{aType}\" not registered");
    var tag = SerializationInfoMapper.GetXmlQualifiedName(serializedTypeInfo.Name);
    writer.WriteStartElement(tag.Name, tag.Namespace);
    if (Options.UseNilValue && !Namespaces.ContainsKey("xsi"))
    {
      Namespaces.Add("xsi", xsi);
      writer.WriteAttributeString("xmlns", "xsi", null, xsi);
    }

    WriteObjectInterior(writer, obj, null, serializedTypeInfo);
    writer.WriteEndElement();
  }

  public void WriteObjectInterior(XmlWriter writer, object obj, string? tag = null, SerializationTypeInfo? typeInfo = null)
  {
    if (typeInfo == null)
    {
      var aType = obj.GetType();
      if (!KnownTypes.TryGetValue(aType, out typeInfo))
        throw new InternalException($"Type \"{aType}\" not registered");
    }

    WriteAttributesBase(writer, obj, typeInfo);
    WritePropertiesBase(writer, tag, obj, typeInfo);
    WriteCollectionBase(writer, tag, null, obj, typeInfo);
  }

  public int WriteAttributesBase(XmlWriter writer, object obj, SerializationTypeInfo typeInfo)
  {
    var aType = obj.GetType();
    var propList = typeInfo.PropsAsAttributes.OrderBy(item => item.Order).ToList();
    int attrsWritten = 0;
    foreach (var item in propList)
    {
      if (item.ShouldSerializeMethod != null)
      {
        var shouldSerializeProperty = item.ShouldSerializeMethod.Invoke(new object[] { obj }, new object[0]);
        if (shouldSerializeProperty is bool shouldSerialize)
          if (!shouldSerialize)
            continue;
      }

      var propInfo = item.PropInfo;
      string? attrName = item.Name;
      var propValue = propInfo.GetValue(obj);
      if (propValue != null)
      {
        var defaultValue =
          (propInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false).FirstOrDefault() as DefaultValueAttribute)?.Value;
        if (defaultValue != null && propValue.Equals(defaultValue))
          continue;
        string? str = GetValueString(propValue);
        if (str != null)
        {
          writer.WriteAttributeString(attrName, str);
          attrsWritten++;
        }
      }
    }
    return attrsWritten;
  }

  public int WritePropertiesBase(XmlWriter writer, string? elementTag, object obj, SerializationTypeInfo typeInfo)
  {
    var props = typeInfo.PropsAsElements;

    int propsWritten = 0;

    foreach (var prop in props)
    {
      if (prop.ShouldSerializeMethod != null)
      {
        var shouldSerializeProperty = prop.ShouldSerializeMethod.Invoke(obj, new object[0]);
        if (shouldSerializeProperty is bool shouldSerialize)
          if (!shouldSerialize)
            continue;
      }

      var propInfo = prop.PropInfo;
      string propTag = prop.Name;

      if (Options?.PrecedePropertyNameWithClassName == true)
        propTag = elementTag + "." + propTag;
      var propValue = propInfo.GetValue(obj);
      if (propValue == null)
      {
        if (Options?.UseNilValue == true && prop.IsNullable == true)
        {
          if (!String.IsNullOrEmpty(propTag))
            WriteStartElement(writer, propTag);
          writer.WriteAttributeString(null, "nil", xsi, "true");
          if (!String.IsNullOrEmpty(propTag))
            WriteEndElement(writer, propTag);
        }
      }
      else
      {
        var defaultValue =
          (propInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false).FirstOrDefault() as
            DefaultValueAttribute)?.Value;
        if (defaultValue != null)
        {
          if (propValue.Equals(defaultValue))
            continue;
          if (defaultValue is int iv && iv == 0 && (int)propValue == 0)
            continue;
        }

        if (!String.IsNullOrEmpty(propTag))
          WriteStartElement(writer, propTag);
        //if (propValue is IXSerializable serializableValue)
        //  serializableValue.Serialize(this, writer);
        //else
        {
          var pType = propValue.GetType();
          if (pType.IsSimple())
          {
            WriteValue(writer, GetValueString(propValue));
          }
          else
          if (KnownTypes.TryGetValue(pType, out var serializedTypeInfo))
          {
            if (prop.IsReference)
              WriteValue(writer, GetValueString(propValue));
            else
              WriteObjectInterior(writer, propValue, null, serializedTypeInfo);
          }
          //else if (propValue is ICollection collection)
          //{
          //  var arrayInfo = prop.CollectionInfo;
          //  foreach (var arrayItem in collection)
          //  {
          //    if (arrayItem != null)
          //    {
          //      var itemType = arrayItem.GetType();
          //      //SerializationTypeInfo? itemTypeInfo;
          //      string? itemName = null;
          //      if (arrayInfo != null)
          //      {

          //        var itemTypeInfoPair = arrayInfo.KnownItemTypes.FindTypeInfo(itemType);
          //        if (itemTypeInfoPair != null)
          //        {
          //          //itemTypeInfo = itemTypeInfoPair.TypeInfo;
          //          itemName = itemTypeInfoPair.ElementName;
          //        }
          //      }

          //      if (itemName == null)
          //        itemName = arrayItem.GetType().Name;
          //      WriteStartElement(writer, itemName);
          //      if (arrayItem.GetType().Name == "SwitchCase")
          //        TestUtils.Stop();
          //      WriteObjectInterior(writer, itemName, arrayItem);
          //      WriteEndElement(writer, itemName);
          //    }
          //  }
          //}
          else
          {
            WriteValue(writer, GetValueString(propValue));
          }
        }
        if (!String.IsNullOrEmpty(propTag))
          WriteEndElement(writer, propTag);
        propsWritten++;
      }

    }
    return propsWritten;
  }

  public int WriteCollectionBase(XmlWriter writer, string? elementTag, string? propTag, Object obj, SerializationTypeInfo typeInfo)
  {
    if (typeInfo.CollectionInfo != null)
      return WriteCollectionBase(writer, elementTag, propTag, obj, typeInfo.CollectionInfo);
    return 0;
  }

  public int WriteCollectionBase(XmlWriter writer, string? elementTag, string? propTag, Object obj, CollectionInfo collectionInfo)
  {
    var itemsWritten = 0;
    var collection = obj as IEnumerable;
    if (collection == null)
      return 0;
    var itemTypes = collectionInfo.KnownItemTypes;
    if (propTag != null && elementTag != null)
    {
      if (Options?.PrecedePropertyNameWithClassName == true)
        propTag = elementTag + "." + propTag;
    }

    if (!String.IsNullOrEmpty(propTag))
      writer.WriteStartElement(propTag);
    foreach (var item in collection)
    {
      if (item != null)
      {
        var itemTag = Options?.ItemTag ?? "item";
        if (item.GetType().IsSimple())
        {
          itemTag = GetItemTag(item.GetType());
          WriteStartElement(writer, itemTag);
          WriteValue(writer, item);
          WriteEndElement(writer, itemTag);
        }
        //else if (item is IXSerializable serializableItem)
        //  serializableItem.Serialize(this);
        else
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
              if (collectionInfo is DictionaryInfo dictionaryInfo && dictionaryInfo.KeyProperty!=null && val!=null 
                && KnownTypes.TryGetValue(val.GetType(), out var serializationTypeInfo))
              {
                WriteObject(writer, val);
              }
              else
              {
                WriteStartElement(writer, itemTag);
                WriteAttributeString(writer, "key", key.ToString() ?? "");
                if (val != null)
                  WriteObjectInterior(writer, val);
                WriteEndElement(writer, itemTag);
              }
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
              itemTypeInfo = itemTypes.FindTypeInfo(itemType);
            }
            if (itemTypeInfo != null)
            {
              typeConverter = itemTypeInfo.TypeInfo.TypeConverter;
            }
          }

          if (typeConverter != null)
          {
            if (!string.IsNullOrEmpty(itemTag))
              WriteStartElement(writer, itemTag);
            WriteValue(writer, typeConverter.ConvertToString(item) ?? "");
            if (!string.IsNullOrEmpty(itemTag))
              WriteEndElement(writer, itemTag);
          }
          else
          {
            if (collectionInfo.IsReferences)
            {
              if (string.IsNullOrEmpty(itemTag))
                WriteValue(writer, item.ToString());
              else
              {
                WriteStartElement(writer, itemTag);
                WriteValue(writer, item.ToString());
                WriteEndElement(writer, itemTag);
              }
            }
            if (KnownTypes.TryGetValue(item.GetType(), out var serializationTypeInfo))
              WriteObject(writer, item);
            else
            {
              WriteStartElement(writer, itemTag);
              WriteObjectInterior(writer, item, itemTag, null);
              WriteEndElement(writer, itemTag);
            }
          }
        }
        itemsWritten++;
      }

      if (!String.IsNullOrEmpty(propTag))
        writer.WriteEndElement();
    }

    writer.Flush();
    return itemsWritten;
  }

  public void WriteStartElement(XmlWriter writer, string propTag)
  {
    writer.WriteStartElement(propTag);
  }

  public void WriteEndElement(XmlWriter writer, string propTag)
  {
    writer.WriteEndElement();
  }

  public void WriteAttributeString(XmlWriter writer, string attrName, string valStr)
  {
    writer.WriteAttributeString(attrName, valStr);
  }

  public void WriteValue(XmlWriter writer, object? value)
  {
    if (value is IXmlSerializable xmlSerializable)
      xmlSerializable.WriteXml(writer);
    else
    {
      var valStr = GetValueString(value);
      if (valStr != null)
        writer.WriteValue(valStr);
    }
  }

  //public void WriteValue(XmlWriter writer, string str)
  //{
  //  if (str.StartsWith(' ') || str.EndsWith(' '))
  //    writer.WriteSignificantSpaces(true);
  //  writer.WriteValue(str);
  //}

  public void WriteValue(XmlWriter writer, string propTag, object value)
  {
    WriteStartElement(writer, propTag);
    WriteValue(writer, value);
    WriteEndElement(writer, propTag);
  }

  protected string GetItemTag(Type aType)
  {
    return aType.Name.ToLowerInvariant();
  }

  protected string? GetValueString(object? propValue)
  {
    if (propValue == null)
      return null;
    if (propValue is string str)
      return EncodeStringValue(str);
    if (propValue is bool || propValue is bool?)
      return ((bool)propValue) ? Options.TrueString : Options.FalseString;
    if (propValue is float || propValue is float?)
      return ((float)propValue).ToString(CultureInfo.InvariantCulture);
    if (propValue is double || propValue is double?)
      return ((double)propValue).ToString(CultureInfo.InvariantCulture);
    if (propValue is decimal || propValue is decimal?)
      return ((decimal)propValue).ToString(CultureInfo.InvariantCulture);
    if (propValue is DateTime || propValue is DateTime?)
      return ((DateTime)propValue).ToString(Options.DateTimeFormat, CultureInfo.InvariantCulture);
    return propValue.ToString() ?? "";
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
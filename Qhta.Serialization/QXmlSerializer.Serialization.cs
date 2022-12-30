//using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Qhta.Conversion;
using Qhta.TestHelper;
using Qhta.TypeUtils;


namespace Qhta.Xml.Serialization;

public partial class QXmlSerializer
{
  public XmlWriterSettings XmlWriterSettings { get; set; } = new XmlWriterSettings
  { Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates };

  #region Serialize methods

  /// <summary>
  /// Main serialization entry
  /// </summary>
  /// <param name="xmlWriter"></param>
  /// <param name="obj"></param>
  /// <param name="namespaces"></param>
  /// <param name="encodingStyle">Style of Soap encoding. Unused</param>
  /// <param name="id">Base for Soap identifiers. Unused</param>
  protected partial void SerializeObject(XmlWriter xmlWriter, object? obj, XmlSerializerNamespaces? namespaces, string? encodingStyle, string? id)
  {
    if (obj == null)
      return;
    if (namespaces != null)
      Namespaces = namespaces;
    WriteObject(xmlWriter, obj, true);
    xmlWriter.Flush();
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


  #endregion

  #region Write methods

  public void WriteObject(XmlWriter writer, object obj, bool emitNamespaces = false)
  {
    if (obj is IXmlSerializable xmlSerializable)
    {
      xmlSerializable.WriteXml(writer);
    }
    else
    {
      var aType = obj.GetType();
      if (!KnownTypes.TryGetValue(aType, out var serializedTypeInfo))
        throw new InternalException($"Type \"{aType}\" not registered");
      var tag = Mapper.GetXmlTag(serializedTypeInfo);
      //if (emitNamespaces && writer is XmlTextWriter xmlTextWriter)
      //{
      //  xmlTextWriter.Namespaces = true;
      //}
      if (DefaultNamespace != null)
        writer.WriteStartElement(tag.Name, DefaultNamespace);
      else
        writer.WriteStartElement(tag.Name, tag.XmlNamespace);
      if (emitNamespaces)
      {
        if (Options.UseNilValue)
        {
          //Namespaces.Add("xsi", xsiNamespace);
          writer.WriteAttributeString("xmlns", "xsi", null, xsiNamespace);
        }

        if (Options.UseXsdScheme)
        {
          //Namespaces.Add("xsd", xsdNamespace);
          writer.WriteAttributeString("xmlns", "xsd", null, xsdNamespace);
        }
        emitNamespaces = false;
      }

      WriteObjectInterior(writer, obj, null, serializedTypeInfo);
      writer.WriteEndElement();
    }
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
    WriteCollectionBase(writer, tag, null, obj as IEnumerable, typeInfo);
  }

  public int WriteAttributesBase(XmlWriter writer, object obj, SerializationTypeInfo typeInfo)
  {
    var aType = obj.GetType();
    var propList = typeInfo.MembersAsAttributes;
    int attrsWritten = 0;
    foreach (var memberInfo in propList)
    {
      if (memberInfo.CheckMethod != null)
      {
        var shouldSerializeProperty = memberInfo.CheckMethod.Invoke(new object[] { obj }, new object[0]);
        if (shouldSerializeProperty is bool shouldSerialize)
          if (!shouldSerialize)
            continue;
      }

      var attrName = memberInfo.XmlName;
      var attrTag = Mapper.GetXmlTag(memberInfo);
      var propValue = memberInfo.GetValue(obj);
      if (propValue != null)
      {
        var defaultValue = memberInfo.DefaultValue;
        if (defaultValue != null && propValue.Equals(defaultValue))
          continue;
        string? str = ConvertMemberValueToString(memberInfo, propValue);
        if (str != null)
        {
          writer.WriteAttributeString(attrTag.Prefix, attrTag.Name, attrTag.XmlNamespace, str);
          attrsWritten++;
        }
      }
    }
    return attrsWritten;
  }

  public int WritePropertiesBase(XmlWriter writer, string? elementTag, object obj, SerializationTypeInfo typeInfo)
  {
    var props = typeInfo.MembersAsElements;

    int propsWritten = 0;

    foreach (var memberInfo in props)
    {
      if (memberInfo.CheckMethod != null)
      {
        var shouldSerializeProperty = memberInfo.CheckMethod.Invoke(obj, new object[0]);
        if (shouldSerializeProperty is bool shouldSerialize)
          if (!shouldSerialize)
            continue;
      }

      var propTag = memberInfo.XmlName;

      if (Options?.PrecedePropertyNameWithClassName == true)
        propTag = elementTag + "." + propTag;
      var propValue = memberInfo.GetValue(obj);
      propValue = memberInfo.GetTypeConverter()?.ConvertToInvariantString(propValue) ?? propValue;
      if (propValue == null)
      {
        if (Options?.UseNilValue == true && memberInfo.IsNullable == true)
        {
          if (!String.IsNullOrEmpty(propTag))
            WriteStartElement(writer, propTag);
          writer.WriteAttributeString(null, "nil", xsiNamespace, "true");
          if (!String.IsNullOrEmpty(propTag))
            WriteEndElement(writer, propTag);
        }
      }
      else
      {
        var defaultValue = memberInfo.DefaultValue;
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
          //if (typeInfo?.XmlName == "HeadingPairs")
          //  Debug.Assert(true);
          var pType = propValue.GetType();
          KnownTypes.TryGetValue(pType, out var serializedTypeInfo);
          if (memberInfo.CollectionInfo != null)
          {
            WriteCollectionBase(writer, null, null, propValue as IEnumerable, memberInfo.ValueType);
          }
          else
          if (memberInfo.ValueType?.CollectionInfo != null)
          {
            WriteCollectionBase(writer, null, null, propValue as IEnumerable, memberInfo.ValueType);
          }
          else
          if (pType.IsSimple())
          {
            if (memberInfo.TypeConverter != null)
              WriteValue(writer, ConvertMemberValueToString(memberInfo, propValue));
            else if (propValue is String str)
              WriteValue(writer, str);
            else
              WriteValue(writer, propValue.ToString());
          }
          else
          if (pType.IsArray(out var itemType) && itemType == typeof(byte))
          {
            WriteValue(writer, ConvertMemberValueToString(memberInfo, propValue));
          }
          else
          if (memberInfo.IsReference)
            WriteValue(writer, ConvertMemberValueToString(memberInfo, propValue));
          else
            WriteObjectInterior(writer, propValue, null, serializedTypeInfo);

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
          //else
          //{
          //  WriteValue(writer, ConvertMemberValueToString(memberInfo, propValue));
          //}
        }
        if (!String.IsNullOrEmpty(propTag))
          WriteEndElement(writer, propTag);
        propsWritten++;
      }

    }
    return propsWritten;
  }

  public int WriteCollectionBase(XmlWriter writer, string? elementTag, string? propTag, IEnumerable? collection, SerializationTypeInfo? typeInfo)
  {
    if (typeInfo?.CollectionInfo != null)
      return WriteCollectionBase(writer, elementTag, propTag, collection, typeInfo.CollectionInfo);
    return 0;
  }

  public int WriteCollectionBase(XmlWriter writer, string? elementTag, string? propTag, IEnumerable? collection, CollectionInfo collectionInfo)
  {
    var itemsWritten = 0;
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
//        var itemTag = Options?.ItemTag ?? "item";
        var itemTag = GetItemTag(item.GetType());
        TypeConverter? typeConverter = null;
        if (itemTypes != null)
        {
          var itemType = item.GetType();
          if (!itemTypes.TryGetValue(itemType, out var itemTypeInfo))
            itemTypeInfo = itemTypes.FindTypeInfo(itemType);

          if (itemTypeInfo != null)
          {
            itemTag = itemTypeInfo.XmlName;
            typeConverter = itemTypeInfo.TypeInfo.TypeConverter;
          }
        }

        if (item.GetType().IsSimple())
        {
          //itemTag = GetItemTag(item.GetType());
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
              if (collectionInfo is DictionaryInfo dictionaryInfo && dictionaryInfo.KeyProperty != null && val != null
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
            if (collectionInfo.StoresReferences)
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
      else
      {
        WriteStartElement(writer, "null");
        WriteEndElement(writer, "null");
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
      if (value != null)
      {
        var typeConverter = new ValueTypeConverter(value.GetType(), null, null, null, Options.ConversionOptions);
        var valStr = typeConverter.ConvertToInvariantString(value);
        if (valStr != null)
        {
          valStr = EncodeStringValue(valStr);
          writer.WriteValue(valStr);
        }
      }
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

  protected string? ConvertMemberValueToString(SerializationMemberInfo memberInfo, object? propValue)
  {
    if (propValue == null)
      return null;
    if (memberInfo.Property == null)
      return null;
    var typeConverter = memberInfo.GetTypeConverter();
    if (typeConverter != null)
    {
      //typeConverter = new ValueTypeConverter(memberInfo.Property.PropertyType, memberInfo.DataType,
      //  memberInfo.Format, memberInfo.Culture, memberInfo.ConversionOptions ?? Options.ConversionOptions);
      var str = typeConverter.ConvertToInvariantString(propValue);
      return str;
    }
    else
    {
      var str = Convert.ToString(propValue);
      return str;
    }
    //if (propValue is string str)
    //  return EncodeStringValue(str);
    //if (propValue is bool || propValue is bool?)
    //  return ((bool)propValue) ? Options.TrueString : Options.FalseString;
    //if (propValue is DateTime || propValue is DateTime?)
    //  return ((DateTime)propValue).ToString(Options.DateTimeFormat, CultureInfo.InvariantCulture);
  }


  public string EncodeStringValue(string str)
  {
    var sb = new StringBuilder();
    foreach (var ch in str)
    {
      if (ch >= ' ' && ch < '\x7f')
        sb.Append(ch);
      else
      {
        var ctx = Char.GetUnicodeCategory(ch);
        switch (ctx)
        {
          case UnicodeCategory.NonSpacingMark:
          //case UnicodeCategory.SpacingCombiningMark:
          //case UnicodeCategory.EnclosingMark:
          case UnicodeCategory.SpaceSeparator:
          case UnicodeCategory.LineSeparator:
          case UnicodeCategory.ParagraphSeparator:
          case UnicodeCategory.Control:
          case UnicodeCategory.Format:
          case UnicodeCategory.Surrogate:
          case UnicodeCategory.PrivateUse:
          //case UnicodeCategory.ConnectorPunctuation:
          //case UnicodeCategory.ClosePunctuation:
          //case UnicodeCategory.OtherPunctuation:
          //case UnicodeCategory.ModifierSymbol:
          case UnicodeCategory.OtherNotAssigned:
            sb.Append(EncodeCharValue(ch));
            break;
          default:
            sb.Append(ch);
            break;
        }
      }
    }
    return sb.ToString();
  }

  public string EncodeCharValue(Char ch)
  {
    switch (ch)
    {
      case '\\':
        return "\\\\";
      case '\t':
        return "\\t";
      case '\r':
        return "\\r";
      case '\n':
        return "\\n";
      case '\xA0':
        return "\\s";
      default:
        return "\\u" + ((UInt16)ch).ToString("X4");
    }
  }

  public string DecodeStringValue(string str)
  {
    var sb = new StringBuilder();
    for (int i = 0; i < str.Length; i++)
    {
      var ch = str[i];
      if (ch == '\\' && i < str.Length - 1)
      {
        i++;
        sb.Append(DecodeEscapeSeq(str, ref i));
      }
      else
        sb.Append(ch);
    }
    return sb.ToString();
  }

  public string DecodeEscapeSeq(string str, ref int index)
  {
    var ch = str[index];
    switch (ch)
    {
      case '\\':
        index++;
        return "\\";
      case 't':
        index++;
        return "\t";
      case 'r':
        index++;
        return "\r";
      case 'n':
        index++;
        return "\n";
      case 's':
        index++;
        return "\xA0";
      case 'u':
        index++;
        UInt16 code = 0;
        for (int i = 0; i < 4; i++)
        {
          char ch1 = str[index+i];
          if (ch1 >= '0' && ch1 <= '9')
            code = (UInt16)(code * 16 + (UInt16)(ch1 - '0'));
          else
          if (ch1 >= 'A' && ch1 <= 'F')
            code = (UInt16)(code * 16 + (UInt16)(ch1 - 'A')+10);
          else
          if (ch1 >= 'a' && ch1 <= 'f')
            code = (UInt16)(code * 16 + (UInt16)(ch1 - 'a') + 10);
        }
        index += 4;
        return new string((char)code,1);
      default:
        index++;
        return new string(ch,1);
    }
  }
  #endregion

}
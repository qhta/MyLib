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

namespace Qhta.Serialization
{
  public class XmlSerializer : BaseSerializer
  {
    #region Creation methods
    public XmlSerializer()
    {
    }
    public XmlSerializer(Type type) : this(type, null, null) { }

    public XmlSerializer(Type type, Type[]? extraTypes) : this(type, extraTypes, null) { }

    public XmlSerializer(Type type, SerializationOptions? options) : this(type, null, options) { }

    public XmlSerializer(Type type, Type[]? extraTypes, SerializationOptions? options)
    {
      if (options != null)
        Options = options;
      AddKnownType(type, "");
      if (extraTypes != null)
        foreach (Type t in extraTypes)
          AddKnownType(t, "");
    }
    #endregion

    #region Serialize methods
    public override void Serialize(Stream stream, object obj)
    {
      using (QXmlWriter xmlWriter = new QXmlWriter(new StreamWriter(stream)))
      {
        Serialize(xmlWriter, obj);
      }
    }


    public override void Serialize(TextWriter textWriter, object obj)
    {
      using (QXmlWriter xmlWriter = new QXmlWriter(textWriter))
      {
        Serialize(xmlWriter, obj);
      }
    }

    public override string Serialize(object obj)
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
    public override void WriteObject(IXWriter writer, object obj)
    {
      var aType = obj.GetType();
      if (!KnownTypes.TryGetValue(aType, out var serializedTypeInfo))
        throw new InternalException($"Type \"{aType}\" not registered");
      var tag = serializedTypeInfo.ElementName;
      writer.WriteStartElement(tag);
      WriteAttributesBase(writer, obj);
      WritePropertiesBase(writer, tag, obj);
      if (obj is ICollection collection)
        WriteCollectionBase(writer, tag, null, collection);
      writer.WriteEndElement(tag);
    }

    public override void WriteObjectInterior(IXWriter writer, string? tag, object obj)
    {
      WriteAttributesBase(writer, obj);
      WritePropertiesBase(writer, tag, obj);
      if (obj is ICollection collection)
        WriteCollectionBase(writer, tag, null, collection);
    }
    public override int WriteAttributesBase(IXWriter writer, object obj)
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

    public override int WritePropertiesBase(IXWriter writer, string? elementTag, object obj)
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
                  itemTypeInfo = arrayInfo.Items.FirstOrDefault(item => itemType == item.Value.Type).Value;
                  if (itemTypeInfo == null)
                    itemTypeInfo = arrayInfo.Items.FirstOrDefault(item => itemType.IsSubclassOf(item.Value.Type)).Value;
                  if (itemTypeInfo != null)
                    itemName = itemTypeInfo.ElementName;
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


    public override int WriteCollectionBase(IXWriter writer, string? elementTag, string? propTag, ICollection collection, KnownTypesDictionary? itemTypes = null)
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
                  itemTypeInfo = itemTypes.FirstOrDefault(item => itemType.IsSubclassOf(item.Value.Type)).Value;
                }
                if (itemTypeInfo != null)
                {
                  itemTag = itemTypes.FirstOrDefault(item => item.Value == itemTypeInfo).Key;
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

    public override void WriteStartElement(IXWriter writer, string propTag)
    {
      writer.WriteStartElement(propTag);
    }

    public override void WriteEndElement(IXWriter writer, string propTag)
    {
      writer.WriteEndElement(propTag);
    }

    public override void WriteAttributeString(IXWriter writer, string attrName, string valStr)
    {
      writer.WriteAttributeString(attrName, valStr);
    }

    public override void WriteValue(IXWriter writer, object? value)
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

    public override void WriteValue(IXWriter writer, string propTag, object value)
    {
      WriteStartElement(writer, propTag);
      WriteValue(writer, value);
      WriteEndElement(writer, propTag);
    }

    public override string EncodeStringValue(string str)
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

    #region Deserialize methods
    public override object? Deserialize(Stream stream)
    {
      using (XmlReader xmlReader = XmlReader.Create(stream))
      {
        return Deserialize(xmlReader);
      }
    }

    public override object? Deserialize(TextReader textReader)
    {
      using (XmlReader xmlReader = XmlReader.Create(textReader))
      {
        return Deserialize(xmlReader);
      }
    }

    public override object? Deserialize(string str)
    {
      using (XmlReader xmlReader = XmlReader.Create(new StringReader(str)))
      {
        return Deserialize(xmlReader);
      }
    }


    public object? Deserialize(XmlReader reader)
    {
      if (reader == null)
        throw new InternalException("$Reader must be set prior to deserialize");
      SkipToElement(reader);
      if (reader.EOF)
        return null;
      var xmlName = reader.Name;
      var aName = new XmlQualifiedName(xmlName, reader.Prefix);
      if (!KnownTypes.TryGetValue(aName.ToString(), out var aTypeInfo))
        throw new XmlInternalException($"Element {aName} not recognized while deserialization.", reader);
      var constructor = aTypeInfo.KnownConstructor;
      if (constructor == null)
        throw new XmlInternalException($"Type {aTypeInfo.Type.Name} must have a public, parameterless constructor.", reader);
      var obj = constructor.Invoke(new object[0]);

      if (obj is IXSerializable qSerializable)
      {
        qSerializable.Deserialize(this);
      }
      else if (obj is IXmlSerializable xmlSerializable)
      {
        xmlSerializable.ReadXml(reader);
      }
      else if (obj != null)
      {
        ReadObject(obj, reader);
      }
      return obj;
    }
    #endregion


    #region Read methods

    public void SkipToElement(XmlReader reader)
    {
      while (!reader.EOF && reader.NodeType != XmlNodeType.Element) 
        reader.Read();
    }

    public void SkipWhitespaces(XmlReader reader)
    {
      while (reader.NodeType == XmlNodeType.Whitespace)
        reader.Read();
    }

    public void ReadObject(object obj, XmlReader reader)
    {
      if (reader.NodeType != XmlNodeType.Element)
        throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
      bool isEmptyElement = reader.IsEmptyElement;
      ReadAttributesBase(obj, reader);
      reader.Read();
      SkipWhitespaces(reader);
      if (isEmptyElement)
        return;
      ReadElementsBase(obj, reader);
    }

    public int ReadAttributesBase(object obj, XmlReader reader)
    {
      var aType = obj.GetType();
      if (!KnownTypes.TryGetValue(aType, out var typeInfo))
        throw new XmlInternalException($"Unknown type {aType.Name} on deserialize", reader);
      var attribs = typeInfo.PropsAsAttributes;
      var props = typeInfo.PropsAsElements;
      foreach (var prop in props)
        if (!attribs.ContainsKey(prop.Name))
          attribs.Add(prop.Name, prop);
      var propList = attribs;

      int attrsRead = 0;
      reader.MoveToFirstAttribute();
      while (reader.NodeType == XmlNodeType.Attribute)
      {
        string attrName = reader.Name;
        var propertyToRead = propList.FirstOrDefault(item => item.Name == attrName);
        if (propertyToRead == null)
          throw new XmlInternalException($"Unknown property for attribute \"{attrName}\" in type {aType.Name}", reader);
        object? propValue = null;
        Type propType = propertyToRead.PropInfo.PropertyType;
        reader.ReadAttributeValue();
        propValue = ReadValue(propType, reader);
        if (propValue != null)
        {
          propertyToRead.PropInfo.SetValue(obj, propValue);
          attrsRead++;
        }
        reader.MoveToNextAttribute();
      }
      return attrsRead;
    }

    public int ReadElementsBase(object obj, XmlReader reader)
    {
      var aType = obj.GetType();
      if (!KnownTypes.TryGetValue(aType, out var typeInfo))
        throw new XmlInternalException($"Unknown type \"{aType.Name}\" on deserialize", reader);
      var attribs = typeInfo.PropsAsAttributes;
      var props = typeInfo.PropsAsElements;
      foreach (var prop in attribs)
        if (!props.ContainsKey(prop.Name))
          props.Add(prop.Name, prop);
      var propList = props;


      int propsRead = 0;
      while (reader.NodeType == XmlNodeType.Element)
      {
        var elementName = reader.Name;
        var propertyToRead = propList.FirstOrDefault(item => item.Name == elementName);
        if (propertyToRead == null)
        {
          if (typeInfo.KnownContentProperty != null)
          {
            var content = ReadElementAsObject(typeInfo.KnownContentProperty.PropInfo.PropertyType, reader);
            if (content != null)
            {
              typeInfo.KnownContentProperty.PropInfo.SetValue(obj, content);
              propsRead++;
            }
          }
          else
          if (typeInfo.Type.IsArray)
          {
          }
          else
            throw new XmlInternalException($"No property to read and no content property found for element \"{elementName}\"" +
              $" in type \"{aType.Name}\"", reader);
        }
        else
        {
          object? propValue = null;
          Type propType = propertyToRead.PropInfo.PropertyType;
          if (propType.IsClass && propType != typeof(string))
          {
            if (propertyToRead is SerializationArrayInfo arrayPropertyInfo)
              ReadElementAsCollectionProperty(obj, arrayPropertyInfo, reader);
            else
            {
              propValue = ReadElementAsObject(propType, reader);
              if (propValue != null)
              {
                propertyToRead.PropInfo.SetValue(obj, propValue);
                propsRead++;
              }
            }
          }
          else
            throw new XmlInternalException($"Element \"{elementName}\" cannot be read on deserialize", reader);
        }
        SkipWhitespaces(reader);
      }
      return propsRead;
    }

    public object? ReadElementAsObject(Type expectedType, XmlReader reader)
    {
      if (reader.NodeType != XmlNodeType.Element)
        throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
      var name = reader.Name;
      if (!KnownTypes.TryGetValue(name, out var typeInfo))
        throw new XmlInternalException($"Unknown type for element \"{name}\" on deserialize", reader);
      if (!(typeInfo.Type == expectedType || typeInfo.Type.IsSubclassOf(expectedType)))
        throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
          $" but {expectedType.Name} or its subclass expected", reader);
      if (typeInfo.KnownConstructor == null)
        throw new XmlInternalException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", reader);
      var obj = typeInfo.KnownConstructor.Invoke(new object[0]);
      ReadObject(obj, reader);
      return obj;
    }

    protected void ReadElementAsCollectionProperty(object obj, SerializationArrayInfo propertyArrayInfo, XmlReader reader)
    {
      var collectionType = propertyArrayInfo.PropInfo.PropertyType;
      if (reader.NodeType != XmlNodeType.Element)
        throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
      var name = reader.Name;
      var collection = propertyArrayInfo.PropInfo.GetValue(obj);
      if (collection == null)
      {
        if (!propertyArrayInfo.PropInfo.CanWrite)
          throw new XmlInternalException($"Collection at property {propertyArrayInfo.PropInfo.Name}" +
            $" of type {obj.GetType().Name} is null but readonly", reader);
        if (!KnownTypes.TryGetValue(name, out var typeInfo))
        {
          if (collectionType.IsArray)
          {
            var itemType = collectionType.GetElementType();
            if (itemType==null)
              throw new XmlInternalException($"Unknown item type of {collectionType} collection", reader);
            var tempCollection = new List<object>();
            reader.Read();
            ReadCollectionItems(tempCollection, itemType, reader);
            var itemArray = Array.CreateInstance(itemType, tempCollection.Count);
            for (int i = 0; i < tempCollection.Count; i++)
              itemArray.SetValue(tempCollection[i],i);
            propertyArrayInfo.PropInfo.SetValue(obj, itemArray);
          }
          else
            throw new XmlInternalException($"Unknown type for element \"{name}\" on deserialize", reader);
        }
        else
        {
          if (!(typeInfo.Type == collectionType || typeInfo.Type.IsSubclassOf(collectionType)))
            throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
              $" but {collectionType.Name} or its subclass expected", reader);
            if (typeInfo.KnownConstructor == null)
              throw new XmlInternalException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", reader);
            collection = typeInfo.KnownConstructor.Invoke(new object[0]);
            propertyArrayInfo.PropInfo.SetValue(obj, collection);
            reader.MoveToContent();
        }
      }
    }

    protected int ReadCollectionItems(ICollection<object> collection, Type expectedItemType, XmlReader reader)
    {
      int itemsRead = 0;
      while (reader.NodeType == XmlNodeType.Element)
      {
        var item = ReadElementAsItem(expectedItemType, reader);
        if (item!=null)
          collection.Add(item);
        SkipWhitespaces(reader);
      }
      return itemsRead;
    }

    public object? ReadElementAsItem(Type expectedType, XmlReader reader)
    {
      if (reader.NodeType != XmlNodeType.Element)
        throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
      var name = reader.Name;
      if (!KnownTypes.TryGetValue(expectedType, out var typeInfo))
        throw new XmlInternalException($"Unknown type {expectedType.Name} on deserialize", reader);
      if (typeInfo.KnownConstructor == null)
        throw new XmlInternalException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", reader);
      var obj = typeInfo.KnownConstructor.Invoke(new object[0]);
      ReadObject(obj, reader);
      return obj;
    }

    protected object? ReadValue(Type propType, XmlReader reader)
    {
      var str = reader.ReadContentAsString();
      object? propValue;
      if (propType.Name.StartsWith("Nullable`1"))
        propType = propType.GetGenericArguments()[0];
      if (propType == typeof(string))
        propValue = str;
      else
      if (propType == typeof(char))
      {
        if (str.Length > 0)
          propValue = str[0];
        else
          propValue = '\0';
      }
      else
      if (propType == typeof(bool))
      {
        switch (str.ToLower())
        {
          case "true":
          case "yes":
          case "on":
          case "t":
          case "1":
            propValue = true;
            break;
          case "false":
          case "no":
          case "off":
          case "f":
          case "0":
            propValue = false;
            break;
          default:
            throw new XmlInternalException($"Cannot convert \"{str}\" to boolean value", reader);
        }
      }
      else if (propType.IsEnum)
      {
        if (!Enum.TryParse(propType, str, Options.IgnoreCaseEnum, out propValue))
          throw new XmlInternalException($"Cannot convert \"{str}\" to enum value of type {propType.Name}", reader);
      }
      else if (propType == typeof(int))
      {
        if (!int.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to int value", reader);
        propValue = val;
      }
      else if (propType == typeof(uint))
      {
        if (!uint.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to uint value", reader);
        propValue = val;
      }
      else if (propType == typeof(long))
      {
        if (!long.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to int64 value", reader);
        propValue = val;
      }
      else if (propType == typeof(ulong))
      {
        if (!ulong.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to uint64 value", reader);
        propValue = val;
      }
      else if (propType == typeof(byte))
      {
        if (!byte.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to byte value", reader);
        propValue = val;
      }
      else if (propType == typeof(sbyte))
      {
        if (!sbyte.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to signed byte value", reader);
        propValue = val;
      }
      else if (propType == typeof(short))
      {
        if (!short.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to int16 value", reader);
        propValue = val;
      }
      else if (propType == typeof(ushort))
      {
        if (!ushort.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to uint16 value", reader);
        propValue = val;
      }
      else if (propType == typeof(float))
      {
        if (!float.TryParse(str, NumberStyles.Float, Options.Culture, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to float value", reader);
        propValue = val;
      }
      else if (propType == typeof(double))
      {
        if (!double.TryParse(str, NumberStyles.Float, Options.Culture, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to double value", reader);
        propValue = val;
      }
      else if (propType == typeof(decimal))
      {
        if (!decimal.TryParse(str, NumberStyles.Float, Options.Culture, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to decimal value", reader);
        propValue = val;
      }
      else if (propType == typeof(DateTime))
      {
        if (!DateTime.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to date/time value", reader);
        propValue = val;
      }
      else if (propType == typeof(TimeSpan))
      {
        if (!TimeSpan.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to time span value", reader);
        propValue = val;
      }
      else
        throw new XmlInternalException($"Value type \"{propType}\" not supported for deserialization", reader);
      return propValue;
    }
    #endregion

  }
}

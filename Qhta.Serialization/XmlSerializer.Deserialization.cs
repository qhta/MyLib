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
  public partial class XmlSerializer
  {

    #region Deserialize methods
    public object? Deserialize(Stream stream)
    {
      using (XmlTextReader xmlReader = new XmlTextReader(stream))
      {
        xmlReader.WhitespaceHandling = WhitespaceHandling.None;
        return Deserialize(xmlReader);
      }
    }

    public object? Deserialize(TextReader textReader)
    {
      using (XmlTextReader xmlReader = new XmlTextReader(textReader))
      {
        xmlReader.WhitespaceHandling = WhitespaceHandling.None;
        return Deserialize(xmlReader);
      }
    }

    public object? Deserialize(string str)
    {
      using (XmlTextReader xmlReader = new XmlTextReader(new StringReader(str)))
      {
        xmlReader.WhitespaceHandling = WhitespaceHandling.None;
        return Deserialize(xmlReader);
      }
    }


    public object? Deserialize(XmlTextReader reader)
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

    public void SkipToElement(XmlTextReader reader)
    {
      while (!reader.EOF && reader.NodeType != XmlNodeType.Element)
        reader.Read();
    }

    public void SkipWhitespaces(XmlTextReader reader)
    {
      while (reader.NodeType == XmlNodeType.Whitespace)
        reader.Read();
    }

    public void ReadObject(object obj, XmlTextReader reader)
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
      ReadTextPropertyValue(obj, reader);
      if (reader.NodeType == XmlNodeType.EndElement)
        reader.Read();
    }

    public int ReadAttributesBase(object obj, XmlTextReader reader)
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
        if (attrName == "xml:space")
        {
          reader.ReadAttributeValue();
          var str = reader.ReadContentAsString();
          if (str == "preserve")
            reader.WhitespaceHandling = WhitespaceHandling.Significant;
          else
            reader.WhitespaceHandling = WhitespaceHandling.None;
        }
        else
        {
          var propertyToRead = propList.FirstOrDefault(item => item.Name == attrName);
          if (propertyToRead == null)
            throw new XmlInternalException($"Unknown property for attribute \"{attrName}\" in type {aType.Name}", reader);
          object? propValue = null;
          reader.ReadAttributeValue();
          propValue = ReadValue(propertyToRead, reader);
          if (propValue != null)
          {
            propertyToRead.PropInfo.SetValue(obj, propValue);
            attrsRead++;
          }
        }
        reader.MoveToNextAttribute();
      }
      return attrsRead;
    }

    public int ReadElementsBase(object obj, XmlTextReader reader)
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

    public object? ReadElementAsObject(Type? expectedType, XmlTextReader reader)
    {
      if (reader.NodeType != XmlNodeType.Element)
        throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
      var name = reader.Name;
      if (!KnownTypes.TryGetValue(name, out var typeInfo))
      {
        //if (expectedType != null)
        //    typeInfo = AddKnownType(expectedType);
        if (typeInfo == null)
          throw new XmlInternalException($"Unknown type for element \"{name}\" on deserialize", reader);
      }
      if (expectedType != null)
        if (!(typeInfo.Type == expectedType || typeInfo.Type.IsSubclassOf(expectedType)))
          throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
            $" but {expectedType.Name} or its subclass expected", reader);
      return ReadElementWithKnownType(typeInfo, reader);
    }

    public object? ReadElementAsItem(Type? expectedType, SerializationArrayInfo propertyArrayInfo, XmlTextReader reader)
    {
      if (reader.NodeType != XmlNodeType.Element)
        throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
      var name = reader.Name;
      SerializationTypeInfo? typeInfo;
      if (propertyArrayInfo.KnownItemTypes.Count > 0)
        propertyArrayInfo.KnownItemTypes.TryGetValue(name, out typeInfo);
      else
        KnownTypes.TryGetValue(name, out typeInfo);
      if (typeInfo == null)
        throw new XmlInternalException($"Unknown type for element \"{name}\" on deserialize", reader);
      if (expectedType != null)
        if (!(typeInfo.Type == expectedType || typeInfo.Type.IsSubclassOf(expectedType)))
          throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
            $" but {expectedType.Name} or its subclass expected", reader);
      return ReadElementWithKnownType(typeInfo, reader);
    }

    protected object? ReadElementWithKnownType(SerializationTypeInfo typeInfo, XmlTextReader reader)
    {
      if (typeInfo.KnownConstructor == null)
      {
        if (IsSimple(typeInfo.Type))
        {
          reader.Read();
          var result = ReadValue(typeInfo.Type, null, reader);
          reader.Read();
          return result;
        }
        throw new XmlInternalException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", reader);
      }
      var obj = typeInfo.KnownConstructor.Invoke(new object[0]);
      ReadObject(obj, reader);
      return obj;

    }

    protected void ReadElementAsCollectionProperty(object obj, SerializationArrayInfo propertyArrayInfo, XmlTextReader reader)
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
            if (itemType == null)
              throw new XmlInternalException($"Unknown item type of {collectionType} collection", reader);
            var tempCollection = new List<object>();
            if (reader.IsStartElement(propertyArrayInfo.Name) && !reader.IsEmptyElement)
            {
              reader.Read();
              ReadCollectionItems(tempCollection, itemType, propertyArrayInfo, reader);
            }
            var itemArray = Array.CreateInstance(itemType, tempCollection.Count);
            for (int i = 0; i < tempCollection.Count; i++)
              itemArray.SetValue(tempCollection[i], i);
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
        reader.Read();
      }
    }

    protected int ReadCollectionItems(ICollection<object> collection, Type expectedItemType, SerializationArrayInfo propertyArrayInfo, XmlTextReader reader)
    {
      int itemsRead = 0;
      while (reader.NodeType == XmlNodeType.Element)
      {
        var item = ReadElementAsItem(expectedItemType, propertyArrayInfo, reader);
        if (item != null)
          collection.Add(item);
        SkipWhitespaces(reader);
      }
      return itemsRead;
    }

    public void ReadTextPropertyValue(object obj, XmlTextReader reader)
    {
      if (reader.NodeType == XmlNodeType.Text)
      {
        var aType = obj.GetType();
        if (!KnownTypes.TryGetValue(aType, out var typeInfo))
          throw new XmlInternalException($"Unknown type {aType.Name} on deserialize", reader);
        var textPropertyInfo = typeInfo.KnownTextProperty;
        if (textPropertyInfo == null)
          throw new XmlInternalException($"Unknown text property in {aType.Name} on deserialize", reader);
        var value = ReadValue(textPropertyInfo, reader);
        textPropertyInfo.PropInfo.SetValue(obj, value);
      }
    }

    public void ReadTextValue(Type valueType, XmlTextReader reader)
    {
    }

    protected object? ReadValue(SerializationPropertyInfo serializationPropertyInfo, XmlTextReader reader)
    {
      var propType = serializationPropertyInfo.PropInfo.PropertyType;
      var typeConverter = serializationPropertyInfo.TypeConverter;
      return ReadValue(propType, typeConverter, reader);
    }

    protected object? ReadValue(Type expectedType, TypeConverter? typeConverter, XmlTextReader reader)
    {
      var str = reader.ReadContentAsString();
      object? propValue;
      if (expectedType.Name.StartsWith("Nullable`1"))
        expectedType = expectedType.GetGenericArguments()[0];
      if (expectedType == typeof(string))
        propValue = str;
      else
      if (expectedType == typeof(char))
      {
        if (str.Length > 0)
          propValue = str[0];
        else
          propValue = '\0';
      }
      else
      if (expectedType == typeof(bool))
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
            if (typeConverter != null)
            {
              propValue = typeConverter.ConvertFromString(str);
              break;
            }
            throw new XmlInternalException($"Cannot convert \"{str}\" to boolean value", reader);
        }
      }
      else if (expectedType.IsEnum)
      {
        if (!Enum.TryParse(expectedType, str, Options.IgnoreCaseEnum, out propValue))
          throw new XmlInternalException($"Cannot convert \"{str}\" to enum value of type {expectedType.Name}", reader);
      }
      else if (expectedType == typeof(int))
      {
        if (!int.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to int value", reader);
        propValue = val;
      }
      else if (expectedType == typeof(uint))
      {
        if (!uint.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to uint value", reader);
        propValue = val;
      }
      else if (expectedType == typeof(long))
      {
        if (!long.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to int64 value", reader);
        propValue = val;
      }
      else if (expectedType == typeof(ulong))
      {
        if (!ulong.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to uint64 value", reader);
        propValue = val;
      }
      else if (expectedType == typeof(byte))
      {
        if (!byte.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to byte value", reader);
        propValue = val;
      }
      else if (expectedType == typeof(sbyte))
      {
        if (!sbyte.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to signed byte value", reader);
        propValue = val;
      }
      else if (expectedType == typeof(short))
      {
        if (!short.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to int16 value", reader);
        propValue = val;
      }
      else if (expectedType == typeof(ushort))
      {
        if (!ushort.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to uint16 value", reader);
        propValue = val;
      }
      else if (expectedType == typeof(float))
      {
        if (!float.TryParse(str, NumberStyles.Float, Options.Culture, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to float value", reader);
        propValue = val;
      }
      else if (expectedType == typeof(double))
      {
        if (!double.TryParse(str, NumberStyles.Float, Options.Culture, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to double value", reader);
        propValue = val;
      }
      else if (expectedType == typeof(decimal))
      {
        if (!decimal.TryParse(str, NumberStyles.Float, Options.Culture, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to decimal value", reader);
        propValue = val;
      }
      else if (expectedType == typeof(DateTime))
      {
        if (!DateTime.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to date/time value", reader);
        propValue = val;
      }
      else if (expectedType == typeof(TimeSpan))
      {
        if (!TimeSpan.TryParse(str, out var val))
          throw new XmlInternalException($"Cannot convert \"{str}\" to time span value", reader);
        propValue = val;
      }
      else
        throw new XmlInternalException($"Value type \"{expectedType}\" not supported for deserialization", reader);
      return propValue;
    }
    #endregion

  }
}

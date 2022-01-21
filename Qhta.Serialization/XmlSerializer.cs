using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

using Qhta.TestHelper;

namespace Qhta.Serialization
{
  public class XmlSerializer : BaseSerializer
  {
    //private XmlWriter writer { get; set; }

    private XmlReader reader { get; set; }

    #region Creator methods
    public XmlSerializer()
    {
    }

    public XmlSerializer(Type type, Type[] extraTypes, SerializationOptions options)
    {
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
      else if (obj is IXmlSerializable xmlSerializable)
      {
        //xmlSerializable.WriteXml(writer);
      }
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
        string attrName = item.Name;
        var propValue = propInfo.GetValue(obj);
        if (propValue != null)
        {
          var defaultValue =
            (propInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false).FirstOrDefault() as DefaultValueAttribute)?.Value;
          if (defaultValue == null || !propValue.Equals(defaultValue))
          {
            string str = null;
            if (propValue is string)
              str = propValue.ToString();
            else if (propValue is bool || propValue.GetType() == typeof(bool?))
              str = propValue.ToString().ToLower();
            else if (propValue is int || propValue.GetType() == typeof(int?))
              str = propValue.ToString();
            if (str != null)
            {
              writer.WriteAttributeString(attrName, str);
              attrsWritten++;
            }
          }
        }
      }
      return attrsWritten;
    }

    public override int WritePropertiesBase(IXWriter writer, string elementTag, object obj)
    {
      var aType = obj.GetType();
      if (!KnownTypes.TryGetValue(aType, out var typeInfo))
        throw new InternalException($"Unknown type \"{aType.Name}\" while serializing object attributes");
      var attribs = typeInfo.PropsAsAttributes;
      var props = typeInfo.PropsAsElements;
      var arrays = typeInfo.PropsAsArrays;

      int propsWritten = 0;
      foreach (var item in attribs)
      {
        var propInfo = item.PropInfo;
        string propTag = item.Name;
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
            WriteValue(writer, propValue.ToString());
          WriteEndElement(writer, propTag);
          propsWritten++;
        }
      }

      foreach (var item in props)
      {
        var propInfo = item.PropInfo;
        string propTag = item.Name;
        if (Options?.PrecedePropertyNameWithElementName == true)
          propTag = elementTag + "." + propTag;
        var propValue = propInfo.GetValue(obj);
        if (propValue != null)
        {
          WriteStartElement(writer, propTag);
          if (propValue is IXSerializable serializableValue)
            serializableValue.Serialize(this, writer);
          else
            WriteValue(writer, propValue.ToString());
          WriteEndElement(writer, propTag);
          propsWritten++;
        }
      }

      foreach (var item in arrays)
      {
        var propInfo = item.PropInfo;
        string propTag = item.Name;
        if (propTag != null && elementTag != null)
        {
          if (Options?.PrecedePropertyNameWithElementName == true)
            propTag = elementTag + "." + propTag;
        }
        var propValue = propInfo.GetValue(obj);
        if (propValue != null)
        {
          if (propValue is IXSerializable serializableValue)
            serializableValue.Serialize(this, writer);
          else
          if (propValue is ICollection collection)
          {
            WriteCollectionBase(writer, null, propTag, collection);
          }
          propsWritten++;
        }
      }
      return propsWritten;
    }


    public override int WriteCollectionBase(IXWriter writer, string elementTag, string propTag, ICollection collection, KnownTypesDictionary itemTypes = null)
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
            var itemTag = Options.ItemTag ?? "item";
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
                  WriteAttributeString(writer, "key", key.ToString());
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
              TypeConverter typeConverter = null;
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
              if (!String.IsNullOrEmpty(itemTag))
                WriteStartElement(writer, itemTag);
              if (typeConverter != null)
                WriteValue(writer, typeConverter.ConvertToString(item));
              else
                SerializeObject(writer, item);
              if (!String.IsNullOrEmpty(itemTag))
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

    public override void WriteValue(IXWriter writer, object value)
    {
      if (value is string valStr)
        writer.WriteValue(valStr);
      else if (value is bool)
        writer.WriteValue(value.ToString().ToLower());
      else if (value is int)
        writer.WriteValue(value.ToString().ToLower());
      else if (value is IXSerializable qSerializable)
        qSerializable.Serialize(this, writer);
    }

    public override void WriteValue(IXWriter writer, string propTag, object value)
    {
      WriteStartElement(writer, propTag);
      WriteValue(writer, value);
      WriteEndElement(writer, propTag);
    }

    public override string EncodeStringValue(string str)
      => str.Replace("\t", "&#9;");
    #endregion

    #region Deserialize methods
    public override object Deserialize(Stream stream)
    {
      using (XmlReader xmlReader = XmlReader.Create(stream))
      {
        return Deserialize(xmlReader);
      }
    }

    public override object Deserialize(TextReader textReader)
    {
      using (XmlReader xmlReader = XmlReader.Create(textReader))
      {
        return Deserialize(xmlReader);
      }
    }

    public object Deserialize(XmlReader xmlReader)
    {
      object obj = null;
      try
      {
        reader = xmlReader;
        obj = DeserializeObject();
      }
      finally
      {
        reader = null;
      }
      return obj;
    }

    public override object DeserializeObject()
    {
      while (!reader.EOF && reader.NodeType != XmlNodeType.Element) reader.Read();
      if (reader.EOF)
        return null;
      var xmlName = reader.Name;
      var aName = new XmlQualifiedName(xmlName, reader.Prefix);
      if (!KnownTypes.TryGetValue(aName.ToString(), out var aTypeInfo))
        throw new InternalException($"Element {aName} not recognized while deserialization.");
      var constructor = aTypeInfo.KnownConstructor;
      if (constructor == null)
        throw new InternalException($"Type {aTypeInfo.Type.Name} must have a public, parameterless constructor.");
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
        //ReadObject(obj);
      }
      return obj;
    }
    #endregion


    //#region Read methods

    //public override void ReadObject(object obj)
    //{
    //  string tag = obj.GetType().Name;
    //  var xmlAttribute = obj.GetType().GetCustomAttributes(true).OfType<XmlRootAttribute>().FirstOrDefault();
    //  if (xmlAttribute != null && xmlAttribute.ElementName != null)
    //    tag = xmlAttribute.ElementName;
    //  ReadAttributesBase(obj);
    //  ReadPropertiesBase(tag, obj);
    //  reader.ReadEndElement();
    //}

    //public override int ReadAttributesBase(object obj)
    //{
    //  var propList = GetPropsAsXmlAttributes(obj);
    //  propList.AddRange(GetPropsAsXmlElements(obj));
    //  int attrsRead = 0;
    //  reader.MoveToFirstAttribute();
    //  while (reader.NodeType == XmlNodeType.Attribute)
    //  {
    //    string attrName = reader.Name;
    //    var propertyToRead = propList.FirstOrDefault(item => item.Name == attrName);
    //    if (propertyToRead == null)
    //      break;
    //    object propValue = null;
    //    Type propType = propertyToRead.PropInfo.PropertyType;
    //    reader.ReadAttributeValue();
    //    if (propType == typeof(string))
    //      propValue = reader.ReadContentAsString();
    //    else if (propType == typeof(bool) || propType == typeof(bool?))
    //      propValue = reader.ReadContentAsBoolean();
    //    else if (propType == typeof(int) || propType == typeof(int?))
    //      propValue = reader.ReadContentAsInt();
    //    else
    //      throw new XmlException($"Attribute \"{attrName}\" type \"{propType}\" not supported for \"{obj.GetType().Name}\" class on deserialize.");
    //    if (propValue != null)
    //    {
    //      propertyToRead.PropInfo.SetValue(obj, propValue);
    //      attrsRead++;
    //    }
    //    reader.MoveToNextAttribute();
    //  }
    //  reader.Read();
    //  return attrsRead;
    //}

    //public override int ReadExtraAttributes(object obj, IDictionary<string, object> properties)
    //{
    //  //int attrsRead = 0;
    //  ////reader.MoveToFirstAttribute();
    //  //while (reader.NodeType == XmlNodeType.Attribute)
    //  //{
    //  //  string attrName = reader.Name;        
    //  //  object propValue = null;
    //  //  Type propType = propertyToRead.PropInfo.PropertyType;
    //  //  reader.ReadAttributeValue();
    //  //  if (propType == typeof(string))
    //  //    propValue = reader.ReadContentAsString();
    //  //  else if (propType == typeof(bool) || propType == typeof(bool?))
    //  //    propValue = reader.ReadContentAsBoolean();
    //  //  else if (propType == typeof(int) || propType == typeof(int?))
    //  //    propValue = reader.ReadContentAsInt();
    //  //  else
    //  //    throw new XmlException($"Attribute \"{attrName}\" type \"{propType}\" not supported for \"{obj.GetType().Name}\" class on deserialize.");
    //  //  if (propValue != null)
    //  //  {
    //  //    propertyToRead.PropInfo.SetValue(obj, propValue);
    //  //    attrsRead++;
    //  //  }
    //  //  reader.MoveToNextAttribute();
    //  //}
    //  //reader.Read();
    //  //return attrsRead;
    //  return 0;
    //}

    //public override int ReadPropertiesBase(string tag, object obj)
    //{
    //  var propList = GetPropsAsXmlElements(obj);
    //  var attribs = GetPropsAsXmlAttributes(obj);
    //  var arrays = GetPropsAsXmlArrays(obj);
    //  propList.AddRange(attribs);
    //  propList.AddRange(arrays);


    //  while (reader.NodeType == XmlNodeType.Whitespace)
    //    reader.Read();
    //  int propsRead = 0;
    //  while (reader.NodeType == XmlNodeType.Element)
    //  {
    //    var elementName = reader.Name;
    //    var propertyToRead = propList.FirstOrDefault(item => item.Name == elementName);
    //    if (propertyToRead == null)
    //      //throw new XmlException($"Property \"{elementName}\" not found for \"{obj.GetType().Name}\" class on deserialize.");
    //      break;
    //    reader.Read();
    //    object propValue = null;
    //    Type propType = propertyToRead.PropInfo.PropertyType;
    //    if (propType == typeof(string))
    //    {
    //      propValue = reader.ReadContentAsString();
    //      reader.ReadEndElement();
    //    }
    //    else if (propType == typeof(bool) || propType == typeof(bool?))
    //    {
    //      propValue = reader.ReadContentAsBoolean();
    //      reader.ReadEndElement();
    //    }
    //    else if (propType == typeof(int) || propType == typeof(int?))
    //    {
    //      propValue = reader.ReadContentAsInt();
    //      reader.ReadEndElement();
    //    }
    //    else
    //    {
    //      propValue = DeserializeObject();
    //    }
    //    if (propValue != null)
    //      propertyToRead.PropInfo.SetValue(obj, propValue);
    //    propsRead++;
    //    while (reader.NodeType == XmlNodeType.Whitespace)
    //      reader.Read();
    //  }
    //  return propsRead;
    //}

    //public override int ReadCollectionBase(string propTag, IList collection)
    //{
    //  int itemsRead = 0;
    //  while (reader.NodeType == XmlNodeType.Element)
    //  {
    //    var elementName = reader.Name;
    //    object value = null;
    //    if (elementName=="Item")
    //    { 
    //      reader.Read();
    //    }
    //    else
    //    {
    //      value = DeserializeObject();
    //    }
    //    if (value!=null)
    //      collection.Add(value);
    //    itemsRead++;
    //    while (reader.NodeType == XmlNodeType.Whitespace)
    //      reader.Read();
    //  }
    //  /*
    //  foreach (var item in collection)
    //  {
    //    if (item != null)
    //    {
    //      if (item is IXmlSerializable serializableItem)
    //        serializableItem.ReadXml(reader);
    //      else if (item.GetType().Name.StartsWith("KeyValuePair`"))
    //      {
    //        var keyProp = item.GetType().GetProperty("Key");
    //        var valProp = item.GetType().GetProperty("Value");
    //        if (keyProp != null && valProp != null)
    //        {
    //          var key = keyProp.GetValue(item);
    //          var val = valProp.GetValue(item);
    //          if (key != null)
    //          {
    //            //reader.WriteStartElement("Item");
    //            //reader.WriteAttributeString("key", key.ToString());
    //            //if (val is IXmlSerializable serializableVal)
    //            //  serializableVal.WriteXml(writer);
    //            //else if (val is string strVal)
    //            //  writer.WriteString(strVal);
    //            //else if (val != null)
    //            //  WriteObject(writer, val, options);
    //            //writer.WriteEndElement();
    //          }
    //        }
    //      }
    //      itemsRead++;
    //    }
    //    else
    //      throw new NotImplementedException($"Item of type \"{item.GetType().Name}\" is not serializable");
    //  }
    //  */
    //  return itemsRead;
    //}

    //public override bool ReadStartElement(string propTag)
    //{
    //  while (reader.NodeType == XmlNodeType.Whitespace) reader.Read();
    //  return (reader.NodeType == XmlNodeType.Element && reader.Name == propTag);
    //}

    //public override bool ReadEndElement(string propTag)
    //{
    //  while (reader.NodeType == XmlNodeType.Whitespace) reader.Read();
    //  return (reader.NodeType == XmlNodeType.EndElement && reader.Name == propTag);
    //}

    ////public override string ReadAttributeString(string attrName)
    ////{
    ////  string text = null;
    ////  if (reader.NodeType == XmlNodeType.Attribute);
    ////    text = reader.ReadContentAsString();
    ////  return text;
    ////}

    ////public abstract object ReadValue();

    ////public abstract object ReadValue(string propTag);
    //#endregion

  }
}

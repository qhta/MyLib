using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Qhta.TestHelper;
using Qhta.TypeUtils;

namespace Qhta.Xml.Serialization
{
  public delegate void OnUnknownProperty(object readObject, string elementName);

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
      if (!KnownTypes.TryGetValue(aName.ToString(), out var typeInfo))
        throw new XmlInternalException($"Element {aName} not recognized while deserialization.", reader);
      var constructor = typeInfo.KnownConstructor;
      if (constructor == null)
        throw new XmlInternalException($"Type {typeInfo.Type.Name} must have a public, parameterless constructor.", reader);
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
        ReadObject(obj, reader, typeInfo);
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

    public void ReadObject(object obj, XmlTextReader reader, SerializationTypeInfo typeInfo, OnUnknownProperty? onUnknownProperty = null)
    {
      if (reader.NodeType != XmlNodeType.Element)
        throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
      bool isEmptyElement = reader.IsEmptyElement;
      ReadAttributesBase(obj, reader, typeInfo, onUnknownProperty);
      reader.Read(); // read end of start element and go to its content;
      SkipWhitespaces(reader);
      if (isEmptyElement)
        return;
      ReadElementsBase(obj, reader, typeInfo);
      ReadTextPropertyValue(obj, reader);
      if (reader.NodeType == XmlNodeType.EndElement)
        reader.Read();
    }

    public int ReadAttributesBase(object obj, XmlTextReader reader, SerializationTypeInfo typeInfo, OnUnknownProperty? onUnknownProperty = null)
    {
      var aType = obj.GetType();
      var attribs = typeInfo.PropsAsAttributes;
      var props = typeInfo.PropsAsElements;
      foreach (var prop in props)
        if (!attribs.ContainsKey(prop.Name))
          attribs.Add(prop.Name, prop);
      var propList = attribs;

      int attrsRead = 0;
      reader.MoveToFirstAttribute();
      var attrCount = reader.AttributeCount;
      for (int i = 0; i < attrCount; i++)
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
          {
            if (onUnknownProperty == null)
              throw new XmlInternalException($"Unknown property for attribute \"{attrName}\" in type {aType.Name}", reader);
            onUnknownProperty(obj, attrName);
          }
          else
          {
            reader.ReadAttributeValue();
            object? propValue = ReadValue(propertyToRead, reader);
            if (propValue != null)
            {
              try
              {
                propertyToRead.PropInfo.SetValue(obj, propValue);
              }
              catch (Exception ex)
              {
                throw new XmlInternalException($"Could not set value for property {propertyToRead.PropInfo} in type {aType.Name}", reader, ex);
              }
              attrsRead++;
            }
          }
        }
        reader.MoveToNextAttribute();
      }
      return attrsRead;
    }

    public int ReadElementsBase(object obj, XmlTextReader reader, SerializationTypeInfo typeInfo, OnUnknownProperty? onUnknownProperty = null)
    {
      var aType = obj.GetType();
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
        if (propertyToRead != null)
        {
          object? propValue = null;
          Type propType = propertyToRead.PropInfo.PropertyType;
          if (propertyToRead.XmlConverter != null)
          {
            propValue = propertyToRead.XmlConverter.ReadXml(reader, typeInfo, propertyToRead, null, this);
          }
          else
          {
            if (propType.IsClass && propType != typeof(string))
            {
              if (propertyToRead is ArrayPropertyInfo arrayPropertyInfo)
                ReadElementAsCollectionProperty(obj, arrayPropertyInfo, reader);
              else
              {
                propValue = ReadElementAsProperty(propType, propertyToRead, reader);
              }
            }
            else
              throw new XmlInternalException($"Element \"{elementName}\" cannot be read on deserialize", reader);
          }
          if (propValue != null)
          {
            propertyToRead.PropInfo.SetValue(obj, propValue);
            propsRead++;
          }
        }
        else
        {
          if (typeInfo.KnownContentProperty != null)
          {
            var content = ReadElementAsObject(typeInfo.KnownContentProperty.PropInfo.PropertyType, reader);
            if (content != null)
            {
              typeInfo.KnownContentProperty.PropInfo.SetValue(obj, content);
              propsRead++;
              continue;
            }
          }
          if (typeInfo.KnownItemTypes != null)
          {
            if (!typeInfo.KnownItemTypes.TryGetValue(elementName, out var knownItemTypeInfo))
            {
              knownItemTypeInfo = (typeInfo.KnownItemTypes as IEnumerable<SerializationItemTypeInfo>).FirstOrDefault();
              if (knownItemTypeInfo == null)
                if (KnownTypes.TryGetValue(elementName, out var itemTypeInfo))
                  knownItemTypeInfo = new SerializationItemTypeInfo(elementName, itemTypeInfo);
                else
                  throw new XmlInternalException($"Unrecognized element \"{elementName}\"", reader);
            }
            var item = ReadElementWithKnownTypeInfo(knownItemTypeInfo, reader);
            if (item == null)
              throw new XmlInternalException($"Item of type {knownItemTypeInfo.Type} could not be read at \"{elementName}\"", reader);
            if (knownItemTypeInfo.AddMethod != null)
            {
              if (obj is IDictionary dictionaryObj)
                knownItemTypeInfo.AddMethod.Invoke(obj, new object[] { elementName, item });
              else
                knownItemTypeInfo.AddMethod.Invoke(obj, new object[] { item });
            }
            else
            {
              if (obj is IDictionary dictionaryObj)
                dictionaryObj.Add(elementName, item);
              else
              {
                var adddMethod = obj.GetType().GetMethod("Add", new Type[] { knownItemTypeInfo.Type });
                if (adddMethod != null)
                  adddMethod.Invoke(obj, new object[] { item });
                else
                  if (obj is ICollection collectionObj)
                    throw new XmlInternalException($"Add method for {knownItemTypeInfo.Type} item not found in type {aType.FullName}", reader);
              }
            }
            propsRead++;
            continue;
          }
          if (onUnknownProperty == null)
          {
            throw new XmlInternalException($"No property to read and no content property found for element \"{elementName}\"" +
              $" in type {aType.FullName}", reader);
          }
          onUnknownProperty(obj, elementName);
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
        if (typeInfo == null)
          throw new XmlInternalException($"Unknown type for element \"{name}\" on deserialize", reader);
      }
      if (expectedType != null)
        if (!typeInfo.Type.IsEqualOrSubclassOf(expectedType))
          throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
            $" but {expectedType.Name} or its subclass expected", reader);
      return ReadElementWithKnownTypeInfo(typeInfo, reader);
    }

    public object? ReadElementAsProperty(Type? expectedType, SerializationPropertyInfo propertyInfo, XmlTextReader reader)
    {
      if (reader.NodeType != XmlNodeType.Element)
        throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
      var name = reader.Name;
      if (!KnownTypes.TryGetValue(name, out var typeInfo))
      {
        if (expectedType != null)
          typeInfo = AddKnownType(expectedType);
        if (typeInfo == null)
          throw new XmlInternalException($"Unknown type for element \"{name}\" on deserialize", reader);
      }
      if (expectedType != null)
        if (!typeInfo.Type.IsEqualOrSubclassOf(expectedType))
          throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
            $" but {expectedType.Name} or its subclass expected", reader);
      return ReadElementWithKnownTypeInfo(typeInfo, reader);
    }

    public object? ReadElementAsItem(Type? expectedType, ArrayPropertyInfo arrayInfo, XmlTextReader reader)
    {
      if (reader.NodeType != XmlNodeType.Element)
        throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
      var name = reader.Name;
      SerializationTypeInfo? typeInfo = null;
      if (arrayInfo.KnownItemTypes.Count > 0)
      {
        if (arrayInfo.KnownItemTypes.TryGetValue(name, out var typeItemInfo))
          typeInfo = typeItemInfo.TypeInfo;
      }
      else
        KnownTypes.TryGetValue(name, out typeInfo);
      if (typeInfo == null)
        throw new XmlInternalException($"Unknown type for element \"{name}\" on deserialize", reader);
      if (expectedType != null)
        if (!typeInfo.Type.IsEqualOrSubclassOf(expectedType))
          throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
            $" but {expectedType.Name} or its subclass expected", reader);
      return ReadElementWithKnownTypeInfo(typeInfo, reader);
    }

    public object? ReadElementWithKnownTypeInfo(SerializationTypeInfo typeInfo, XmlTextReader reader)
    {
      if (typeInfo.XmlConverter != null && typeInfo.XmlConverter.CanRead)
        return typeInfo.XmlConverter.ReadXml(reader, typeInfo, null, null, this);
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
      if (obj is IXSerializable qSerializable)
      {
        qSerializable.Deserialize(this);
      }
      else if (obj is IXmlSerializable xmlSerializable)
      {
        xmlSerializable.ReadXml(reader);
      }
      else
        ReadObject(obj, reader, typeInfo);
      return obj;
    }

    public object? ReadElementWithKnownTypeInfo(SerializationItemTypeInfo itemTypeInfo, XmlTextReader reader)
    {
      var typeInfo = itemTypeInfo.TypeInfo;
      if (typeInfo.XmlConverter != null && typeInfo.XmlConverter.CanRead)
        return typeInfo.XmlConverter.ReadXml(reader, typeInfo, null, itemTypeInfo, this);
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
      if (obj is IXSerializable qSerializable)
      {
        qSerializable.Deserialize(this);
      }
      else if (obj is IXmlSerializable xmlSerializable)
      {
        xmlSerializable.ReadXml(reader);
      }
      else
        ReadObject(obj, reader, typeInfo);
      return obj;
    }

    public object? ReadElementAsDictionaryItem(Type? expectedKeyType, Type? expectedValueType, DictionaryPropertyInfo dictionaryInfo, XmlTextReader reader)
    {
      //if (dictionaryInfo.ElementsAreKeys)
      //  return ReadElementAsKVPair1(expectedKeyType, expectedValueType, dictionaryInfo, reader);
      //else
      return ReadElementAsKVObject(expectedKeyType, expectedValueType, dictionaryInfo, reader);
    }

    public object? ReadElementAsKVPair1(Type? expectedKeyType, Type? expectedValueType, DictionaryPropertyInfo dictionaryInfo, XmlTextReader reader)
    {
      if (reader.NodeType != XmlNodeType.Element)
        throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
      var keyName = reader.Name;
      SerializationTypeInfo? typeInfo = null;
      if (dictionaryInfo.KnownItemTypes.Count > 0)
      {
        if (dictionaryInfo.KnownItemTypes.TryGetValue(keyName, out var typeItemInfo))
        {
          typeInfo = typeItemInfo.TypeInfo;
          keyName = typeItemInfo.KeyName;
        }
      }
      if (keyName == null)
        keyName = dictionaryInfo.KeyName;
      //if (keyName == null)
      //  throw new XmlInternalException($"Key name unknown for dictionary item \"{name}\" on deserialize", reader);
      if (typeInfo == null)
        throw new XmlInternalException($"Unknown type for element \"{keyName}\" on deserialize", reader);
      var foundValueType = typeInfo.Type;
      if (expectedValueType != null)
        if (!foundValueType.IsEqualOrSubclassOf(expectedValueType))
        {
          if (!expectedValueType.IsCollection(foundValueType))
            throw new XmlInternalException($"Element \"{keyName}\" is mapped to {typeInfo.Type.Name}" +
              $" but {expectedValueType.Name} or its subclass expected", reader);
        }
      if (typeInfo.KnownConstructor == null)
      {
        if (IsSimple(typeInfo.Type))
        {
          reader.Read(); // read to end of start tag
          var result = ReadValue(typeInfo.Type, null, reader);
          reader.Read();
          return result;
        }
        throw new XmlInternalException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", reader);
      }
      KVPair kvPair = new KVPair();
      kvPair.Value = typeInfo.KnownConstructor.Invoke(new object[0]);
      ReadObject(kvPair.Value, reader, typeInfo, (object readObject, string elementName) =>
      {
        if (reader.NodeType == XmlNodeType.Attribute)
        {
          if (elementName == keyName)
          {
            var keyValue = ReadValue(dictionaryInfo.KeyTypeInfo?.Type ?? typeof(string), null, reader);
            kvPair.Key = keyValue;
          }
          else
            throw new XmlInternalException($"Unknown attribute \"{elementName}\" in type {typeInfo.Type.Name} on deserialize", reader);
        }
        else
        if (reader.NodeType == XmlNodeType.Element)
        {
          throw new XmlInternalException($"Unknown element \"{elementName}\" in type {typeInfo.Type.Name} on deserialize", reader);
        }
      });
      return kvPair;
    }

    public object? ReadElementAsKVObject(Type? expectedKeyType, Type? expectedValueType, DictionaryPropertyInfo dictionaryInfo, XmlTextReader reader)
    {
      if (reader.NodeType != XmlNodeType.Element)
        throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
      var name = reader.Name;
      SerializationTypeInfo? typeInfo = null;
      string? keyName = null;
      if (dictionaryInfo.KnownItemTypes.Count > 0)
      {
        if (dictionaryInfo.KnownItemTypes.TryGetValue(name, out var typeItemInfo))
        {
          typeInfo = typeItemInfo.TypeInfo;
          keyName = typeItemInfo.KeyName;
        }
      }
      if (keyName == null)
        keyName = dictionaryInfo.KeyName;
      //if (keyName == null)
      //  throw new XmlInternalException($"Key name unknown for dictionary item \"{name}\" on deserialize", reader);
      if (typeInfo == null)
        throw new XmlInternalException($"Unknown type for element \"{name}\" on deserialize", reader);
      var foundValueType = typeInfo.Type;
      if (expectedValueType != null)
        if (!foundValueType.IsEqualOrSubclassOf(expectedValueType))
        {
          if (!expectedValueType.IsCollection(foundValueType))
            throw new XmlInternalException($"Element \"{name}\" is mapped to {typeInfo.Type.Name}" +
              $" but {expectedValueType.Name} or its subclass expected", reader);
        }
      if (typeInfo.KnownConstructor == null)
      {
        if (IsSimple(typeInfo.Type))
        {
          reader.Read(); // read to end of start tag
          var result = ReadValue(typeInfo.Type, null, reader);
          reader.Read();
          return result;
        }
        throw new XmlInternalException($"Unknown constructor for type {typeInfo.Type.Name} on deserialize", reader);
      }
      KVPair kvPair = new KVPair();
      kvPair.Value = typeInfo.KnownConstructor.Invoke(new object[0]);
      ReadObject(kvPair.Value, reader, typeInfo, (object readObject, string elementName) =>
      {
        if (reader.NodeType == XmlNodeType.Attribute)
        {
          if (elementName == keyName)
          {
            var keyValue = ReadValue(dictionaryInfo.KeyTypeInfo?.Type ?? typeof(string), null, reader);
            kvPair.Key = keyValue;
          }
          else
            throw new XmlInternalException($"Unknown attribute \"{elementName}\" in type {typeInfo.Type.Name} on deserialize", reader);
        }
        else
        if (reader.NodeType == XmlNodeType.Element)
        {
          throw new XmlInternalException($"Unknown element \"{elementName}\" in type {typeInfo.Type.Name} on deserialize", reader);
        }
      });
      return kvPair;
    }

    public enum CollectionTypeKind
    {
      Array,
      Collection,
      List,
      Dictionary,
    }

    public record KVPair
    {
      public object? Key { get; set; }
      public object? Value { get; set; }
    }

    public void ReadElementAsCollectionProperty(object obj, ArrayPropertyInfo arrayInfo, XmlTextReader reader)
    {
      if (arrayInfo.TypeInfo?.Type == null)
        throw new XmlInternalException($"Collection type at property {arrayInfo.PropInfo.Name}" +
          $" of type {obj.GetType().Name} unknown", reader);

      var collectionType = arrayInfo.TypeInfo.Type;

      if (reader.NodeType != XmlNodeType.Element)
        throw new XmlInternalException($"XmlReader must be at XmlElement on deserialize object", reader);
      var elementName = reader.Name;

      // all items will be read to this temporary list;
      List<object> tempList = new List<object>();

      var collection = arrayInfo.PropInfo.GetValue(obj);
      if (collection == null)
      { // Check if collection can be written - if not we will not be able to set the property
        if (!arrayInfo.PropInfo.CanWrite)
          throw new XmlInternalException($"Collection at property {arrayInfo.PropInfo.Name}" +
            $" of type {obj.GetType().Name} is  but readonly", reader);
      }


      CollectionTypeKind? collectionTypeKind = null;
      Type? itemType;
      Type? valueType = typeof(object);
      Type? keyType = typeof(object);
      if (collectionType.IsArray)
      {
        itemType = collectionType.GetElementType();
        if (itemType == null)
          throw new XmlInternalException($"Unknown item type of {collectionType} collection", reader);
        collectionTypeKind = CollectionTypeKind.Array;
      }
      else if (collectionType.IsDictionary(out keyType, out valueType))
      {
        if (keyType == null)
          throw new XmlInternalException($"Unknown key type of {collectionType} collection", reader);
        if (valueType == null)
          throw new XmlInternalException($"Unknown item type of {collectionType} collection", reader);
        collectionTypeKind = CollectionTypeKind.Dictionary;
        itemType = valueType;

      }
      else if (collectionType.IsList(out itemType))
      {
        if (itemType == null)
          throw new XmlInternalException($"Unknown item type of {collectionType} collection", reader);
        collectionTypeKind = CollectionTypeKind.List;
      }
      else if (collectionType.IsCollection(out itemType))
      {
        if (itemType == null)
          throw new XmlInternalException($"Unknown item type of {collectionType} collection", reader);
        collectionTypeKind = CollectionTypeKind.Collection;
      }

      if (collectionTypeKind == null)
        throw new XmlInternalException($"Invalid type kind of {collectionType} collection", reader);

      if (itemType == null)
        throw new XmlInternalException($"Unknown item type of {collectionType} collection", reader);

      if (collectionTypeKind == CollectionTypeKind.Dictionary)
      {
        var dictionaryInfo = arrayInfo as DictionaryPropertyInfo;
        if (dictionaryInfo == null)
          throw new XmlInternalException($"Collection of type {collectionType} must be marked with SerializationDictionaryInfo attribute", reader);
        if (reader.IsStartElement(arrayInfo.Name) && !reader.IsEmptyElement)
        {
          reader.Read();
          ReadDictionaryItems(tempList, keyType, valueType, dictionaryInfo, reader);
        }
      }
      else
      {
        if (reader.IsStartElement(arrayInfo.Name) && !reader.IsEmptyElement)
        {
          reader.Read();
          ReadCollectionItems(tempList, itemType, arrayInfo, reader);
        }
      }

      if (collection == null)
      {
        switch (collectionTypeKind)
        {
          case CollectionTypeKind.Array:
            var arrayObject = Array.CreateInstance(itemType, tempList.Count);
            for (int i = 0; i < tempList.Count; i++)
              arrayObject.SetValue(tempList[i], i);
            arrayInfo.PropInfo.SetValue(obj, arrayObject);
            break;

          case CollectionTypeKind.Collection:
            // We can't use non-generic ICollection interface because implementation of ICollection<T>
            // does implicate implementation of ICollection.
            object? newCollectionObject;
            if (collectionType.IsConstructedGenericType)
            {
              Type d1 = typeof(Collection<>);
              Type[] typeArgs = { itemType };
              Type newListType = d1.MakeGenericType(typeArgs);
              newCollectionObject = Activator.CreateInstance(newListType);
            }
            else
            {
              var constructor = collectionType.GetConstructor(new Type[0]);
              if (constructor == null)
                throw new XmlInternalException($"Collection type {collectionType} must have a parameterless public constructor", reader);
              newCollectionObject = constructor.Invoke(new object[0]);
            }
            if (newCollectionObject == null)
              throw new XmlInternalException($"Could not create a new instance of {collectionType} collection", reader);

            // ICollection has no Add method so we must localize this method using reflection.
            var addMethod = newCollectionObject.GetType().GetMethod("Add");
            if (addMethod == null)
              throw new XmlInternalException($"Could not get \"Add\" method of {collectionType} collection", reader);
            for (int i = 0; i < tempList.Count; i++)
              addMethod.Invoke(newCollectionObject, new object[] { tempList[i] });
            arrayInfo.PropInfo.SetValue(obj, newCollectionObject);
            break;

          case CollectionTypeKind.List:
            IList? newListObject;
            if (collectionType.IsConstructedGenericType)
            {
              Type d1 = typeof(List<>);
              Type[] typeArgs = { itemType };
              Type newListType = d1.MakeGenericType(typeArgs);
              newListObject = Activator.CreateInstance(newListType) as IList;
            }
            else
            {
              var constructor = collectionType.GetConstructor(new Type[0]);
              if (constructor == null)
                throw new XmlInternalException($"Collection type {collectionType} must have a parameterless public constructor", reader);
              newListObject = constructor.Invoke(new object[0]) as IList;
            }
            if (newListObject == null)
              throw new XmlInternalException($"Could not create a new instance of {collectionType} collection", reader);
            for (int i = 0; i < tempList.Count; i++)
              newListObject.Add(tempList[i]);
            arrayInfo.PropInfo.SetValue(obj, newListObject);
            break;

          case CollectionTypeKind.Dictionary:
            IDictionary? newDictionaryObject;
            if (collectionType.IsConstructedGenericType)
            {
              Type d1 = typeof(Dictionary<,>);
              Type[] typeArgs = { keyType, valueType };
              Type newListType = d1.MakeGenericType(typeArgs);
              newDictionaryObject = Activator.CreateInstance(newListType) as IDictionary;
            }
            else
            {
              var constructor = collectionType.GetConstructor(new Type[0]);
              if (constructor == null)
                throw new XmlInternalException($"Collection type {collectionType} must have a parameterless public constructor", reader);
              newDictionaryObject = constructor.Invoke(new object[0]) as IDictionary;
            }
            if (newDictionaryObject == null)
              throw new XmlInternalException($"Could not create a new instance of {collectionType} collection", reader);
            for (int i = 0; i < tempList.Count; i++)
            {
              KVPair? kvPair = tempList[i] as KVPair;
              if (kvPair != null && kvPair.Key != null)
                newDictionaryObject.Add(kvPair.Key, kvPair.Value);
            }
            arrayInfo.PropInfo.SetValue(obj, newDictionaryObject);
            break;

          default:
            throw new XmlInternalException($"Collection type {collectionType} is not implemented for creation", reader);
        }
      }
      else
      {
        switch (collectionTypeKind)
        {
          case CollectionTypeKind.Array:
            var arrayObject = collection as Array;
            if (arrayObject == null)
              throw new XmlInternalException($"Collection value at property {arrayInfo.PropInfo.Name} cannot be typecasted to Array", reader);
            if (arrayObject.Length == tempList.Count)
              for (int i = 0; i < tempList.Count; i++)
                arrayObject.SetValue(tempList[i], i);
            else
            if (!arrayInfo.PropInfo.CanWrite)
              throw new XmlInternalException($"Collection at property {arrayInfo.PropInfo.Name}" +
                $" is an array of different length than number of read items but is readonly and can't be changed", reader);
            var itemArray = Array.CreateInstance(itemType, tempList.Count);
            for (int i = 0; i < tempList.Count; i++)
              itemArray.SetValue(tempList[i], i);
            arrayInfo.PropInfo.SetValue(obj, itemArray);
            break;

          case CollectionTypeKind.Collection:
            // We can't cast a collection to non-generic ICollection because implementation of ICollection<T>
            // does implicate implementation of ICollection.
            object? collectionObject = collection;
            // ICollection has no Add method so we must localize this method using reflection.
            var addMethod = collectionObject.GetType().GetMethod("Add");
            if (addMethod == null)
              throw new XmlInternalException($"Could not get \"Add\" method of {collectionType} collection", reader);
            // We must do the same with Clear method.
            var clearMethod = collectionObject.GetType().GetMethod("Clear");
            if (clearMethod == null)
              throw new XmlInternalException($"Could not get \"Add\" method of {collectionType} collection", reader);

            clearMethod.Invoke(collectionObject, new object[0]);
            for (int i = 0; i < tempList.Count; i++)
              addMethod.Invoke(collectionObject, new object[] { tempList[i] });
            break;

          case CollectionTypeKind.List:
            IList? listObject = collection as IList;
            if (listObject == null)
              throw new XmlInternalException($"Collection value at property {arrayInfo.PropInfo.Name} cannot be typecasted to IList", reader);
            listObject.Clear();
            for (int i = 0; i < tempList.Count; i++)
              listObject.Add(tempList[i]);
            break;

          case CollectionTypeKind.Dictionary:
            IDictionary? dictionaryObject = collection as IDictionary;
            if (dictionaryObject == null)
              throw new XmlInternalException($"Collection value at property {arrayInfo.PropInfo.Name} cannot be typecasted to IDictionary", reader);
            dictionaryObject.Clear();
            for (int i = 0; i < tempList.Count; i++)
            {
              KVPair? kvPair = tempList[i] as KVPair;
              if (kvPair != null && kvPair.Key != null)
                dictionaryObject.Add(kvPair.Key, kvPair.Value);
            }
            break;

          default:
            throw new XmlInternalException($"Collection type {collectionType} is not implemented for set content", reader);
        }
      }
      reader.Read();
    }

    public int ReadCollectionItems(ICollection<object> collection, Type collectionItemType, ArrayPropertyInfo propertyArrayInfo, XmlTextReader reader)
    {
      int itemsRead = 0;
      while (reader.NodeType == XmlNodeType.Element)
      {
        var item = ReadElementAsItem(collectionItemType, propertyArrayInfo, reader);
        if (item != null)
          collection.Add(item);
        SkipWhitespaces(reader);
      }
      return itemsRead;
    }

    public int ReadDictionaryItems(ICollection<object> collection, Type collectionKeyType, Type collectionValueType, DictionaryPropertyInfo propertyDictionaryInfo, XmlTextReader reader)
    {
      int itemsRead = 0;
      while (reader.NodeType == XmlNodeType.Element)
      {
        var item = ReadElementAsDictionaryItem(collectionKeyType, collectionValueType, propertyDictionaryInfo, reader);
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

    public object? ReadValue(SerializationPropertyInfo serializationPropertyInfo, XmlTextReader reader)
    {
      var propType = serializationPropertyInfo.PropInfo.PropertyType;
      var typeConverter = serializationPropertyInfo.TypeConverter;
      return ReadValue(propType, typeConverter, reader);
    }

    public object? ReadValue(Type expectedType, TypeConverter? typeConverter, XmlTextReader reader)
    {
      var str = reader.ReadContentAsString();
      object? propValue;
      if (expectedType.Name.StartsWith("Nullable`1"))
        expectedType = expectedType.GetGenericArguments()[0];

      if (expectedType == typeof(string))
        propValue = str;
      else
      if (str == "")
      {
        if (typeConverter != null)
        {
          propValue = typeConverter.ConvertFromString(str);
          return propValue;
        }
        return null;
      }
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
      else if (expectedType == typeof(object))
      {
        propValue = str;
      }
      else
        throw new XmlInternalException($"Value type \"{expectedType}\" not supported for deserialization", reader);
      return propValue;
    }
    #endregion

  }
}

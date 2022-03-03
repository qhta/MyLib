using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Qhta.TypeUtils;
using Qhta.TestHelper;
using System.Collections;

namespace Qhta.Json.Converters
{
  public static class JsonSafeConvertExtensions
  {
    public static object ToObjectSafe(this JToken jToken, Type objectType, JsonSerializer jsonSerializer)
    {
      //return jToken.Value<object>();
      return jToken.ToObject(objectType);
    }

    public static (object key, object value) ToKeyValuePairSafe(this JToken jToken, Type keyType, Type valueType, JsonSerializer jsonSerializer)
    {
      if (jToken is JProperty jProperty)
      {
        object? key = null;
        if (keyType == typeof(string))
          key = jProperty.Name;
        else
          throw new InternalException($"Unsupported key type {keyType}.");
        object? value = null;
        if (valueType == typeof(string))
          value = jProperty.Value<string>();
        else if (valueType.IsClass)
          value = ToObjectSafe(jProperty.Value, valueType, jsonSerializer);
        else
          throw new InternalException($"Unsupported value type {valueType}.");
        return (key, value);
      }
      throw new InternalException($"JProperty expected.");
    }

    public static object ToObjectCollectionSafe(this JToken jToken, Type objectType)
    {
      return ToObjectCollectionSafe(jToken, objectType, JsonSerializer.CreateDefault());
    }

    public static object ToObjectCollectionSafe(this JToken jToken, Type objectType, JsonSerializer jsonSerializer)
    {
      var expectArray = typeof(System.Collections.IEnumerable).IsAssignableFrom(objectType);
      object result;
      if (jToken is JArray jArray)
      {
        if (!expectArray)
        {
          //to object via singe token
          if (jArray.Count == 0)
            return JValue.CreateNull().ToObject(objectType, jsonSerializer);

          if (jArray.Count == 1)
            return jArray.First.ToObject(objectType, jsonSerializer);
          var arrayResult = new List<object>();
          var arrayAttribute = objectType.GetCustomAttributes(true).OfType<JsonVariantArrayAttribute>().FirstOrDefault();
          var itemType = arrayAttribute?.ItemType ?? typeof(object);
          foreach (var token in jArray)
          {
            var item = token.ToObjectCollectionSafe(itemType, jsonSerializer);
            arrayResult.Add(item);
          }
          MethodInfo addObject = objectType.GetMethod("AddObject");
          if (addObject != null)
          {
            object resultObj = objectType.GetConstructor(new Type[0]).Invoke(new object[0]);
            foreach (var arrayItem in arrayResult)
              addObject.Invoke(resultObj, new object[] { arrayItem });
            return resultObj;
          }
          return arrayResult;
        }
        else if (jArray.Count==0)
          return Activator.CreateInstance(objectType);
      }
      else if (expectArray)
      {
        if (objectType.IsDictionary(out var keyType, out var valueType))
        {
          result = ToObjectDictionarySafe(jToken, keyType, valueType, jsonSerializer);
          return result;
        }
        else
        {
          //to object via JArray
          result = new JArray(jToken).ToObjectSafe(objectType, jsonSerializer);
          return result;
        }
      }

      result = jToken.ToObjectSafe(objectType, jsonSerializer);
      return result;
    }

    public static T ToObjectCollectionSafe<T>(this JToken jToken)
    {
      return (T)ToObjectCollectionSafe(jToken, typeof(T));
    }

    public static T ToObjectCollectionSafe<T>(this JToken jToken, JsonSerializer jsonSerializer)
    {
      return (T)ToObjectCollectionSafe(jToken, typeof(T), jsonSerializer);
    }

    public static object ToObjectDictionarySafe(this JToken jToken, Type keyType, Type valueType, JsonSerializer jsonSerializer)
    {
      Type dType = typeof(Dictionary<,>);
      Type[] dTypeArgs = { keyType, valueType };
      Type dictionaryType = dType.MakeGenericType(dTypeArgs);
      Type iType = typeof(KeyValuePair<,>);
      Type[] iTypeArgs = { keyType, valueType };
      Type itemType = iType.MakeGenericType(iTypeArgs);

      var expectArray = typeof(System.Collections.IEnumerable).IsAssignableFrom(dictionaryType);
      if (jToken is JArray jArray)
      {
        if (!expectArray)
        {
          //to object via singe token
          if (jArray.Count == 0)
            return JValue.CreateNull().ToObject(dictionaryType, jsonSerializer);

          if (jArray.Count == 1)
            return jArray.First.ToObject(dictionaryType, jsonSerializer);
          var arrayResult = new List<object>();
          //var arrayAttribute = dictionaryType.GetCustomAttributes(true).OfType<JsonDictionaryAttribute>().FirstOrDefault();
          //var itemType = arrayAttribute?. ?? typeof(object);
          foreach (var token in jArray)
          {
            var item = token.ToObjectCollectionSafe(itemType, jsonSerializer);
            arrayResult.Add(item);
          }
          MethodInfo addObject = dictionaryType.GetMethod("AddObject");
          if (addObject != null)
          {
            object resultObj = dictionaryType.GetConstructor(new Type[0]).Invoke(new object[0]);
            foreach (var arrayItem in arrayResult)
              addObject.Invoke(resultObj, new object[] { arrayItem });
            return resultObj;
          }
          return arrayResult;
        }
      }
      else if (expectArray)
      {
        var dictionary = (IDictionary)Activator.CreateInstance(dictionaryType);
        var token = jToken.FirstOrDefault();
        for (int i = 0; i < jToken.Count(); i++)
        {
          if (token != null)
          {
            var kvpair = token.ToKeyValuePairSafe(keyType, valueType, jsonSerializer);
            dictionary.Add(kvpair.key, kvpair.value);
          }
          token = token.Next;
        }
        return dictionary;
      }

      var result = jToken.ToObjectSafe(dictionaryType, jsonSerializer);
      return result;
    }

    public static T ToObjectDictionarySafe<T>(this JToken jToken)
    {
      return (T)ToObjectCollectionSafe(jToken, typeof(T));
    }

    public static T ToObjectDictionarySafe<T>(this JToken jToken, JsonSerializer jsonSerializer)
    {
      return (T)ToObjectCollectionSafe(jToken, typeof(T), jsonSerializer);
    }
  }
}

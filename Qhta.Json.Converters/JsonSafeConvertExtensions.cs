using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Qhta.TypeUtils;
using Qhta.TestHelper;
using System.Collections;
using System.Globalization;

namespace Qhta.Json.Converters
{
  public static class JsonSafeConvertExtensions
  {
    public static object? ToObjectSafe(this JToken jToken, Type objectType, JsonSerializer jsonSerializer)
    {
      //return jToken.Value<object>();
      return jToken.ToObject(objectType);
    }

    public static (object? key, object? value) ToKeyValuePairSafe(this JToken jToken, Type keyType, Type valueType, JsonSerializer jsonSerializer)
    {
      if (jToken is JProperty jProperty)
      {
        object? key;
        if (keyType == typeof(string))
          key = jProperty.Name;
        else
        if (keyType == typeof(DateTime))
          key = DateTime.Parse(jProperty.Name);
        else
          throw new InternalException($"Unsupported key type {keyType}.");
        if (valueType.IsNullable(out var baseType))
          valueType = baseType;
        object? value = null;
        if (valueType.IsClass)
          value = ToObjectSafe(jProperty.Value, valueType, jsonSerializer);
        else
        {
          var valStr = jProperty.Value.ToString();
          if (valStr == "null" || valStr == "")
            valStr = null;
          if (valStr != null)
          {
            if (valueType == typeof(string))
              value = valStr;
            else
            if (valueType == typeof(int))
              value = Int32.Parse(valStr);
            else
            if (valueType == typeof(Int64))
              value = Int64.Parse(valStr);
            else
            if (valueType == typeof(Single))
              value = Single.Parse(valStr, CultureInfo.InvariantCulture);
            else
            if (valueType == typeof(Double))
              value = Double.Parse(valStr, CultureInfo.InvariantCulture);
            else
              throw new InternalException($"Unsupported value type {valueType}.");
          }
        }
        return (key, value);
      }
      throw new InternalException($"JProperty expected.");
    }

    public static object? ToObjectCollectionSafe(this JToken jToken, Type objectType)
    {
      return ToObjectCollectionSafe(jToken, objectType, JsonSerializer.CreateDefault());
    }

    public static object? ToObjectCollectionSafe(this JToken jToken, Type objectType, JsonSerializer jsonSerializer)
    {
      var expectArray = typeof(System.Collections.IEnumerable).IsAssignableFrom(objectType);
      object? result;
      if (jToken is JArray jArray)
      {
        if (!expectArray)
        {
          //to object via singe token
          if (jArray.Count == 0)
            return JValue.CreateNull().ToObject(objectType, jsonSerializer);

          if (jArray.Count == 1)
            return jArray.First?.ToObject(objectType, jsonSerializer);
          var arrayResult = new List<object>();
          var arrayAttribute = objectType.GetCustomAttributes(true).OfType<JsonVariantArrayAttribute>().FirstOrDefault();
          var itemType = arrayAttribute?.ItemType ?? typeof(object);
          foreach (var token in jArray)
          {
            var item = token.ToObjectCollectionSafe(itemType, jsonSerializer);
            if (item != null)
              arrayResult.Add(item);
          }
          MethodInfo? addObject = objectType?.GetMethod("AddObject");
          if (addObject != null)
          {
            object? resultObj = objectType?.GetConstructor(new Type[0])?.Invoke(new object[0]);
            if (resultObj != null)
              foreach (var arrayItem in arrayResult)
                addObject.Invoke(resultObj, new object[] { arrayItem });
            return resultObj;
          }
          return arrayResult;
        }
        else if (jArray.Count == 0)
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

    public static object? ToObjectDictionarySafe(this JToken jToken, Type keyType, Type valueType, JsonSerializer jsonSerializer)
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
            return jArray.First?.ToObject(dictionaryType, jsonSerializer);
          var arrayResult = new List<object>();
          foreach (var token in jArray)
          {
            var item = token.ToObjectCollectionSafe(itemType, jsonSerializer);
            if (item != null)
              arrayResult.Add(item);
          }
          MethodInfo? addObject = dictionaryType?.GetMethod("AddObject");
          if (addObject != null)
          {
            object? resultObj = dictionaryType?.GetConstructor(new Type[0])?.Invoke(new object[0]);
            if (resultObj != null)
              foreach (var arrayItem in arrayResult)
                addObject.Invoke(resultObj, new object[] { arrayItem });
            return resultObj;
          }
          return arrayResult;
        }
      }
      else if (expectArray)
      {
        var dictionary = Activator.CreateInstance(dictionaryType) as IDictionary;
        if (dictionary != null)
        {
          var token = jToken.FirstOrDefault();
          for (int i = 0; i < jToken.Count(); i++)
          {
            if (token != null)
            {
              var kvpair = token.ToKeyValuePairSafe(keyType, valueType, jsonSerializer);
              if (kvpair.key != null)
                dictionary.Add(kvpair.key, kvpair.value);
              token = token.Next;
            }
          }
        }
        return dictionary;
      }

      var result = jToken.ToObjectSafe(dictionaryType, jsonSerializer);
      return result;
    }

  }
}

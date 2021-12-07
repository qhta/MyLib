using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Qhta.Json.Converters
{
  public static class JsonSafeConvertExtensions
  {
    public static object ToObjectCollectionSafe(this JToken jToken, Type objectType)
    {
      return ToObjectCollectionSafe(jToken, objectType, JsonSerializer.CreateDefault());
    }

    public static object ToObjectCollectionSafe(this JToken jToken, Type objectType, JsonSerializer jsonSerializer)
    {
      var expectArray = typeof(System.Collections.IEnumerable).IsAssignableFrom(objectType);

      if (jToken is JArray jArray)
      {
        if (!expectArray)
        {
          //to object via singel
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
      }
      else if (expectArray)
      {
        //to object via JArray
        return new JArray(jToken).ToObject(objectType, jsonSerializer);
      }

      var result = jToken.ToObject(objectType, jsonSerializer);
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
  }
}

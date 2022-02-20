using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// from: https://stackoverflow.com/questions/43873024/newtonsoft-json-deserialize-xmlanyattribute
/// </summary>
namespace Qhta.Json.Converters
{
  public class JsonAnyAttributeConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType.GetInterface("IDictionary")!=null && objectType.GetConstructor(new Type[0])!=null;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return null;
      var dict = serializer.Deserialize<Dictionary<string, string>>(reader);
      var constructor = objectType.GetConstructor(new Type[0]);
      if (constructor !=null)
      {
        var obj = constructor.Invoke(new object[0]) as IDictionary<string, string>;
        foreach (var item in dict)
          obj.Add(item.Key, item.Value);
        return obj;
      }
      return dict;
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }
  }
}

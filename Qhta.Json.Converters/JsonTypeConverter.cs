using Newtonsoft.Json;

using System;
using System.Diagnostics;

namespace Qhta.Json.Converters
{
  public class JsonTypeConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      var result = objectType.IsSubclassOf(typeof(System.Type));
      Debug.WriteLine(objectType);
      return result;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      return existingValue == null || (string)existingValue == "";
    }

    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      writer.WriteValue((value as Type).Name);
    }
  }
}

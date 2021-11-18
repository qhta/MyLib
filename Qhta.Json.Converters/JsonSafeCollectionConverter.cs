using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

/// <summary>
/// from: https://stackoverflow.com/questions/27047875/json-net-deserialization-single-result-vs-array
/// </summary>
namespace Qhta.Json.Converters
{
  public class JsonSafeCollectionConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return true;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      //This not works for Populate (on existingValue)
      return serializer.Deserialize<JToken>(reader).ToObjectCollectionSafe(objectType, serializer);
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }
  }
}

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

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
      try
      {
        var jToken = serializer.Deserialize<JToken>(reader);
        return jToken?.ToObjectCollectionSafe(objectType, serializer);
      }
      catch
      {
      }
      return null;
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }
  }
}

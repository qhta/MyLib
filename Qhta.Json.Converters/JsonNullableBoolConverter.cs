using Newtonsoft.Json;

using Qhta.TestHelper;

using System;

/// <summary>
/// from: https://stackoverflow.com/questions/27047875/json-net-deserialization-single-result-vs-array
/// </summary>
namespace Qhta.Json.Converters
{
  public class JsonNullableBoolConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return true;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      if (existingValue == null)
        return true;
      if (existingValue is bool boolValue)
        return boolValue;
      throw new InternalException($"Cannot convert {existingValue} to bool");
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }
  }
}

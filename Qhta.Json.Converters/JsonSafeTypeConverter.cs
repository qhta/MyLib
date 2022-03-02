using Newtonsoft.Json;

using Qhta.TestHelper;
using System.Reflection;
using System;

namespace Qhta.Json.Converters
{
  public class JsonSafeTypeConverter : JsonConverter
  {
    public JsonSafeTypeConverter(Type valueType)
    {
      ValueType = valueType;
    }

    public Type ValueType {get; init;}

    public override bool CanConvert(Type objectType)
    {
      return true;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      if (existingValue == null)
      {
        if (reader.TokenType == JsonToken.String)
          existingValue = reader.Value;
      }
      if (existingValue is string strValue)
      {
        var parseMethod = ValueType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, new Type[]{ typeof(string) });
        if (parseMethod == null)
          throw new InternalException($"Static \"Parse(string)\" method not found in {ValueType.Name} type");
        return parseMethod.Invoke(null, new object[]{ strValue});
      }
      throw new InternalException($"Cannot convert {existingValue} to {ValueType.Name}");
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }
  }
}

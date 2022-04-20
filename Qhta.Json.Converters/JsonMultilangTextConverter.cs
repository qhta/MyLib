using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qhta.Json.Converters
{
  public class JsonMultilangTextConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return true;
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
      var jToken = serializer.Deserialize<JToken>(reader);
      if (jToken is JValue jValue)
      {
        var s = jValue.Value;
        if (s!=null)
          return new MultilangText{ Text = s.ToString() };
      }
      objectType = typeof(Dictionary<string, string>);
      var obj = jToken?.ToObjectCollectionSafe(objectType, serializer) as Dictionary<string, string>;
      var result = new MultilangText{ MultiText = obj };
      return result;
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }
  }
}

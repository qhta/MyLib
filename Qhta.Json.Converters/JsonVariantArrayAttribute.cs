using System;

namespace Qhta.Json.Converters
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
  public class JsonVariantArrayAttribute : Attribute
  {

    public JsonVariantArrayAttribute(Type itemType)
    {
      ItemType = itemType;
    }

    public Type ItemType { get; set; }
  }
}

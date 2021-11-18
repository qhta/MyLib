using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Json.Converters
{
  [AttributeUsage( AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited =true)]
  public class JsonVariantArrayAttribute: Attribute
  {

    public JsonVariantArrayAttribute(Type itemType)
    {
      ItemType = itemType;
    }

    public Type ItemType { get; set; }
  }
}

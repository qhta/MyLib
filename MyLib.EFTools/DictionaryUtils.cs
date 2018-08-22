using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.EFTools
{
  public static class DictionaryUtils
  {
    public static valueType FindOrDefault<valueType>(this Dictionary<string, valueType> dictionary, string key)
    {
      valueType value;
      if (key!=null && dictionary.TryGetValue(key, out value))
        return value;
      return default(valueType);
    }

    public static int? ZeroToNull(this int value)
    {
      if (value == 0)
        return null;
      return value;
    }
  }
}

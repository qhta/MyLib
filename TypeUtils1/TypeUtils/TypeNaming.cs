using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeUtils
{
  public static class TypeNaming
  {
    static Dictionary<Type, string> typeNames = new Dictionary<Type, string>()
    {
      { typeof(string), "string" },
      { typeof(char), "char" },
      { typeof(bool), "bool" },
      { typeof(sbyte), "sbyte" },
      { typeof(Int16), "int16" },
      { typeof(Int32), "int" },
      { typeof(Int64), "int64" },
      { typeof(byte),   "byte" },
      { typeof(UInt16), "uint16" },
      { typeof(UInt32), "uint" },
      { typeof(UInt64), "uint64" },
      { typeof(Decimal), "decimal" },
      { typeof(float),   "float" },
      { typeof(double),  "double" },
      { typeof(DateTime),  "date&time" },
      { typeof(TimeSpan),  "time" },
    };
    public static string GetTypeName(this Type type)
    {
      string name;
      if (typeNames.TryGetValue(type, out name))
        return name;
      if (type.Name.StartsWith("Nullable`"))
      {
        Type baseType = type.GenericTypeArguments[0];
        name = GetTypeName(baseType);
        return name;
      }
      return type.Name;
    }
  }

}

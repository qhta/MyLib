using System;
using System.Collections.Generic;

namespace Qhta.TypeUtils;

/// <summary>
/// More friendy type names
/// </summary>
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
    { typeof(DateTime),  "DateTime" },
    { typeof(TimeSpan),  "TimeSpan" },
  };

  /// <summary>
  /// Getting specific name of the type
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public static string GetTypeName(this Type type)
  {
    string name;
    if (typeNames.TryGetValue(type, out name))
      return name;
    name = type.Name;
    if (name.StartsWith("Nullable`"))
    {
      Type baseType = type.GenericTypeArguments[0];
      name = GetTypeName(baseType);
      return name+"?";
    }
    int k = name.IndexOf('`');
    if (k>0)
    {
      var nstr = name.Substring(k+1);
      if (Int32.TryParse(nstr, out int n))
      {
        name = name.Substring(0, k);
        var argTypeNames = new List<string>();
        for (int i = 0; i<n; i++)
        {
          Type baseType = type.GenericTypeArguments[0];
          argTypeNames.Add(GetTypeName(baseType));
        }
        return $"{name}<{String.Join(",",argTypeNames)}>";
      }
    }
    return name;
  }
}
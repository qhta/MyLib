using System;
using System.Collections.Generic;

using Qhta.Collections;

namespace Qhta.TypeUtils;

/// <summary>
///   More friendy type names
/// </summary>
public static class TypeNaming
{
  /// <summary>
  /// The simple type names
  /// </summary>
  public static BiDiDictionary<Type, string> TypeNames = new()
  {
    { typeof(string), "string" },
    { typeof(char), "char" },
    { typeof(bool), "bool" },
    { typeof(sbyte), "sbyte" },
    { typeof(short), "short" },
    { typeof(int), "int" },
    { typeof(long), "long" },
    { typeof(byte), "byte" },
    { typeof(ushort), "ushort" },
    { typeof(uint), "uint" },
    { typeof(ulong), "ulong" },
    { typeof(decimal), "decimal" },
    { typeof(float), "float" },
    { typeof(double), "double" },
    { typeof(DateTime), "dateTime" },
    { typeof(TimeSpan), "timeSpan" },
#if NET6_0_OR_GREATER
    { typeof(DateOnly), "date" },
    { typeof(TimeOnly), "time" },
#endif
    { typeof(Guid), "guid" }
  };

  /// <summary>
  ///   Getting specific name of the type.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public static string GetTypeName(this Type type)
  {
    string? name;
    if (TypeNames.TryGetValue2(type, out name))
      return name;
    if (type.Name.StartsWith("Nullable`"))
    {
      var baseType = type.GenericTypeArguments[0];
      name = GetTypeName(baseType);
      return name + "?";
    }
    name = type.FullName;
    var k = name.IndexOf('`');
    if (k > 0)
    {
      var nstr = name.Substring(k + 1);
      if (Int32.TryParse(nstr, out var n))
      {
        name = name.Substring(0, k);
        var argTypeNames = new List<string>();
        for (var i = 0; i < n; i++)
        {
          var baseType = type.GenericTypeArguments[0];
          argTypeNames.Add(GetTypeName(baseType));
        }
        return $"{name}<{String.Join(",", argTypeNames)}>";
      }
    }
    return name;
  }

    /// <summary>
  ///   Getting specific type from the name. NetStandard handles NullableTypes
  /// </summary>
  /// <param name="typeName"></param>
  /// <returns></returns>
  public static Type? GetType(string typeName)
  {
    var nullable = typeName.EndsWith("?");
    Type? type = null;
    if (!TypeNames.TryGetValue1(typeName, out type))
    {
      type = Type.GetType(typeName);
      if (nullable && type!=null)
      {
#if NET6_0_OR_GREATER
        return Type.MakeGenericSignatureType(typeof(Nullable<>), new Type[] { type });
#endif
      }
      return type;
    }
    return type;
  }
}
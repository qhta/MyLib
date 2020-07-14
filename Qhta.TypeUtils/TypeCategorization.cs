using System;
using System.Collections.Generic;

namespace Qhta.TypeUtils
{
  /// <summary>
  /// Specific categories of types
  /// </summary>
  public enum TypeCategory
  {
    /// <summary>
    /// A simple type (type has no members)
    /// </summary>
    Simple = 1,
    /// <summary>
    /// String or character type
    /// </summary>
    Textual = 2,
    /// <summary>
    /// logical operations enabled (e.g. boolean)
    /// </summary>
    Logical = 4,
    /// <summary>
    /// Arithmetic operations enabled
    /// </summary>
    Numeral = 8,
    /// <summary>
    /// Signed numeral
    /// </summary>
    Signed = 16,
    /// <summary>
    /// Unsigned numeral
    /// </summary>
    Unsigned = 32,
    /// <summary>
    /// Integer numeral
    /// </summary>
    Integer = 64,
    /// <summary>
    /// Fixed decimal position numeral
    /// </summary>
    Fixed = 128,
    /// <summary>
    /// Float decimal position numeral
    /// </summary>
    Float = 256, 
    /// <summary>
    /// DateTime or TimeSpan
    /// </summary>
    Temporal = 512,
    /// <summary>
    /// Enumerable type
    /// </summary>
    Enumerable = 1024,
    /// <summary>
    /// Nullable type
    /// </summary>
    Nullable = 2048,
  }

  /// <summary>
  /// A static class that evaluates category of a type
  /// </summary>
  public static class TypeCategorization
  {
    static Dictionary<Type, TypeCategory> typeCategories = new Dictionary<Type, TypeCategory>()
    {
      { typeof(string), TypeCategory.Simple | TypeCategory.Textual },
      { typeof(char), TypeCategory.Simple | TypeCategory.Textual },
      { typeof(bool), TypeCategory.Simple | TypeCategory.Logical },
      { typeof(sbyte), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Signed | TypeCategory.Integer },
      { typeof(Int16), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Signed | TypeCategory.Integer },
      { typeof(Int32), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Signed | TypeCategory.Integer },
      { typeof(Int64), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Signed | TypeCategory.Integer },
      { typeof(byte),   TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Unsigned | TypeCategory.Integer },
      { typeof(UInt16), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Unsigned | TypeCategory.Integer },
      { typeof(UInt32), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Unsigned | TypeCategory.Integer },
      { typeof(UInt64), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Unsigned | TypeCategory.Integer },
      { typeof(Decimal), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Signed | TypeCategory.Fixed },
      { typeof(float),   TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Signed | TypeCategory.Float },
      { typeof(double),  TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Signed | TypeCategory.Float },
      { typeof(DateTime),  TypeCategory.Simple | TypeCategory.Temporal },
      { typeof(TimeSpan),  TypeCategory.Simple | TypeCategory.Temporal },
    };

    /// <summary>
    /// Categorization of a type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static TypeCategory GetCategory(this Type type)
    {
      TypeCategory category;
      if (typeCategories.TryGetValue(type, out category))
        return category;
      if (type.IsEnum)
        return TypeCategory.Simple | TypeCategory.Enumerable;
      if (type.Name.StartsWith("Nullable`"))
      {
        Type baseType = type.GenericTypeArguments[0];
        category = GetCategory(baseType);
        return category | TypeCategory.Nullable;
      }
      return 0;
    }

    /// <summary>
    /// Is it a simple type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsSimple(this Type type)
    {
      TypeCategory category = GetCategory(type);
      return category.HasFlag(TypeCategory.Simple);
    }

    /// <summary>
    /// Is it a nullable type of some specific base type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="baseType"></param>
    /// <returns></returns>
    public static bool IsNullable(this Type type, out Type baseType)
    {
      if (type.Name.StartsWith("Nullable`"))
      {
        baseType = type.GenericTypeArguments[0];
        return true;
      }
      baseType = null;
      return false;
    }

    /// <summary>
    /// Is it a nullable type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsNullable(this Type type)
    {
      if (type.Name.StartsWith("Nullable`"))
      {
        return true;
      }
      return false;
    }

    /// <summary>
    /// Is it a numeral type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsNumeral(this Type type)
    {
      TypeCategory category = type.GetCategory();
      return category.HasFlag(TypeCategory.Numeral);
    }
  }
}

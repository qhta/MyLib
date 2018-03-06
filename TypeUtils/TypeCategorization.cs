using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.TypeUtils
{
  public enum TypeCategory
  {
    Simple = 1,
    Textual = 2,
    Logical = 4,
    Numeral = 8,
    Signed = 16,
    Unsigned = 32,
    Integer = 64,
    Fixed = 128,
    Float = 256, 
    Temporal = 512,
    Enumerable = 1024,
    Nullable = 2048,
  }

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

    public static bool IsSimple(this Type type)
    {
      TypeCategory category = GetCategory(type);
      return category.HasFlag(TypeCategory.Simple);
    }

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

    public static bool IsNullable(this Type type)
    {
      if (type.Name.StartsWith("Nullable`"))
      {
        return true;
      }
      return false;
    }

    public static bool IsNumeral(this Type type)
    {
      TypeCategory category = type.GetCategory();
      return category.HasFlag(TypeCategory.Numeral);
    }
  }
}

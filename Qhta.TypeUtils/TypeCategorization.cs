using System;
using System.Collections.Generic;
using System.Linq;

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
      { typeof(Guid),  TypeCategory.Simple },
    };

    /// <summary>
    /// Categorization of a type
    /// </summary>
    /// <param name="aType">checked type</param>
    /// <returns>a <see cref="TypeCategory"/> of a type or 0 if not known</returns>
    public static TypeCategory GetCategory(this Type aType)
    {
      TypeCategory category;
      if (typeCategories.TryGetValue(aType, out category))
        return category;
      if (aType.IsEnum)
        return TypeCategory.Simple | TypeCategory.Enumerable;
      if (aType.Name.StartsWith("Nullable`"))
      {
        Type baseType = aType.GenericTypeArguments[0];
        category = GetCategory(baseType);
        return category | TypeCategory.Nullable;
      }
      return 0;
    }

    /// <summary>
    /// Is a type a simple type? A simple type is string, char, boolean, all numeral types, date/time, time span and guid type.
    /// Also an enum type is a simple type.
    /// </summary>
    /// <param name="aType">checked type</param>
    /// <returns>true if a type is a simple type</returns>
    public static bool IsSimple(this Type aType)
    {
      TypeCategory category = GetCategory(aType);
      return category.HasFlag(TypeCategory.Simple) || aType.IsEnum;
    }

    /// <summary>
    /// Is a type a textual type, i.e. string or char type
    /// </summary>
    /// <param name="aType">checked type</param>
    /// <returns>true if a type is a textual type</returns>
    public static bool IsTextual(this Type aType)
    {
      TypeCategory category = GetCategory(aType);
      return category.HasFlag(TypeCategory.Textual);
    }

    /// <summary>
    /// Is a type a nullable type, i.e. it's name starts with "Nullable`1".
    /// </summary>
    /// <returns>true if a type is a nullable type</returns>
    public static bool IsNullable(this Type aType)
    {
      if (aType.Name.StartsWith("Nullable`1"))
      {
        return true;
      }
      return false;
    }

    /// <summary>
    /// Checks if a type is a nullable type, i.e. it's name starts with "Nullable`1"
    /// and returns it's base type
    /// </summary>
    /// <param name="aType">checked type</param>
    /// <param name="baseType">based type of the nullable type</param>
    /// <returns>true if a type is a nullable type</returns>
    public static bool IsNullable(this Type aType, out Type baseType)
    {
      if (aType.Name.StartsWith("Nullable``"))
      {
        baseType = aType.GenericTypeArguments[0];
        return true;
      }
      baseType = null;
      return false;
    }


    /// <summary>
    /// Is a type a numeral type, i.e. integer or float or decimal type
    /// </summary>
    /// <param name="aType">checked type</param>
    /// <returns>true if a type is a numeral type</returns>
    public static bool IsNumeral(this Type aType)
    {
      TypeCategory category = aType.GetCategory();
      return category.HasFlag(TypeCategory.Numeral);
    }

    /// <summary>
    /// Is a type a list type, i.e. it's name starts with "List`1"
    /// or if it is a defined type which implements a IList`1 interface.
    /// </summary>
    /// <param name="aType">checked type</param>
    /// <returns>true if a type is a list type</returns>
    public static bool IsList(this Type aType)
    {
      return aType.Name.StartsWith("List`1") || aType.GetInterfaces().FirstOrDefault(item=>item.Name.StartsWith("IList`1"))!=null;
    }

    /// <summary>
    /// Checks if a type a list type, i.e. it's name starts with "List`1"
    /// or if it is a defined type which implements a IList`1 interface.
    /// If so it returns the item type of the list.
    /// </summary>
    /// <param name="aType">checked type</param>
    /// <param name="itemType">item type if a type is a list</param>
    /// <returns>true if a type is a list type</returns>
    public static bool IsList(this Type aType, out Type itemType)
    {
      if (aType.Name.StartsWith("List`1"))
      {
        itemType = aType.GenericTypeArguments[0];
        return true;
      }
      var intf = aType.GetInterfaces().FirstOrDefault(item => item.Name.StartsWith("IList`1"));
      if (intf!=null)
      { 
        itemType = intf.GenericTypeArguments[0];
        return true;
      }
      itemType = null;
      return false;
    }

    /// <summary>
    /// Is a type a collection type, i.e. it's name starts with "Collection`1"
    /// or if it is a defined type which implements a ICollection`1 interface.
    /// </summary>
    /// <param name="aType">checked type</param>
    /// <returns>true if a type is a collection type</returns>
    public static bool IsCollection(this Type aType)
    {
      return aType.Name.StartsWith("Collection`1") || aType.GetInterfaces().FirstOrDefault(item => item.Name.StartsWith("ICollection`1")) != null;
    }

    /// <summary>
    /// Is a type a collection type, i.e. it's name starts with "Collection`1"
    /// or if it is a defined type which implements a ICollection`1 interface.
    /// If so it returns the item type of the collection.
    /// </summary>
    /// <param name="aType">checked type</param>
    /// <param name="itemType">item type if a type is a collection</param>
    /// <returns>true if a type is a collection type</returns>
    public static bool IsCollection(this Type aType, out Type itemType)
    {
      if (aType.Name.StartsWith("Collection`1"))
      {
        itemType = aType.GenericTypeArguments[0];
        return true;
      }
      var intf = aType.GetInterfaces().FirstOrDefault(item => item.Name.StartsWith("ICollection`1"));
      if (intf != null)
      {
        itemType = intf.GenericTypeArguments[0];
        return true;
      }
      itemType = null;
      return false;
    }

    /// <summary>
    /// Is a type an enumerable type, i.e. it's name starts with "Enumerable`1"
    /// or if it is a defined type which implements a IEnumerable`1 interface.
    /// </summary>
    /// <param name="aType">checked type</param>
    /// <returns>true if a type is an enumerable type</returns>
    public static bool IsEnumerable(this Type aType)
    {
      return aType.Name.StartsWith("Enumerable`1") || aType.GetInterfaces().FirstOrDefault(item => item.Name.StartsWith("IEnumerable`1")) != null;
    }

    /// <summary>
    /// Is a type an enumerable type, i.e. it's name starts with "Enumerable`1"
    /// or if it is a defined type which implements a IEnumerable`1 interface.
    /// If so it returns the item type of the collection.
    /// </summary>
    /// <param name="aType">checked type</param>
    /// <param name="itemType">item type if a type is am enumerable</param>
    /// <returns>true if a type is an enumerable type</returns>
    public static bool IsEnumerable(this Type aType, out Type itemType)
    {
      if (aType.Name.StartsWith("Enumerable`1"))
      {
        itemType = aType.GenericTypeArguments[0];
        return true;
      }
      var intf = aType.GetInterfaces().FirstOrDefault(item => item.Name.StartsWith("IEnumerable`1"));
      if (intf != null)
      {
        itemType = intf.GenericTypeArguments[0];
        return true;
      }
      itemType = null;
      return false;
    }

    /// <summary>
    /// Is a type a dictionary type, i.e. it's name starts with "Dictionary`2"
    /// or if it is a defined type which implements a IDictionary`2 interface.
    /// </summary>
    /// <param name="aType">checked type</param>
    /// <returns>true if a type is a dictionary type</returns>
    public static bool IsDictionary(this Type aType)
    {
      return aType.Name.StartsWith("Dictionary`2") || aType.GetInterfaces().FirstOrDefault(item => item.Name.StartsWith("IDictionary`2")) != null;
    }

    /// <summary>
    /// Is a type a dictionary type, i.e. it's name starts with "Dictionary`2"
    /// or if it is a defined type which implements a IDictionary`2 interface.
    /// If so it returns the key type and the value type of the dictionary.
    /// </summary>
    /// <param name="aType">checked type</param>
    /// <param name="keyType">item type if a type is a list</param>
    /// <param name="valueType">item type if a type is a list</param>
    /// <returns>true if a type is a dictionary type</returns>
    public static bool IsDictionary(this Type aType, out Type keyType, out Type valueType)
    {
      if (aType.Name.StartsWith("Dictionary`2"))
      {
        keyType = aType.GenericTypeArguments[0];
        valueType = aType.GenericTypeArguments[1];
        return true;
      }
      var intf = aType.GetInterfaces().FirstOrDefault(item => item.Name.StartsWith("IDictionary`2"));
      if (intf != null)
      {
        keyType = intf.GenericTypeArguments[0];
        valueType = intf.GenericTypeArguments[1];
        return true;
      }
      keyType = null;
      valueType = null;
      return false;
    }
  }
}

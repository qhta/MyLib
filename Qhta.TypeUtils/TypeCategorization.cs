using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Qhta.TypeUtils;

/// <summary>
///   Specific categories of types
/// </summary>
public enum TypeCategory
{
  /// <summary>
  ///   A simple type (type has no members)
  /// </summary>
  Simple = 1,

  /// <summary>
  ///   String or character type
  /// </summary>
  Textual = 2,

  /// <summary>
  ///   logical operations enabled (e.g. boolean)
  /// </summary>
  Logical = 4,

  /// <summary>
  ///   Arithmetic operations enabled
  /// </summary>
  Numeral = 8,

  /// <summary>
  ///   Signed numeral
  /// </summary>
  Signed = 16,

  /// <summary>
  ///   Unsigned numeral
  /// </summary>
  Unsigned = 32,

  /// <summary>
  ///   Integer numeral
  /// </summary>
  Integer = 64,

  /// <summary>
  ///   Fixed decimal position numeral
  /// </summary>
  Fixed = 128,

  /// <summary>
  ///   Float decimal position numeral
  /// </summary>
  Float = 256,

  /// <summary>
  ///   DateTime or TimeSpan
  /// </summary>
  Temporal = 512,

  /// <summary>
  ///   Enumerable type
  /// </summary>
  Enumerable = 1024,

  /// <summary>
  ///   Nullable type
  /// </summary>
  Nullable = 2048
}

/// <summary>
///   A static class that evaluates category of a type
/// </summary>
public static class TypeCategorization
{
  private static readonly Dictionary<Type, TypeCategory> typeCategories = new()
  {
    { typeof(string), TypeCategory.Simple | TypeCategory.Textual },
    { typeof(char), TypeCategory.Simple | TypeCategory.Textual },
    { typeof(bool), TypeCategory.Simple | TypeCategory.Logical },
    { typeof(sbyte), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Signed | TypeCategory.Integer },
    { typeof(Int16), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Signed | TypeCategory.Integer },
    { typeof(Int32), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Signed | TypeCategory.Integer },
    { typeof(Int64), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Signed | TypeCategory.Integer },
    { typeof(byte), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Unsigned | TypeCategory.Integer },
    { typeof(UInt16), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Unsigned | TypeCategory.Integer },
    { typeof(UInt32), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Unsigned | TypeCategory.Integer },
    { typeof(UInt64), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Unsigned | TypeCategory.Integer },
    { typeof(Decimal), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Signed | TypeCategory.Fixed },
    { typeof(float), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Signed | TypeCategory.Float },
    { typeof(double), TypeCategory.Simple | TypeCategory.Numeral | TypeCategory.Signed | TypeCategory.Float },
    { typeof(DateTime), TypeCategory.Simple | TypeCategory.Temporal },
    { typeof(TimeSpan), TypeCategory.Simple | TypeCategory.Temporal },
#if NET6_0_OR_GREATER
    { typeof(DateOnly),  TypeCategory.Simple | TypeCategory.Temporal },
    { typeof(TimeOnly),  TypeCategory.Simple | TypeCategory.Temporal },
#endif
    { typeof(Guid), TypeCategory.Simple }
  };

  /// <summary>
  ///   Categorization of a type
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <returns>a <see cref="TypeCategory" /> of a type or 0 if not known</returns>
  public static TypeCategory GetCategory(this Type aType)
  {
    TypeCategory category;
    if (typeCategories.TryGetValue(aType, out category))
      return category;
    if (aType.IsEnum)
      return TypeCategory.Simple | TypeCategory.Enumerable;
    if (aType.Name.StartsWith("Nullable`"))
    {
      var baseType = aType.GenericTypeArguments[0];
      category = GetCategory(baseType);
      return category | TypeCategory.Nullable;
    }
    return 0;
  }

  /// <summary>
  ///   Is a type a simple type? A simple type is string, char, boolean, all numeral types, date/time, time span and guid
  ///   type.
  ///   Also an enum type is a simple type.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <returns>true if a type is a simple type</returns>
  public static bool IsSimple(this Type aType)
  {
    var category = GetCategory(aType);
    return category.HasFlag(TypeCategory.Simple) || aType.IsEnum || aType.IsValueType;
  }

  /// <summary>
  ///   Is a type a textual type, i.e. string or char type
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <returns>true if a type is a textual type</returns>
  public static bool IsTextual(this Type aType)
  {
    var category = GetCategory(aType);
    return category.HasFlag(TypeCategory.Textual);
  }

  /// <summary>
  ///   Is a type a numeral type, i.e. integer or float or decimal type
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <returns>true if a type is a numeral type</returns>
  public static bool IsNumeral(this Type aType)
  {
    var category = aType.GetCategory();
    return category.HasFlag(TypeCategory.Numeral);
  }

  /// <summary>
  ///   Shortcut of type equality comparison or IsSubclassOf function
  /// </summary>
  /// <param name="thisType"></param>
  /// <param name="otherType"></param>
  /// <returns></returns>
  public static bool IsEqualOrSubclassOf(this Type thisType, Type otherType)
  {
    return thisType == otherType || thisType.IsSubclassOf(otherType);
  }

  /// <summary>
  ///   Shortcut of type equality comparison or IsSubclassOf or Implements function
  /// </summary>
  /// <param name="thisType"></param>
  /// <param name="otherType"></param>
  /// <returns></returns>
  public static bool IsEqualOrSubclassOfOrImplements(this Type thisType, Type otherType)
  {
    return thisType == otherType || thisType.IsSubclassOf(otherType) || thisType.Implements(otherType);
  }
  /// <summary>
  ///   Checks if a type implements an interface
  /// </summary>
  /// <param name="thisType"></param>
  /// <param name="intf"></param>
  /// <returns></returns>
  public static bool Implements(this Type thisType, Type intf)
  {
    return thisType.GetInterfaces().Contains(intf);
  }

  #region IsNullable
  /// <summary>
  ///   Is a type a nullable type, i.e. it's name starts with "Nullable`1".
  /// </summary>
  /// <returns>true if a type is a nullable type</returns>
  public static bool IsNullable(this Type aType)
  {
    if (aType.Name.StartsWith("Nullable`1")) return true;
    return false;
  }

  /// <summary>
  ///   Checks if a type is a nullable type, i.e. it's name starts with "Nullable`1"
  ///   and returns it's base type
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <param name="baseType">based type of the nullable type</param>
  /// <returns>true if a type is a nullable type</returns>
#if NET6_0_OR_GREATER
  public static bool IsNullable(this Type aType, [NotNullWhen(true)][MaybeNullWhen(false)] out Type? baseType)
#else
  public static bool IsNullable(this Type aType, out Type? baseType)
#endif
  {
    if (aType.Name.StartsWith("Nullable`1"))
    {
      baseType = aType.GenericTypeArguments[0];
      return true;
    }
    baseType = null;
    return false;
  }

  /// <summary>
  ///   Checks if a type is a nullable type of the base type.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <param name="baseType">based type of the nullable type</param>
  /// <returns>true if a type is a nullable type</returns>
  public static bool IsNullable(this Type aType, Type baseType)
  {
    if (aType.Name.StartsWith("Nullable`1"))
    {
      var aBaseType = aType.GenericTypeArguments[0];
      return aBaseType == baseType;
    }
    return false;
  }

  /// <summary>
  /// Gets the type of the object and if the type is Nullable, returns its baseType
  /// </summary>
  public static Type GetNotNullableType(this object obj)
  {
    Type type = obj.GetType() ?? typeof(object);
    if (type.IsNullable(out var baseType) && baseType!=null)
      type = baseType;
    return type;
  }

  /// <summary>
  /// Checks if the type is Nullable and returns its baseType.
  /// </summary>
  public static Type GetNotNullableType(this Type type)
  {
    if (type.IsNullable(out var baseType) && baseType!=null)
      type = baseType;
    return type;
  }
  #endregion

  #region IsArray

  /// <summary>
  ///   Is a type an array type.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <returns>true if a type is an array type</returns>
  public static bool IsArray(this Type aType)
  {
    return aType.IsArray;
  }

  /// <summary>
  ///   Is a type an array type.
  ///   If so it returns the item type of the array.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <param name="itemType">returned item type if a type is an array</param>
  /// <returns>true if a type is an array type</returns>
  /// 
#if NET6_0_OR_GREATER
  public static bool IsArray(this Type aType, [NotNullWhen(true)][MaybeNullWhen(false)] out Type? itemType)
#else
  public static bool IsArray(this Type aType, out Type? itemType)
#endif
  {
    if (aType.IsArray)
    {
      itemType = aType.GetElementType() ?? typeof(object);
      return true;
    }
    itemType = null;
    return false;
  }

  /// <summary>
  ///   Is a type an array type.
  ///   If so it check the item type of the array.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <param name="itemType">checked item type if a type is an array</param>
  /// <returns>true if a type is an array type of item Type</returns>
  public static bool IsArray(this Type aType, Type itemType)
  {
    return (IsArray(aType, out var foundItemType)
            && foundItemType == itemType) || (foundItemType != null && itemType.IsSubclassOf(foundItemType));
  }

  #endregion

  #region IsList

  /// <summary>
  ///   Is a type a list type, i.e. it's name starts with "List`1"
  ///   or if it is a defined type which implements a IList`1 interface.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <returns>true if a type is a list type</returns>
  public static bool IsList(this Type aType)
  {
    return aType.Name.StartsWith("List`1") || aType.GetInterfaces().FirstOrDefault(item => item.Name.StartsWith("IList`1")) != null;
  }

  /// <summary>
  ///   Checks if a type a list type, i.e. it's name starts with "List`1"
  ///   or if it is a defined type which implements a IList`1 interface.
  ///   If so it returns the item type of the list.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <param name="itemType">returned item type if a type is a list</param>
  /// <returns>true if a type is a list type</returns>
#if NET6_0_OR_GREATER
  public static bool IsList(this Type aType, [NotNullWhen(true)][MaybeNullWhen(false)] out Type? itemType)
#else
  public static bool IsList(this Type aType, out Type? itemType)
#endif
  {
    if (aType.Name.StartsWith("List`1"))
    {
      itemType = aType.GenericTypeArguments[0];
      return true;
    }
    var intf = aType.GetInterfaces().FirstOrDefault(item => item.Name.StartsWith("IList`1"));
    if (intf != null)
    {
      itemType = intf.GenericTypeArguments[0];
      return true;
    }
    itemType = null;
    return false;
  }

  /// <summary>
  ///   Is a type an list type, i.e. it's name starts with "List`1"
  ///   or if it is a defined type which implements a IList`1 interface.
  ///   If so it check the item type of the collection.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <param name="itemType">checked item type if a type is a list</param>
  /// <returns>true if a type is an enumerable type of item Type</returns>
  public static bool IsList(this Type aType, Type itemType)
  {
    return (IsList(aType, out var foundItemType)
            && foundItemType == itemType) || (foundItemType != null && itemType.IsSubclassOf(foundItemType));
  }

  #endregion

  #region IsCollection

  /// <summary>
  ///   Is a type a collection type, i.e. it's name starts with "Collection`1"
  ///   or if it is a defined type which implements a ICollection`1 interface.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <returns>true if a type is a collection type</returns>
  public static bool IsCollection(this Type aType)
  {
    return aType.Name.StartsWith("Collection`1") || aType.GetInterfaces().FirstOrDefault(item => item.Name.StartsWith("ICollection`1")) != null;
  }

  /// <summary>
  ///   Is a type a collection type, i.e. it's name starts with "Collection`1"
  ///   or if it is a defined type which implements a ICollection`1 interface.
  ///   If so it returns the item type of the collection.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <param name="itemType">returned item type if a type is a collection</param>
  /// <returns>true if a type is a collection type</returns>
#if NET6_0_OR_GREATER
  public static bool IsCollection(this Type aType, [NotNullWhen(true)][MaybeNullWhen(false)] out Type? itemType)
#else
  public static bool IsCollection(this Type aType, out Type? itemType)
#endif
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
  ///   Is a type a collection type, i.e. it's name starts with "Collection`1"
  ///   or if it is a defined type which implements a ICollection`1 interface.
  ///   If so it checks the item type of the collection.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <param name="itemType">checked item type if a type is a collection</param>
  /// <returns>true if a type is a collection type of item type</returns>
  public static bool IsCollection(this Type aType, Type itemType)
  {
    return IsCollection(aType, out var foundItemType)
           && (foundItemType == itemType || (foundItemType != null && itemType.IsSubclassOf(foundItemType)));
  }

  #endregion

  #region IsEnumerable

  /// <summary>
  ///   Is a type an enumerable type, i.e. it's name starts with "Enumerable`1"
  ///   or if it is a defined type which implements a IEnumerable`1 interface.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <returns>true if a type is an enumerable type</returns>
  public static bool IsEnumerable(this Type aType)
  {
    return aType.Name.StartsWith("Enumerable`1") || aType.GetInterfaces().FirstOrDefault(item => item.Name.StartsWith("IEnumerable`1")) != null;
  }

  /// <summary>
  ///   Is a type an enumerable type, i.e. it's name starts with "Enumerable`1"
  ///   or if it is a defined type which implements a IEnumerable`1 interface.
  ///   If so it returns the item type of the collection.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <param name="itemType">returned item type if a type is an enumerable</param>
  /// <returns>true if a type is an enumerable type</returns>
#if NET6_0_OR_GREATER
  public static bool IsEnumerable(this Type aType, [NotNullWhen(true)][MaybeNullWhen(false)] out Type? itemType)
#else
  public static bool IsEnumerable(this Type aType, out Type? itemType)
#endif
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
  ///   Is a type an enumerable type, i.e. it's name starts with "Enumerable`1"
  ///   or if it is a defined type which implements a IEnumerable`1 interface.
  ///   If so it check the item type of the collection.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <param name="itemType">checked item type if a type is an enumerable</param>
  /// <returns>true if a type is an enumerable type of item Type</returns>
  public static bool IsEnumerable(this Type aType, Type itemType)
  {
    return (IsEnumerable(aType, out var foundItemType)
            && foundItemType == itemType) || (foundItemType != null && itemType.IsSubclassOf(foundItemType));
  }

  #endregion

  #region IsDictionary

  /// <summary>
  ///   Is a type a dictionary type, i.e. it's name starts with "Dictionary`2"
  ///   or if it is a defined type which implements a IDictionary`2 interface.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <returns>true if a type is a dictionary type</returns>
  public static bool IsDictionary(this Type aType)
  {
    return aType.Name.StartsWith("Dictionary`2") || aType.GetInterfaces().FirstOrDefault(item => item.Name.StartsWith("IDictionary`2")) != null;
  }

  /// <summary>
  ///   Is a type a dictionary type, i.e. it's name starts with "Dictionary`2"
  ///   or if it is a defined type which implements a IDictionary`2 interface.
  ///   If so it returns the key type and the value type of the dictionary.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <param name="keyType">returned key type if a type is a dictionary</param>
  /// <param name="valueType">returned value type if a type is a dictionary</param>
  /// <returns>true if a type is a dictionary type</returns>
#if NET6_0_OR_GREATER
  public static bool IsDictionary(this Type aType, [NotNullWhen(true)][MaybeNullWhen(false)] out Type? keyType, 
    [NotNullWhen(true)][MaybeNullWhen(false)] out Type? valueType)
#else
  public static bool IsDictionary(this Type aType, out Type? keyType, out Type? valueType)
#endif
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

  /// <summary>
  ///   Is a type a dictionary type, i.e. it's name starts with "Dictionary`2"
  ///   or if it is a defined type which implements a IDictionary`2 interface.
  ///   If so it checks the key type and the value type of the dictionary.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <param name="keyType">checked key type if a type is a dictionary</param>
  /// <param name="valueType">checked value type if a type is a dictionary</param>
  /// <returns>true if a type is a dictionary type of key and value type</returns>
  public static bool IsDictionary(this Type aType, Type keyType, Type valueType)
  {
    return (IsDictionary(aType, out var foundKeyType, out var foundValueType)
            && foundKeyType == keyType) || (foundKeyType != null && keyType.IsSubclassOf(foundKeyType)
                                                                 && foundValueType == valueType) || (foundValueType != null &&
      valueType.IsSubclassOf(foundValueType));
  }

  #endregion IsDictionary

  #region IeKeyValuePair

  /// <summary>
  ///   Is a type a key value pair type, i.e. it's name starts with "KeyValuePair`2"
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <returns>true if a type is a key value pair type</returns>
  public static bool IsKeyValuePair(this Type aType)
  {
    return aType.Name.StartsWith("KeyValuePair`2") /* || aType.GetInterfaces().FirstOrDefault(item => item.Name.StartsWith("IDictionary`2")) != null*/
      ;
  }

  /// <summary>
  ///   Is a type a key value pair  type, i.e. it's name starts with  "KeyValuePair`2"
  ///   If so it returns the key type and the value type of the pair.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <param name="keyType">returned key type if a type is a key value pair</param>
  /// <param name="valueType">returned value type if a type is a key value pair</param>
  /// <returns>true if a type is a key value pair type</returns>
#if NET6_0_OR_GREATER
  public static bool IsKeyValuePair(this Type aType, [NotNullWhen(true)][MaybeNullWhen(false)] out Type? keyType, 
    [NotNullWhen(true)][MaybeNullWhen(false)] out Type? valueType)
#else
  public static bool IsKeyValuePair(this Type aType, out Type? keyType, out Type? valueType)
#endif
  {
    if (aType.Name.StartsWith("KeyValuePair`2"))
    {
      keyType = aType.GenericTypeArguments[0];
      valueType = aType.GenericTypeArguments[1];
      return true;
    }
    //var intf = aType.GetInterfaces().FirstOrDefault(item => item.Name.StartsWith("IDictionary`2"));
    //if (intf != null)
    //{
    //  keyType = intf.GenericTypeArguments[0];
    //  valueType = intf.GenericTypeArguments[1];
    //  return true;
    //}
    keyType = null;
    valueType = null;
    return false;
  }

  /// <summary>
  ///   Is a type a key value pair  type, i.e. it's name starts with "KeyValuePair`2"
  ///   If so it checks the key type and the value type of the pair.
  /// </summary>
  /// <param name="aType">checked type</param>
  /// <param name="keyType">checked item type if a type is a key value pair </param>
  /// <param name="valueType">checked item type if a type is a key value pair </param>
  /// <returns>true if a type is a key value pair type  of key and value type</returns>
  public static bool IsKeyValuePair(this Type aType, Type keyType, Type valueType)
  {
    return (IsKeyValuePair(aType, out var foundKeyType, out var foundValueType)
            && foundKeyType == keyType) || (foundKeyType != null && keyType.IsSubclassOf(foundKeyType)
                                                                 && foundValueType == valueType) || (foundValueType != null &&
      valueType.IsSubclassOf(foundValueType));
  }

  #endregion IsDictionary

  #region numeric type classification

  /// <summary>
  ///   Mapping from a numeric type to possible numeric supertype
  /// </summary>
  public static Type[,] NumericSupertypes =
  {
    { typeof(SByte), typeof(Byte) },
    { typeof(SByte), typeof(Int16) },
    { typeof(SByte), typeof(Int32) },
    { typeof(SByte), typeof(Int64) },
    { typeof(SByte), typeof(UInt16) },
    { typeof(SByte), typeof(UInt32) },
    { typeof(SByte), typeof(UInt64) },
    { typeof(SByte), typeof(Single) },
    { typeof(SByte), typeof(Double) },
    { typeof(SByte), typeof(Decimal) },

    { typeof(Byte), typeof(SByte) },
    { typeof(Byte), typeof(Int16) },
    { typeof(Byte), typeof(Int32) },
    { typeof(Byte), typeof(Int64) },
    { typeof(Byte), typeof(UInt16) },
    { typeof(Byte), typeof(UInt32) },
    { typeof(Byte), typeof(UInt64) },
    { typeof(Byte), typeof(Single) },
    { typeof(Byte), typeof(Double) },
    { typeof(Byte), typeof(Decimal) },

    { typeof(Int16), typeof(UInt16) },
    { typeof(Int16), typeof(Int32) },
    { typeof(Int16), typeof(Int64) },
    { typeof(Int16), typeof(UInt32) },
    { typeof(Int16), typeof(UInt64) },
    { typeof(Int16), typeof(Single) },
    { typeof(Int16), typeof(Double) },
    { typeof(Int16), typeof(Decimal) },

    { typeof(UInt16), typeof(Int16) },
    { typeof(UInt16), typeof(Int32) },
    { typeof(UInt16), typeof(Int64) },
    { typeof(UInt16), typeof(UInt32) },
    { typeof(UInt16), typeof(UInt64) },
    { typeof(UInt32), typeof(Single) },
    { typeof(UInt32), typeof(Double) },
    { typeof(UInt32), typeof(Decimal) },

    { typeof(Int32), typeof(UInt32) },
    { typeof(Int32), typeof(Int64) },
    { typeof(Int32), typeof(UInt64) },
    { typeof(Int32), typeof(Single) },
    { typeof(Int32), typeof(Double) },
    { typeof(Int32), typeof(Decimal) },

    { typeof(UInt32), typeof(Int32) },
    { typeof(UInt32), typeof(Int64) },
    { typeof(UInt32), typeof(UInt64) },
    { typeof(UInt32), typeof(Single) },
    { typeof(UInt32), typeof(Double) },
    { typeof(UInt32), typeof(Decimal) },

    { typeof(Single), typeof(Double) },
    { typeof(Single), typeof(Decimal) },

    { typeof(Double), typeof(Decimal) }
  };

  /// <summary>
  ///   Check if this type can be a supertype of other type
  /// </summary>
  /// <param name="thisType"></param>
  /// <param name="otherType"></param>
  /// <returns></returns>
  public static bool IsNumericSupertypeOf(this Type thisType, Type otherType)
  {
    for (var i = 0; i < NumericSupertypes.GetLength(0); i++)
      if (NumericSupertypes[i, 0] == otherType && NumericSupertypes[i, 1] == thisType)
        return true;
    return false;
  }

  #endregion
}
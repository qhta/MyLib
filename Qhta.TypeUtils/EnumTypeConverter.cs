using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Qhta.TypeUtils;

/// <summary>
///   A class that helps to convert enum type values to/from string. It accepts shortcuts of enum value names.
/// </summary>
public static class EnumTypeConverter
{
  private static readonly ConcurrentDictionary<Type, Dicts> convDict = new();

  private static void CreateDicts(Type type)
  {
    lock (convDict)
    {
      if (convDict.ContainsKey(type))
        return;
      var uType = Enum.GetUnderlyingType(type);
      var dicts = new Dicts();
      dicts = convDict.GetOrAdd(type, dicts);
      lock (dicts)
      {
        foreach (var enumMember in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
          UInt64 value = 0;
          var obj = enumMember.GetValue(null);
          if (obj != null)
          {
            if (uType == typeof(byte))
              value = (byte)obj;
            else if (uType == typeof(sbyte))
              value = (UInt64)(sbyte)obj;
            else if (uType == typeof(short))
              value = (UInt64)(short)obj;
            else if (uType == typeof(ushort))
              value = (ushort)obj;
            else if (uType == typeof(uint))
              value = (uint)obj;
            else if (uType == typeof(Int64))
              value = (UInt64)(Int64)obj;
            else if (uType == typeof(UInt64))
              value = (UInt64)obj;
            else
              value = (UInt64)(int)obj;
            var enumMemberAttribute = enumMember.GetCustomAttributes(typeof(EnumMemberAttribute), false).FirstOrDefault() as EnumMemberAttribute;
            if (enumMemberAttribute != null && enumMemberAttribute.Value != null)
            {
              dicts.ValueToString.TryAdd(value, enumMemberAttribute.Value);
              var s = enumMemberAttribute.Value.ToLower();
              dicts.StringToValue.TryAdd(s, value);
              if (s != enumMember.Name.ToLower())
                dicts.StringToValue.TryAdd(enumMember.Name.ToLower(), value);
            }
            else
            {
              dicts.ValueToString.TryAdd(value, enumMember.Name);
              dicts.StringToValue.TryAdd(enumMember.Name.ToLower(), value);
            }
          }
        }
        var strings = dicts.StringToValue.ToList();
        var sortedStrings = new SortedSet<string>(strings.Select(item => item.Key).ToList());
        foreach (var kvp in strings)
        {
          var str = kvp.Key;
          //if (str.StartsWith("colloq"))
          //  Debug.Assert(true);
          var s = str;
          while (s.Length > 0)
          {
            if (!sortedStrings.Contains(s))
            {
              //if ( && strings.Select(item => item.Key).Where(item => item.StartsWith(s)).Count()<=1))
              sortedStrings.Add(s);
              dicts.StringToValue.TryAdd(s, kvp.Value);
            }
            s = s.Substring(0, s.Length - 1);
          }
          s = str.RemoveAll(1, new[] { 'a', 'e', 'i', 'o', 'u', 'y' });
          while (s.Length > 0)
          {
            if (!sortedStrings.Contains(s) && strings.Select(item => item.Key).Where(item => item.StartsWith(s)).Count() <= 1)
            {
              sortedStrings.Add(s);
              dicts.StringToValue.TryAdd(s, kvp.Value);
            }
            s = s.Substring(0, s.Length - 1);
          }
        }
      }
    }
  }

  /// <summary>
  ///   Remove specific chars from string
  /// </summary>
  /// <param name="str"></param>
  /// <param name="from"></param>
  /// <param name="charsToRemove"></param>
  /// <returns></returns>
  public static string RemoveAll(this string str, int from, char[] charsToRemove)
  {
    var chars = str.ToCharArray().ToList();
    for (var i = from; i < chars.Count; i++)
      if (charsToRemove.Contains(chars[i]))
        chars.RemoveAt(i--);
    return new string(chars.ToArray());
  }

  /// <summary>
  ///   Translating enum value to string
  /// </summary>
  /// <typeparam name="EType"></typeparam>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string Encode<EType>(EType value) where EType : struct, IConvertible
  {
    if (!convDict.ContainsKey(typeof(EType)))
      CreateDicts(typeof(EType));
    if (typeof(EType).GetCustomAttribute<FlagsAttribute>() != null)
    {
      UInt64 mask = 1;
      var uVal = value.ToUInt64(null);
      var ss = new List<string>();
      for (var i = 0; i < 32; i++)
      {
        if ((uVal & mask) != 0) ss.Add(convDict[typeof(EType)].ValueToString[mask]);
        mask <<= 1;
      }
      return string.Join(";", ss);
    }
    return convDict[typeof(EType)].ValueToString[value.ToUInt64(null)];
  }

  /// <summary>
  ///   Translating string value to enum value
  /// </summary>
  /// <typeparam name="EType"></typeparam>
  /// <param name="value"></param>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  [DebuggerStepThrough]
  public static EType? Decode<EType>(string value) where EType : struct, IConvertible
  {
    EType? result;
    if (!TryDecode(value, out result, out var invKey))
      throw new KeyNotFoundException($"Key \"{invKey}\" not found in {typeof(EType).Name} type");
    return result;
  }

  /// <summary>
  ///   Trying translating string value to enum value
  /// </summary>
  /// <typeparam name="EType"></typeparam>
  /// <param name="value"></param>
  /// <param name="result"></param>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  [DebuggerStepThrough]
  public static bool TryDecode<EType>(string value, out EType? result) where EType : struct, IConvertible
  {
    return TryDecode(value, out result, out var invKey);
  }

  /// <summary>
  ///   Trying translating string value to enum value
  /// </summary>
  /// <typeparam name="EType"></typeparam>
  /// <param name="value"></param>
  /// <param name="result"></param>
  /// <param name="invalidKey"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public static bool TryDecode<EType>(string value, out EType? result, out string? invalidKey) where EType : struct, IConvertible
  {
    result = default(EType);
    invalidKey = null;
    if (!convDict.ContainsKey(typeof(EType)))
      CreateDicts(typeof(EType));
    lock (convDict[typeof(EType)])
    {
      var ss = value.Split(';', ',', '|', '+');
      UInt64 n = 0;
      foreach (var s in ss)
      {
        if (!convDict[typeof(EType)].StringToValue.TryGetValue(s.ToLower().Trim(), out var v))
        {
          invalidKey = s;
          return false;
        }
        n |= v;
      }
      result = (EType)Enum.ToObject(typeof(EType), n);
      return true;
    }
  }

  private class Dicts
  {
    public readonly ConcurrentDictionary<string, UInt64> StringToValue;
    public readonly ConcurrentDictionary<UInt64, string> ValueToString;

    public Dicts()
    {
      ValueToString = new ConcurrentDictionary<UInt64, string>();
      StringToValue = new ConcurrentDictionary<string, UInt64>();
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;

namespace MyLib.TypeUtils
{
  public static class EnumTypeConverter
  {
    class Dicts {
      public Dictionary<UInt64, string> ValueToString;
      public Dictionary<string, UInt64> StringToValue;

      public Dicts()
      {
        ValueToString = new Dictionary<UInt64, string>();
        StringToValue = new Dictionary<string, UInt64>();
      }
    }

    static ConcurrentDictionary<Type, Dicts> convDict = new ConcurrentDictionary<Type, Dicts>();

    static void CreateDicts(Type type)
    {
      lock (convDict)
      {
        if (convDict.ContainsKey(type))
          return;
        var uType = Enum.GetUnderlyingType(type);
        var Dicts = new Dicts();
        convDict.TryAdd(type, Dicts);
        foreach (var enumMember in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {

          UInt64 value = 0;
          var obj = enumMember.GetValue(null);
          if (uType==typeof(byte))
            value = (byte)obj;
          else if (uType==typeof(sbyte))
            value = (UInt64)(sbyte)obj;
          else if (uType==typeof(short))
            value = (UInt64)(short)obj;
          else if (uType==typeof(ushort))
            value = (ushort)obj;
          else if (uType==typeof(uint))
            value = (UInt64)(uint)obj;
          else if (uType==typeof(Int64))
            value = (UInt64)(Int64)obj;
          else if (uType==typeof(UInt64))
            value = (UInt64)obj;
          else
            value = (UInt64)(int)obj;
          //if (enumMember.Name=="Interjection")
          //  Debug.Assert(true);
          var enumMemberAttribute = enumMember.GetCustomAttributes(typeof(EnumMemberAttribute), false).FirstOrDefault() as EnumMemberAttribute;
          if (enumMemberAttribute!=null)
          {
            Dicts.ValueToString.Add(value, enumMemberAttribute.Value);
            string s = enumMemberAttribute.Value.ToLower();
            Dicts.StringToValue.Add(s, value);
            if (s!=enumMember.Name.ToLower())
              Dicts.StringToValue.Add(enumMember.Name.ToLower(), value);
          }
          else
          {
            Dicts.ValueToString.Add(value, enumMember.Name);
            Dicts.StringToValue.Add(enumMember.Name.ToLower(), value);
          }
        }
        var strings = Dicts.StringToValue.ToList();
        var sortedStrings = new SortedSet<string>(strings.Select(item => item.Key).ToList());
        foreach (var kvp in strings)
        {
          string str = kvp.Key;
          //if (str.StartsWith("colloq"))
          //  Debug.Assert(true);
          string s = str;
          while (s.Length>0)
          {
            if (!sortedStrings.Contains(s))
            {
              //if ( && strings.Select(item => item.Key).Where(item => item.StartsWith(s)).Count()<=1))
              sortedStrings.Add(s);
              Dicts.StringToValue.Add(s, kvp.Value);
            }
            s = s.Substring(0, s.Length-1);
          }
          s = str.RemoveAll(1, new char[] { 'a', 'e', 'i', 'o', 'u', 'y' });
          while (s.Length>0)
          {
            if (!sortedStrings.Contains(s) && strings.Select(item => item.Key).Where(item => item.StartsWith(s)).Count()<=1)
            {
              sortedStrings.Add(s);
              Dicts.StringToValue.Add(s, kvp.Value);
            }
            s = s.Substring(0, s.Length-1);
          }
        }
      }
    }

    public static string RemoveAll(this string str, int from, char[] charsToRemove)
    {
      var chars = str.ToCharArray().ToList();
      for (int i = from; i<chars.Count; i++)
        if (charsToRemove.Contains(chars[i]))
          chars.RemoveAt(i--);
      return new string(chars.ToArray());
    }

    public static string Encode<EType>(EType value) where EType: struct, IConvertible
    {
      if (!convDict.ContainsKey(typeof(EType)))
        CreateDicts(typeof(EType));
      if (typeof(EType).GetCustomAttribute<FlagsAttribute>()!=null)
      {
        UInt64 mask = 1;
        UInt64 uVal = ((IConvertible)value).ToUInt64(null);
        List<string> ss = new List<string>();
        for (int i=0; i<32; i++)
        {
          if ((uVal & mask)!=0)
          {
            ss.Add(convDict[typeof(EType)].ValueToString[mask]);
          }
          mask <<= 1;
        }
        return string.Join(";", ss);
      }
      else
        return convDict[typeof(EType)].ValueToString[((IConvertible)value).ToUInt64(null)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public static EType Decode<EType>(string value) where EType : struct, IConvertible
    {
      EType result;
      if (!TryDecode(value, out result, out string invKey))
        throw new KeyNotFoundException($"Key \"{invKey}\" not found in {typeof(EType).Name} type");
      return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public static bool TryDecode<EType>(string value, out EType result) where EType : struct, IConvertible
    {
      return TryDecode<EType>(value, out result, out string invKey);
    }

    [DebuggerStepThrough]
    public static bool TryDecode<EType>(string value, out EType result, out string invalidKey) where EType : struct, IConvertible
    {
      result = default(EType);
      invalidKey = null;
      if (!convDict.ContainsKey(typeof(EType)))
        CreateDicts(typeof(EType));
      string[] ss = value.Split(new char[] { ';', ',', '|', '+' });
      UInt64 n = 0;
      foreach (var s in ss)
      {
        if (!convDict[typeof(EType)].StringToValue.TryGetValue(s.ToLower().Trim(), out UInt64 v))
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
}

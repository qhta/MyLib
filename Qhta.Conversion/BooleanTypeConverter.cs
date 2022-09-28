using System.Buffers.Text;
using System.ComponentModel;
using System.Globalization;

namespace Qhta.Conversion;


public class BooleanTypeConverter : TypeConverter
{
  public (string, string)[] BooleanStrings { get; set; }
    = new (string, string)[] { ("true", "false"), ("1", "0"), ("on", "off") };

  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string)
           || destinationType == typeof(int)
           || destinationType == typeof(byte)
           || destinationType == typeof(sbyte)
           || destinationType == typeof(uint)
           || destinationType == typeof(short)
           || destinationType == typeof(ushort)
           || destinationType == typeof(long)
           || destinationType == typeof(ulong);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value is not null && value is not bool)
      value = IntToBool(value);
    if (value is bool bv)
    {
      if (destinationType == typeof(string))
      {
        var result = bv ? BooleanStrings[0].Item1 : BooleanStrings[0].Item2;
        return result;
      }
      if (destinationType == typeof(bool))
        return bv;
      if (destinationType == typeof(int))
        return bv ? (int)1 : (int)0;
      if (destinationType == typeof(byte))
        return bv ? (byte)1 : (byte)0;
      if (destinationType == typeof(sbyte))
        return bv ? (sbyte)1 : (sbyte)0;
      if (destinationType == typeof(uint))
        return bv ? (uint)1 : (uint)0;
      if (destinationType == typeof(short))
        return bv ? (short)1 : (short)0;
      if (destinationType == typeof(ushort))
        return bv ? (ushort)1 : (ushort)0;
      if (destinationType == typeof(short))
        return bv ? (long)1 : (long)0;
      if (destinationType == typeof(ulong))
        return bv ? (ulong)1 : (ulong)0;
    }
    return base.ConvertTo(context, culture, value, destinationType);
  }

  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string)
           || sourceType == typeof(int)
           || sourceType == typeof(byte)
           || sourceType == typeof(sbyte)
           || sourceType == typeof(uint)
           || sourceType == typeof(short)
           || sourceType == typeof(ushort)
           || sourceType == typeof(long)
           || sourceType == typeof(ulong);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (value is string str)
    {
      str = str.ToLowerInvariant();
      foreach (var bs in BooleanStrings)
      {
        if (str.Equals(bs.Item1, StringComparison.InvariantCultureIgnoreCase))
          return true;
        if (str.Equals(bs.Item2, StringComparison.InvariantCultureIgnoreCase))
          return false;
      }
    }
    return IntToBool(value);
  }

  public static bool? IntToBool(object value)
  {
    if (value is int i32)
      return i32 != 0;
    if (value is byte u8)
      return u8 != 0;
    if (value is sbyte i8)
      return i8 != 0;
    if (value is uint u32)
      return u32 != 0;
    if (value is short i16)
      return i16 != 0;
    if (value is ushort u16)
      return u16 != 0;
    if (value is long i64)
      return i64 != 0;
    if (value is ulong u64)
      return u64 != 0;
    return null;
  }

  public static int? BoolToInt(bool value)
  {
    return value ? 1 : 0;
  }
}
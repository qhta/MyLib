using System.Buffers.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Qhta.Conversion;

public class NumericTypeConverter : TypeConverter
{
  public Type? ValueType { get; set; }

  public string? Format
  {
    get => _Format; 
    set
    {
      if (_Format != value)
      {
        _Format = value;
        if (_Format!=null && (_Format.Contains('X', StringComparison.InvariantCultureIgnoreCase)))
          NumberStyle = NumberStyles.HexNumber;
      }
    }
}

  private string? _Format;

  public NumberStyles NumberStyle { get; set; } = NumberStyles.None;

  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string)
           || destinationType == typeof(int)
           || destinationType == typeof(byte)
           || destinationType == typeof(uint)
           || destinationType == typeof(sbyte)
           || destinationType == typeof(short)
           || destinationType == typeof(ushort)
           || destinationType == typeof(long)
           || destinationType == typeof(ulong)
           || destinationType == typeof(float)
           || destinationType == typeof(double)
           || destinationType == typeof(decimal)
           || destinationType == typeof(bool);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value!=null)
    {
      if (destinationType == typeof(string))
      {
        if (ValueType!=null)
          value = Convert.ChangeType(value, ValueType, culture);
        Type valueType = value.GetType();
        if (valueType == typeof(int))
          return ((int)value).ToString(Format);
        if (valueType == typeof(byte))
          return ((byte)value).ToString(Format);
        if (valueType == typeof(uint))
          return ((uint)value).ToString(Format);
        if (valueType == typeof(sbyte))
          return ((sbyte)value).ToString(Format);
        if (valueType == typeof(short))
          return ((short)value).ToString(Format);
        if (valueType == typeof(ushort))
          return ((ushort)value).ToString(Format);
        if (valueType == typeof(long))
          return ((ulong)value).ToString(Format);
        if (valueType == typeof(ulong))
          return ((ulong)value).ToString(Format);
        if (valueType == typeof(float))
          return ((float)value).ToString(Format, culture);
        if (valueType == typeof(double))
          return ((double)value).ToString(Format, culture);
        if (valueType == typeof(bool))
          return ((bool)value) ? "1" : "0";
      }
      return Convert.ChangeType(value, destinationType, culture);
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
           || sourceType == typeof(ulong)
           || sourceType == typeof(float)
           || sourceType == typeof(double)
           || sourceType == typeof(decimal)
           || sourceType == typeof(bool);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (value is string str)
    {
      if (ValueType == null || ValueType == typeof(string))
        return str;
      if (ValueType == typeof(int))
        return int.Parse(str, NumberStyle);
      if (ValueType == typeof(byte))
        return byte.Parse(str, NumberStyle);
      if (ValueType == typeof(uint))
        return uint.Parse(str, NumberStyle);
      if (ValueType == typeof(sbyte))
        return sbyte.Parse(str, NumberStyle);
      if (ValueType == typeof(short))
        return short.Parse(str, NumberStyle);
      if (ValueType == typeof(ushort))
        return ushort.Parse(str, NumberStyle);
      if (ValueType == typeof(long))
        return long.Parse(str, NumberStyle);
      if (ValueType == typeof(ulong))
        return ulong.Parse(str, NumberStyle);
      if (ValueType == typeof(float))
        return float.Parse(str, culture);
      if (ValueType == typeof(double))
        return double.Parse(str, culture);
      if (ValueType == typeof(decimal))
        return decimal.Parse(str, culture);
      if (ValueType == typeof(bool))
        return (str != "0");
      return base.ConvertFrom(context, culture, value);
    }
    if (value is bool bv)
      return bv ? 1 : 0;
    if (ValueType!=null)
      return Convert.ChangeType(value, ValueType, culture);
    return base.ConvertFrom(context, culture, value);
  }
}
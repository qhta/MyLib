using System.Buffers.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Qhta.Conversion;

public class NumericTypeConverter : TypeConverter
{
  public Type? ExpectedType { get; set; }

  public string? Format
  {
    get => _Format;
    set
    {
      if (_Format != value)
      {
        _Format = value;
        if (_Format != null && (_Format.Contains('X', StringComparison.InvariantCultureIgnoreCase)))
          NumberStyle = NumberStyles.HexNumber;
      }
    }
  }

  private string? _Format;

  public NumberStyles NumberStyle { get; set; } = NumberStyles.None;

  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null;
    if (destinationType == typeof(string))
    {
      if (culture == null)
        culture = CultureInfo.InvariantCulture;
      Type valueType = value.GetType();
      if (valueType == typeof(int))
        return ((int)value).ToString(Format, culture);
      if (valueType == typeof(byte))
        return ((byte)value).ToString(Format, culture);
      if (valueType == typeof(uint))
        return ((uint)value).ToString(Format, culture);
      if (valueType == typeof(sbyte))
        return ((sbyte)value).ToString(Format, culture);
      if (valueType == typeof(short))
        return ((short)value).ToString(Format, culture);
      if (valueType == typeof(ushort))
        return ((ushort)value).ToString(Format, culture);
      if (valueType == typeof(long))
        return ((long)value).ToString(Format, culture);
      if (valueType == typeof(ulong))
        return ((ulong)value).ToString(Format, culture);
      if (valueType == typeof(float))
        return ((float)value).ToString(Format, culture);
      if (valueType == typeof(double))
        return ((double)value).ToString(Format, culture);
      if (valueType == typeof(decimal))
        return ((decimal)value).ToString(Format, culture);
    }
    return Convert.ChangeType(value, destinationType, culture);
  }

  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type? sourceType)
  {
    return sourceType == typeof(string);
  }
  public new object? ConvertFrom(object value) => ConvertFrom(null, CultureInfo.InvariantCulture, value);

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    if (value is null)
      return null;
    if (value is string str)
    {
      if (str == String.Empty)
        return null;
      if (TryParseAnyNumber(str, NumberStyle, culture, out var number))
      {
        if (ExpectedType != null)
          return Convert.ChangeType(number, ExpectedType, culture);
        return number;
      }
      return base.ConvertFrom(context, culture, value);
    }
    return base.ConvertFrom(context, culture, value);
  }

  public bool TryParseAnyNumber(string? str, NumberStyles numberStyle, CultureInfo? culture, out object? value)
  {
    value = null;
    if (string.IsNullOrEmpty(str))
    {
      return true;
    }
    if (culture == null)
      culture = CultureInfo.InvariantCulture;
    if (str.Contains("E+", StringComparison.Ordinal) || str.Contains("E-", StringComparison.Ordinal))
    {
      if (numberStyle == NumberStyles.None)
        numberStyle = NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
      if (double.TryParse(str, numberStyle, culture, out var dbl))
      {
        value = dbl;
        return true;
      }
      return false;
    }
    if (str.Contains(culture.NumberFormat.NumberDecimalSeparator))
    {
      if (numberStyle == NumberStyles.None)
        numberStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
      if (decimal.TryParse(str, numberStyle, culture, out var dm))
      {
        value = dm;
        return true;
      }
      return false;
    }
    if (str[0] == '-')
    {
      if (numberStyle == NumberStyles.None)
        numberStyle= NumberStyles.AllowLeadingSign;
      if (Int64.TryParse(str, out var i64))
      {
        if (i64 >= Int32.MinValue)
          value = (Int32)i64;
        else
          value = i64;
        return true;
      }
      if (decimal.TryParse(str, out var dm))
      {
        value = dm;
        return true;
      }
      if (str.StartsWith("-INF", StringComparison.InvariantCultureIgnoreCase))
      {
        value = double.NegativeInfinity;
        return true;
      }
      return false;
    }
    else
    {
      if (numberStyle.HasFlag(NumberStyles.HexNumber))
      {
        if (UInt64.TryParse(str, numberStyle, culture, out var uhex))
        {
          if (uhex <= Int32.MaxValue)
            value = (Int32)uhex;
          else
            value = uhex;
          return true;
        }
        return false;
      }
      if (UInt64.TryParse(str, out var u64))
      {
        if (u64 <= Int32.MaxValue)
          value = (Int32)u64;
        else
          value = u64;
        return true;
      }
      if (decimal.TryParse(str, out var dm))
      {
        value = dm;
        return true;
      }
    }
    if (String.Equals(str, "NaN", StringComparison.InvariantCultureIgnoreCase))
    {
      value = double.NaN;
      return true;
    }
    if (str.StartsWith("INF", StringComparison.InvariantCultureIgnoreCase))
    {
      value = double.PositiveInfinity;
      return true;
    }
    return false;
  }

}
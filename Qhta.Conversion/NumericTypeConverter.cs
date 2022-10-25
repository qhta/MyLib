using System.Buffers.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Qhta.Conversion;

public class NumericTypeConverter : TypeConverter, ITypeConverter, INumberRestrictions
{
  public Type? ExpectedType { get; set; }

  public XsdSimpleType? XsdType
  {
    get => _XsdType;
    set
    {
      if (ExpectedType == null && value != null)
      {
        switch (value)
        {
          case XsdSimpleType.Byte:
            ExpectedType = typeof(SByte);
            break;
          case XsdSimpleType.UnsignedByte:
            ExpectedType = typeof(Byte);
            break;
          case XsdSimpleType.Short:
            ExpectedType = typeof(Int16);
            break;
          case XsdSimpleType.UnsignedShort:
            ExpectedType = typeof(UInt16);
            break;
          case XsdSimpleType.Int:
            ExpectedType = typeof(Int32);
            break;
          case XsdSimpleType.UnsignedInt:
            ExpectedType = typeof(UInt32);
            break;
          case XsdSimpleType.Long:
            ExpectedType = typeof(Int64);
            break;
          case XsdSimpleType.UnsignedLong:
            ExpectedType = typeof(UInt64);
            break;
          case XsdSimpleType.Decimal:
            ExpectedType = typeof(Decimal);
            break;
          case XsdSimpleType.Float:
            ExpectedType = typeof(Single);
            break;
          case XsdSimpleType.Double:
            ExpectedType = typeof(Double);
            break;
          case XsdSimpleType.Integer:
          case XsdSimpleType.PositiveInteger:
          case XsdSimpleType.NegativeInteger:
          case XsdSimpleType.NonNegativeInteger:
          case XsdSimpleType.NonPositiveInteger:
            ExpectedType = typeof(String);
            break;
          default:
            return;
        }
        _XsdType = value;
      }
    }
  }
  protected XsdSimpleType? _XsdType;

  public string? Format
  {
    get => _Format;
    set
    {
      if (_Format != value)
      {
        _Format = value;
        if (_Format != null && (_Format.Contains('X', StringComparison.InvariantCultureIgnoreCase)))
          NumberStyle |= NumberStyles.HexNumber;
      }
    }
  }

  public CultureInfo? Culture { get; set; }

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
    var format = Format;
    var valueType = value.GetType();
    if (TotalDigits != null || FractionDigits != null)
    {
      if (valueType == typeof(Decimal) || valueType == typeof(Single) || valueType == typeof(Double))
      {
        if (format == null)
          format = "F";
        if (FractionDigits != null)
          format += FractionDigits.ToString();
      }
      else
      {
        if (format == null)
          format = "D";
        if (TotalDigits != null)
          format += TotalDigits.ToString();
      }
    }
    if (destinationType == typeof(string))
    {
      if (culture == null)
        culture = CultureInfo.InvariantCulture;
      if (valueType == typeof(int))
        return ((int)value).ToString(format, culture);
      if (valueType == typeof(byte))
        return ((byte)value).ToString(format, culture);
      if (valueType == typeof(uint))
        return ((uint)value).ToString(format, culture);
      if (valueType == typeof(sbyte))
        return ((sbyte)value).ToString(format, culture);
      if (valueType == typeof(short))
        return ((short)value).ToString(format, culture);
      if (valueType == typeof(ushort))
        return ((ushort)value).ToString(format, culture);
      if (valueType == typeof(long))
        return ((long)value).ToString(format, culture);
      if (valueType == typeof(ulong))
        return ((ulong)value).ToString(format, culture);
      if (valueType == typeof(float))
        return ((float)value).ToString(format, culture);
      if (valueType == typeof(double))
        return ((double)value).ToString(format, culture);
      if (valueType == typeof(decimal))
        return ((decimal)value).ToString(format, culture);
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
      if (ExpectedType==typeof(string))
        return str;
      if (TryParseAnyNumber(str, NumberStyle, culture, out var number))
      {
        if (number != null)
        {
          if (MaxExclusive != null && Convert.ToDouble(number) <= MaxExclusive)
            throw new InvalidDataException($"Converter value {number} is not greater than max exclusive value {MaxExclusive}");
          if (MinInclusive != null && Convert.ToDouble(number) < MinInclusive)
            throw new InvalidDataException($"Converter value {number} is less than min inclusive value {MinInclusive}");
          if (MaxInclusive != null && Convert.ToDouble(number) > MaxInclusive)
            throw new InvalidDataException($"Converter value {number} is greater than max inclusive value {MaxInclusive}");
          if (MinExclusive != null && Convert.ToDouble(number) >= MinExclusive)
            throw new InvalidDataException($"Converter value {number} is not less than max exclusive value {MinExclusive}");
        }
        if (ExpectedType != null)
          return Convert.ChangeType(number, ExpectedType, culture);
        return number;
      }
      throw new InvalidDataException($"NumericTypeConverter cannot convert from string \"{str}\"");
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
      numberStyle |= NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
      if (double.TryParse(str, numberStyle, culture, out var dbl))
      {
        value = dbl;
        return true;
      }
      return false;
    }
    if (str.Contains(culture.NumberFormat.NumberDecimalSeparator))
    {
      numberStyle |= NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
      if (decimal.TryParse(str, numberStyle, culture, out var dm))
      {
        value = dm;
        return true;
      }
      return false;
    }
    if (str.Contains(culture.NumberFormat.NegativeSign))
    {
      numberStyle |= NumberStyles.AllowLeadingSign;
      if (Int64.TryParse(str, numberStyle, culture, out var i64))
      {
        if (i64 >= Int32.MinValue)
          value = (Int32)i64;
        else
          value = i64;
        return true;
      }
      if (decimal.TryParse(str, numberStyle, culture, out var dm))
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
      if (UInt64.TryParse(str, numberStyle, culture, out var u64))
      {
        if (u64 <= Int32.MaxValue)
          value = (Int32)u64;
        else
          value = u64;
        return true;
      }
      if (decimal.TryParse(str, numberStyle, culture, out var dm))
      {
        value = dm;
        return true;
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

  public int? TotalDigits { get; set; }
  public int? FractionDigits { get; set; }
  public double? MinExclusive { get; set; }
  public double? MaxExclusive { get; set; }
  public double? MinInclusive { get; set; }
  public double? MaxInclusive { get; set; }
}
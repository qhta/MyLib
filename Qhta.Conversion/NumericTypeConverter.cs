namespace Qhta.Conversion;

/// <summary>
/// Universal converter for converting a numeric type into a string (forth and back). 
/// Supports types: Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Decimal, Single, Double.
/// Specifies additional NumberStyle property to support hexadecimal format.
/// Specifies min/max exclusive/inclusive values to validate the range.
/// </summary>
public class NumericTypeConverter : BaseTypeConverter, INumberRestrictions
{
  private string? _Format;
  private SimpleType? _SimpleType;

  /// <summary>
  /// Additional formatting property.
  /// </summary>
  public NumberStyles NumberStyle { get; set; } = NumberStyles.None;

  /// <summary>
  /// Specifies total number of digits
  /// </summary>
  public int? TotalDigits { get; set; }

  /// <summary>
  /// Specifies fractional number of digits
  /// </summary>
  public int? FractionDigits { get; set; }

  /// <summary>
  /// Specifies minimum (double) value outside an accepted range.
  /// </summary>
  public double? MinExclusive { get; set; }

  /// <summary>
  /// Specifies maximum (double) value outside an accepted range.
  /// </summary>
  public double? MaxExclusive { get; set; }

  /// <summary>
  /// Specifies minimum (double) value inside an accepted range.
  /// </summary>
  public double? MinInclusive { get; set; }

  /// <summary>
  /// Specifies maximum (double) value inside an accepted range.
  /// </summary>
  public double? MaxInclusive { get; set; }

  /// <summary>
  /// Overrides the base XsdType property such that setting a XsdType also sets ExpectedType.
  /// </summary>
  public override SimpleType? SimpleType
  {
    get => _SimpleType;
    set
    {
      if (ExpectedType == null && value != null)
      {
        switch (value)
        {
          case Xml.SimpleType.Byte:
            ExpectedType = typeof(SByte);
            break;
          case Xml.SimpleType.UnsignedByte:
            ExpectedType = typeof(Byte);
            break;
          case Xml.SimpleType.Short:
            ExpectedType = typeof(Int16);
            break;
          case Xml.SimpleType.UnsignedShort:
            ExpectedType = typeof(UInt16);
            break;
          case Xml.SimpleType.Int:
            ExpectedType = typeof(Int32);
            break;
          case Xml.SimpleType.UnsignedInt:
            ExpectedType = typeof(UInt32);
            break;
          case Xml.SimpleType.Long:
            ExpectedType = typeof(Int64);
            break;
          case Xml.SimpleType.UnsignedLong:
            ExpectedType = typeof(UInt64);
            break;
          case Xml.SimpleType.Decimal:
            ExpectedType = typeof(Decimal);
            break;
          case Xml.SimpleType.Float:
            ExpectedType = typeof(Single);
            break;
          case Xml.SimpleType.Double:
            ExpectedType = typeof(Double);
            break;
          case Xml.SimpleType.Integer:
          case Xml.SimpleType.PositiveInteger:
          case Xml.SimpleType.NegativeInteger:
          case Xml.SimpleType.NonNegativeInteger:
          case Xml.SimpleType.NonPositiveInteger:
            ExpectedType = typeof(String);
            break;
          default:
            return;
        }
        _SimpleType = value;
      }
    }
  }

  /// <summary>
  /// Overrides the base Format property such that setting a format with 'X' (or 'x') character
  /// implies specifying NumberStyle as HexNumber.
  /// </summary>
  public override string? Format
  {
    get => _Format;
    set
    {
      if (_Format != value)
      {
        _Format = value;
        if (_Format != null &&
#if NET6_0_OR_GREATER
          _Format.Contains('X', StringComparison.InvariantCultureIgnoreCase)
#else
            _Format.Contains('X') || _Format.Contains('x')
#endif
           )
          NumberStyle |= NumberStyles.HexNumber;
      }
    }
  }

  /// <inheritdoc/>
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  /// <inheritdoc/>
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

  /// <inheritdoc/>
  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type? sourceType)
  {
    return sourceType == typeof(string);
  }

  /// <summary>
  /// Converts from object to the expected numeric type using invariant culture.
  /// </summary>
  public new object? ConvertFrom(object value)
  {
    return ConvertFrom(null, CultureInfo.InvariantCulture, value);
  }

  /// <inheritdoc/>
  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
  {
    if (culture == null)
      culture = CultureInfo.InvariantCulture;
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    if (value is null)
      return null;
    if (value is string str)
    {
      if (str == String.Empty)
        return null;
      if (ExpectedType == typeof(string))
        return str;
      if (TryParseAnyNumber(ExpectedType, str, NumberStyle, culture, out var number))
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
      }
      return number;
    }
    return base.ConvertFrom(context!, culture, value);
  }

  /// <summary>
  /// Universal method to parse any number from string.
  /// </summary>
  /// <param name="expectedType"></param>
  /// <param name="str"></param>
  /// <param name="numberStyle"></param>
  /// <param name="culture"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public bool TryParseAnyNumber(Type? expectedType, string? str, NumberStyles numberStyle, CultureInfo? culture, out object? value)
  {
    value = null;
    if (str == null || str == String.Empty) return true;
    culture ??= CultureInfo.InvariantCulture;
    bool isFloat = false;
    bool isNegative = false;
    if (str.StartsWith("NAN", StringComparison.InvariantCultureIgnoreCase))
    {
      value = double.NaN;
      return true;
    }
    if (str.StartsWith("-INF", StringComparison.InvariantCultureIgnoreCase))
    {
      value = double.NegativeInfinity;
      return true;
    }
    if (str.StartsWith("INF", StringComparison.InvariantCultureIgnoreCase))
    {
      value = double.PositiveInfinity;
      return true;
    }


    if (str.Contains("E+") || str.Contains("E-") || str.Contains("e+") || str.Contains("e-") || str.Contains(culture.NumberFormat.NumberDecimalSeparator))
    {
      numberStyle |= NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
      isFloat = true;
    }
    if (str.Contains(culture.NumberFormat.NegativeSign))
    {
      numberStyle |= NumberStyles.AllowLeadingSign;
      isNegative = true;
    }

    Type[] intTypes = { typeof(SByte), typeof(Int16), typeof(Int32), typeof(Int64) };
    Type[] uintTypes = { typeof(Byte), typeof(UInt16), typeof(UInt32), typeof(UInt64) };

    if (expectedType == null)
    {
      if (isFloat)
        expectedType = typeof(double);
      else if (isNegative)
        expectedType = typeof(Int64);
      else
        expectedType = typeof(UInt64);
    }
    else
    {
      if (isFloat && intTypes.Contains(expectedType))
        throw new InvalidOperationException($"Type {expectedType} cannot be parsed from '{str}'");
      if (isNegative && uintTypes.Contains(expectedType))
        throw new InvalidOperationException($"Type {expectedType} cannot be parsed from '{str}'");
    }
    if (expectedType == typeof(float))
    {
      if (float.TryParse(str, numberStyle, culture, out var num))
      {
        value = num;
        return true;
      }
    }
    else if (ExpectedType == typeof(decimal))

    {
      if (decimal.TryParse(str, numberStyle, culture, out var num))
      {
        value = num;
        return true;
      }
    }
    else if (ExpectedType == typeof(double))
    {
      if (double.TryParse(str, numberStyle, culture, out var num))
      {
        value = num;
        return true;
      }
      return false;
    }
    if (expectedType == typeof(Byte))
    {
      if (Byte.TryParse(str, numberStyle, culture, out var num))
      {
        value = num;
        return true;
      }
      return false;
    }

    if (ExpectedType == typeof(SByte))
    {
      if (SByte.TryParse(str, numberStyle, culture, out var num))
      {
        value = num;
        return true;
      }
      return false;
    }

    if (ExpectedType == typeof(Int16))
    {
      if (Int16.TryParse(str, numberStyle, culture, out var num))
      {
        value = num;
        return true;
      }
      return false;
    }

    if (ExpectedType == typeof(UInt16))
    {
      if (UInt16.TryParse(str, numberStyle, culture, out var num))
      {
        value = num;
        return true;
      }
      return false;
    }

    if (ExpectedType == typeof(Int32))
    {
      if (Int32.TryParse(str, numberStyle, culture, out var num))
      {
        value = num;
        return true;
      }
      return false;
    }

    if (ExpectedType == typeof(UInt32))
    {
      if (UInt32.TryParse(str, numberStyle, culture, out var num))
      {
        value = num;
        return true;
      }
      return false;
    }

    if (ExpectedType == typeof(Int64))
    {
      if (Int64.TryParse(str, numberStyle, culture, out var num))
      {
        value = num;
        return true;
      }
      return false;
    }

    if (ExpectedType == typeof(UInt64))
    {
      if (UInt64.TryParse(str, numberStyle, culture, out var num))
      {
        value = num;
        return true;
      }
      return false;
    }
    return false;
  }
}
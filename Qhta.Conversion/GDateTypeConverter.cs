namespace Qhta.Conversion;

/// <summary>
/// Type converter for GDate (Generic Date) structure.
/// </summary>
public class GDateTypeConverter : BaseTypeConverter
{
  /// <summary>
  /// Sets ExpectedType to GDate.
  /// </summary>
  public GDateTypeConverter()
  {
    ExpectedType = typeof(GDate);
  }

  /// <summary>
  ///   Specifies whether to add the time zone to day.
  /// </summary>
  public bool ShowTimeZone { get; set; }

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

    if (value is GDate dt)
      if (destinationType == typeof(string))
      {
        var sb = new StringBuilder();
        switch (SimpleType)
        {
          case Xml.SimpleType.GYear:
            return dt.Year.ToString("D4");
          case Xml.SimpleType.GYearMonth:
            return dt.Year.ToString("D4") + "-" + dt.Month.ToString("D2");
          case Xml.SimpleType.GMonth:
            return "--" + dt.Month.ToString("D2");
          case Xml.SimpleType.GMonthDay:
            sb.Append("--" + dt.Month.ToString("D2") + "-" + dt.Day.ToString("D2"));
            if (ShowTimeZone)
              sb.Append(ZoneToStr(dt));
            return sb.ToString();
          case Xml.SimpleType.GDay:
            sb.Append("---" + dt.Day.ToString("D2"));
            if (ShowTimeZone)
              sb.Append(ZoneToStr(dt));
            return sb.ToString();
          default:
            if (dt.Year != 0)
              sb.Append(dt.Year.ToString("D4"));
            else
              sb.Append('-');
            sb.Append('-');
            if (dt.Month != 0)
              sb.Append(dt.Month.ToString("D2"));
            else
              sb.Append('-');
            sb.Append('-');
            if (dt.Day != 0)
              sb.Append(dt.Day.ToString("D2"));
            else
              sb.Append('-');
            break;
        }
        var str = sb.ToString();
        if (ShowTimeZone && dt.Day != 0)
          str += ZoneToStr(dt);
        else
          while (str.EndsWith("-"))
            str = str.Substring(0, str.Length - 1);
        if (str.StartsWith("----"))
          str = str.Substring(1, str.Length - 1);
        return str;
      }
    return base.ConvertTo(context, culture, value, destinationType);
  }

  private string ZoneToStr(GDate dt)
  {
    if (dt.Zone > 0)
      return "+" + dt.Zone.ToString("D2");
    if (dt.Zone < 0)
      return dt.Zone.ToString("D2");
    return "Z";
  }

  /// <inheritdoc/>
  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  /// <inheritdoc/>
  public new object? ConvertFrom(object value)
  {
    return ConvertFrom(null, null, value);
  }

  /// <inheritdoc/>
  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    if (value == null)
      return null;
    if (value is string str)
    {
      if (str == String.Empty)
        return null;
      var result = new GDate();
      var i = 0;
      var
        k = 0; // 0 - year expected, 1 - first separator, 2 - month exp, 3 - second sep, 4 - day exp, 5 - third sep, 6 - zone expected, 7 -end string od string
      var val = 0;
      var plus = false;
      var wasDigit = false;
      while (i <= str.Length)
      {
        var ch = i < str.Length ? str[i] : '\0';
        if (ch is >= '0' and <= '9')
        {
          val = val * 10 + ch - '0';
          if (!wasDigit)
          {
            wasDigit = true;
            k++;
          }
        }
        else if (ch == '-' || ch == '\0' || ch == '+' || ch == 'Z')
        {
          switch (k)
          {
            case 0:
            case 1:
              if (val > 9999)
                throw new InvalidDataException($"Invalid year value in string \"{str}\"");
              result.Year = (ushort)val;
              break;
            case 2:
            case 3:
              if (val > 12)
                throw new InvalidDataException($"Invalid month value in string \"{str}\"");
              result.Month = (byte)val;
              break;
            case 4:
            case 5:
              if (val > 31)
                throw new InvalidDataException($"Invalid day value in string \"{str}\"");
              result.Day = (byte)val;
              break;
            case 6:
            case 7:
              if (val > 12)
                throw new InvalidDataException($"Invalid zone value in string \"{str}\"");
              if (plus)
                result.Zone = (sbyte)val;
              else
                result.Zone = (sbyte)-val;
              break;
            default:
              if (ch == '\0')
                break;
              throw new InvalidDataException($"Invalid '-' at index {i} in string \"{str}\"");
          }
          if (ch == '\0')
            break;
          if (ch == 'Z')
          {
            if (k == 5 || k == 4)
              break;
            throw new InvalidDataException($"Invalid character 'Z' at index {i} in string \"{str}\"");
          }
          if (ch == '+')
          {
            if (k == 5 || k == 4)
              plus = true;
            else
              throw new InvalidDataException($"Invalid '+' at index {i} in string \"{str}\"");
          }
          val = 0;
          wasDigit = false;
          k++;
        }
        else
        {
          throw new InvalidDataException($"Invalid character at index {i} in string \"{str}\"");
        }
        i++;
      }
      return result;
    }
    return base.ConvertFrom(context, culture, value);
  }
}
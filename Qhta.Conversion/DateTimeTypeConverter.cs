namespace Qhta.Conversion;

/// <summary>
/// This converter supports the following types: DateTime, DateTimeOffset, DateOnly and TimeOnly
/// (the last two types are supported when compiled for .NET 6 or greater version).
/// Has several properties that specify format of output string.
/// </summary>
public class DateTimeTypeConverter : BaseTypeConverter
{
  /// <summary>
  /// Sets ExpectedType to DateTime and XsdType to XsdSimpleType.DateTime.
  /// </summary>
  public DateTimeTypeConverter()
  {
    ExpectedType = typeof(DateTime);
    SimpleType = Xml.SimpleType.DateTime;
  }

  /// <summary>
  ///   The character to insert between the date and time when serializing a DateTime value.
  /// </summary>
  public char DateTimeSeparator { get; set; } = ' ';

  /// <summary>
  ///   Specifies whether to add the fractional part of seconds to time format.
  /// </summary>
  public bool ShowFullTime { get; set; }

  /// <summary>
  ///   Specifies whether to add the time zone to time format.
  /// </summary>
  public bool ShowTimeZone { get; set; }

  /// <summary>
  /// Can specify DateTimeFormatInfo as defined in System.Globalization.CultureInfo.
  /// </summary>
  public DateTimeFormatInfo? FormatInfo { get; set; }

  /// <summary>
  /// Can specify DateTimeStyle as defined in System.Globalization.CultureInfo.
  /// </summary>
  public DateTimeStyles DateTimeStyle { get; set; }

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
    var mode = SimpleType;
    var showTimeZone = ShowTimeZone || value is DateTimeOffset;
#if NET6_0_OR_GREATER
    if (value is DateOnly dateOnly)
    {
      value = new DateTimeOffset(dateOnly.ToDateTime(new TimeOnly()));
      mode = Xml.SimpleType.Date;
    }
    else if (value is TimeOnly timeOnly)
    {
      value = new DateTimeOffset(new DateOnly(1, 1, 1).ToDateTime(timeOnly));
      mode = Xml.SimpleType.Time;
    }
    else 
#endif
    if (value is DateTime dateTime)
    {
      value = new DateTimeOffset(dateTime);
    }

    if (value is DateTimeOffset dt)
      if (destinationType == typeof(string))
      {
        var format = Format;
        if (format == null)
        {
          var showFullTime = ShowFullTime || dt.TimeOfDay.Milliseconds != 0;
          switch (mode)
          {
            case Xml.SimpleType.DateTime:
              format = GetDateTimeFormat(culture, showFullTime, showTimeZone);
              break;
            case Xml.SimpleType.Date:
              format = GetDateFormat(culture);
              break;
            case Xml.SimpleType.Time:
              format = GetTimeFormat(culture, showFullTime, showTimeZone);
              break;
            default:
              if (dt.TimeOfDay.TotalMilliseconds == 0)
                format = GetDateFormat(culture);
              else
                format = GetDateTimeFormat(culture, showFullTime, showTimeZone);
              break;
          }
        }
        return dt.ToString(format);
      }
    return base.ConvertTo(context, culture, value, destinationType);
  }


  private string GetDateTimeFormat(CultureInfo? culture, bool showFullTime, bool showTimeZone)
  {
    var format = GetDateFormat(culture) + DateTimeSeparator + GetTimeFormat(culture, showFullTime, showTimeZone);
    return format;
  }

  private string GetDateFormat(CultureInfo? culture)
  {
    if (culture != null && culture != CultureInfo.InvariantCulture)
      return culture.DateTimeFormat.ShortDatePattern;
    if (FormatInfo != null)
      return FormatInfo.ShortDatePattern;
    return "yyyy-MM-dd";
  }


  private string GetTimeFormat(CultureInfo? culture, bool showFullTime, bool showTimeZone)
  {
    string format;
    if (culture != null && culture != CultureInfo.InvariantCulture)
    {
      format = culture.DateTimeFormat.LongTimePattern;
    }
    else if (FormatInfo != null)
    {
      format = FormatInfo.LongTimePattern;
    }
    else
    {
      format = "HH:mm:ss";
      if (showFullTime)
        format += ".fffffff";
      if (showTimeZone)
        format += "zzzz";
    }
    return format;
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
      var style = DateTimeStyle;
      var format = Format;
      DateTimeOffset result;
      if (format != null) result = DateTimeOffset.ParseExact(str, format, culture, style);
      else if (culture != null) result = DateTimeOffset.Parse(str, culture, style);
      else if (FormatInfo != null) result = DateTimeOffset.Parse(str, FormatInfo, style);
      else result = DateTimeOffset.Parse(str, null, style);

#if NET6_0_OR_GREATER
      if (ExpectedType == typeof(DateOnly))
        return DateOnly.FromDateTime(result.Date);

      if (ExpectedType == typeof(TimeOnly))
        return TimeOnly.FromDateTime(result.DateTime);
#endif

      if (ExpectedType == typeof(DateTime))
        return result.DateTime;

      //if (XsdType == XsdSimpleType.Time)
      //{
      //  var timeOnly = TimeOnly.FromDateTime(result.DateTime);
      //  var dateOnly = new DateOnly(1, 1, 1);
      //  result = dateOnly.ToDateTime(timeOnly);
      //  if (ExpectedType == typeof(DateTimeOffset)
      //}
      return result;
    }
    return base.ConvertFrom(context, culture, value);
  }
}
using System.Buffers.Text;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace Qhta.Conversion;

public enum DateTimeConversionMode
{
  Default,
  DateTime,
  DateOnly,
  TimeOnly,
}

public class DateTimeTypeConverter : TypeConverter
{
  public DateTimeConversionMode Mode { get; set; }

  public Type? ExpectedType { get; set; }

  /// <summary>
  /// The character to insert between the date and time when serializing a DateTime value.
  /// </summary>
  public char DateTimeSeparator { get; set; } = ' ';

  /// <summary>
  /// Specifies whether to add the fractional part of seconds to time format.
  /// </summary>
  public bool ShowFullTime { get; set; }

  /// <summary>
  /// Specifies whether to add the time zone to time format.
  /// </summary>
  public bool ShowTimeZone { get; set; }

  /// <summary>
  /// Specifies format for ConvertTo method.
  /// </summary>
  public string? Format { get; set; }

  public DateTimeFormatInfo? FormatInfo  {get; set;}

  public DateTimeStyles DateTimeStyle { get; set;}

  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null;
    var mode = Mode;
    if (value is DateOnly dateOnly)
    {
      value = dateOnly.ToDateTime(new TimeOnly());
      mode = DateTimeConversionMode.DateOnly;
    }
    if (value is TimeOnly timeOnly)
    {
      value = new DateOnly(1, 1, 1).ToDateTime(timeOnly);
      mode = DateTimeConversionMode.TimeOnly;
    }

    if (value is DateTime dt)
    {
      if (destinationType == typeof(string))
      {
        string? format = Format;
        if (format == null)
          switch (mode)
          {
            case DateTimeConversionMode.DateTime:
              format = GetDateTimeFormat(culture);
              break;
            case DateTimeConversionMode.DateOnly:
              format = GetDateFormat(culture);
              break;
            case DateTimeConversionMode.TimeOnly:
              format = GetTimeFormat(culture);
              break;
            default:
              if (dt.TimeOfDay.TotalMilliseconds == 0)
                format = GetDateFormat(culture);
              else
                format = GetDateTimeFormat(culture);
              break;
          }
        return dt.ToString(format);
      }
    }
    return base.ConvertTo(context, culture, value, destinationType);
  }

  private string GetDateTimeFormat(CultureInfo? culture)
  {
    var format = GetDateFormat(culture) + DateTimeSeparator + GetTimeFormat(culture);
    return format;
  }

  private string GetDateFormat(CultureInfo? culture)
  {

    if (culture != null)
      return culture.DateTimeFormat.ShortDatePattern;
    if (FormatInfo != null)
      return FormatInfo.ShortDatePattern;
    var format = "yyyy-MM-dd";
    return format;
  }

  private string GetTimeFormat(CultureInfo? culture)
  {
    var format = culture?.DateTimeFormat.LongTimePattern ?? FormatInfo?.ShortTimePattern ?? "HH:mm:ss";
    if (ShowFullTime)
      format += ".FFFFFFF";
    if (ShowTimeZone)
      format += "zzz";
    return format;
  }

  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  public new object? ConvertFrom(object value) => ConvertFrom(null, null, value);

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
      var result = (format!=null) ? DateTime.ParseExact(str, format, culture, style)
        : (culture != null) ? DateTime.Parse(str, culture, style) 
        : (FormatInfo != null) ? DateTime.Parse(str, FormatInfo, style) 
        : DateTime.Parse(str, null, style);

      if (ExpectedType == typeof(DateOnly))
        return DateOnly.FromDateTime(result);

      if (ExpectedType == typeof(TimeOnly))
        return TimeOnly.FromDateTime(result);

      if (Mode == DateTimeConversionMode.TimeOnly)
      {
        var timeOnly = TimeOnly.FromDateTime(result);
        var dateOnly = new DateOnly(1, 1, 1);
        result = dateOnly.ToDateTime(timeOnly);
      }
      return result;
    }
    return base.ConvertFrom(context, culture, value);
  }
}
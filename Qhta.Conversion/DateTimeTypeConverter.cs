using System.Buffers.Text;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace Qhta.Conversion;

public enum DateTimeFormatMode
{
  Default,
  DateTime,
  DateOnly,
  TimeOnly,
}

public class DateTimeTypeConverter : TypeConverter
{
  public DateTimeFormatMode Mode { get; set; }

  /// <summary>
  /// The character to insert between the date and time when serializing a DateTime value.
  /// </summary>
  public char DateTimeSeparator { get; set; } = ' ';

  /// <summary>
  /// Specifies whether to display the fractional part of seconds when serializing a DateTime value.
  /// </summary>
  public bool ShowSecondsFractionalPart { get; set; }

  /// <summary>
  /// Specifies whether to display the time zone when serializing a DateTime value.
  /// </summary>
  public bool ShowTimeZone { get; set; }

  public string? Format { get; set; }

  public CultureInfo? Culture { get; set; }

  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string)
           || destinationType == typeof(DateOnly)
           || destinationType == typeof(TimeOnly);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value is DateOnly dateOnly)
      value = dateOnly.ToDateTime(new TimeOnly());
    else if (value is TimeOnly timeOnly)
    {
      dateOnly = new DateOnly(1, 1, 1);
      value = dateOnly.ToDateTime(timeOnly);
    }

    if (value is DateTime dt)
    {
      if (destinationType == typeof(DateOnly))
        return DateOnly.FromDateTime(dt);
      if (destinationType == typeof(TimeOnly))
        return TimeOnly.FromDateTime(dt);
      if (destinationType == typeof(string))
      {
        var format = Format;
        if (format == null)
          switch (Mode)
          {
            case DateTimeFormatMode.DateTime:
              format = "yyyy-MM-dd" + DateTimeSeparator + "HH:mm:ss";
              if (ShowSecondsFractionalPart)
                format += ".fffffff";
              if (ShowTimeZone)
                format += "zzz";
              break;
            case DateTimeFormatMode.DateOnly:
              format = "yyyy-MM-dd";
              break;
            case DateTimeFormatMode.TimeOnly:
              format = "HH:mm:ss";
              if (ShowSecondsFractionalPart)
                format += ".fffffff";
              if (ShowTimeZone)
                format += "zzz";
              break;
            default:
              if (dt.TimeOfDay.TotalMilliseconds == 0)
                format = "yyyy-MM-dd";
              else
              {
                format = "yyyy-MM-dd" + DateTimeSeparator + "HH:mm:ss";
                ;
                if (ShowSecondsFractionalPart)
                  format += ".fffffff";
                if (ShowTimeZone)
                  format += "zzz";
              }
              break;
          }
        return dt.ToString(format);
      }
    }
    return base.ConvertTo(context, culture, value, destinationType);
  }

  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string)
           || sourceType == typeof(DateOnly)
           || sourceType == typeof(TimeOnly);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    //if (value is DateOnly dateOnly)
    //  value = dateOnly.ToDateTime(new TimeOnly());
    //else if (value is TimeOnly timeOnly)
    //{
    //  dateOnly = new DateOnly(1, 1, 1);
    //  value = dateOnly.ToDateTime(timeOnly);
    //}

    if (value is string str)
    {
      var result = DateTime.Parse(str);

      if (Mode == DateTimeFormatMode.TimeOnly)
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
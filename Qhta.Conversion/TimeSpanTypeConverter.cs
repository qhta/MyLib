using System.Buffers.Text;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace Qhta.Conversion;

public class TimeSpanTypeConverter : TypeConverter
{

  /// <summary>
  /// Specifies format for ConvertTo method.
  /// </summary>
  public string? Format { get; set; }

  public TimeSpanStyles TimeSpanStyle { get; set;}

  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null;

    if (value is TimeSpan ts)
    {
      if (destinationType == typeof(string))
      {
        string? format = Format;
        return ts.ToString(format);
      }
    }
    return base.ConvertTo(context, culture, value, destinationType);
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
      var style = TimeSpanStyle;
      var format = Format;
      var result = (format!=null) 
        ? TimeSpan.ParseExact(str, format, culture, style)
        : TimeSpan.Parse(str);
      return result;
    }
    return base.ConvertFrom(context, culture, value);
  }
}
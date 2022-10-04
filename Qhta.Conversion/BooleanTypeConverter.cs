using System.ComponentModel;
using System.Globalization;

namespace Qhta.Conversion;

public enum BooleanConversionMode
{
  Boolean = 0,
  Numeric = 1,
  OnOff = 2,
}

public class BooleanTypeConverter : TypeConverter
{
  public BooleanConversionMode Mode { get; set; }

  public (string, string)[] BooleanStrings { get; set; }
    = { ("true", "false"), ("1", "0"), ("on", "off") };

  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value is null)
      return null;
    if (value is bool bv)
    {
      if (destinationType == typeof(string))
      {
        var result = bv ? BooleanStrings[(int)Mode].Item1 : BooleanStrings[(int)Mode].Item2;
        return result;
      }
    }
    return base.ConvertTo(context, culture, value, destinationType);
  }

  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    if (value is null)
      return null;
    if (value is string str)
    {
      if (str == String.Empty)
        return null;
      str = str.ToLowerInvariant();
      foreach (var bs in BooleanStrings)
      {
        if (str.Equals(bs.Item1, StringComparison.InvariantCultureIgnoreCase))
          return true;
        if (str.Equals(bs.Item2, StringComparison.InvariantCultureIgnoreCase))
          return false;
      }
    }
    return base.ConvertFrom(context, culture, value);
  }

}
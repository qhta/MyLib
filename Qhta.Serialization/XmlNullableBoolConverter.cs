using System;
using System.ComponentModel;
using System.Globalization;

namespace Qhta.Xml.Serialization;

public class XmlNullableBoolConverter : TypeConverter
{
  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (value == null)
      return true;
    if (value is string str)
    {
      if (str =="")
        return true;
      if (bool.TryParse(str, out var boolValue))
        return boolValue;
    }
    return base.ConvertFrom(context, culture, value);
  }

  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    return base.ConvertTo(context, culture, value, destinationType);
  }

}
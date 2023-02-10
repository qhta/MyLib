using System.ComponentModel;
using System.Globalization;

namespace Qhta.Conversion;

public class EnumTypeConverter : BaseTypeConverter
{
  private EnumConverter Base = null!;

  public EnumTypeConverter(Type enumType)
  {
    Base = new EnumConverter(enumType);
    ExpectedType = typeof(Enum);
    XsdType = XsdSimpleType.String;
  }
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null!;
    if (value is Enum Enum)
      if (Format != null)
        return Enum.ToString(Format);
    return Base.ConvertTo(context, culture, value, destinationType);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    if (value == null)
      return null;
    if (value is String str)
      if (str == string.Empty)
        return null;
    return Base.ConvertFrom(context, culture, value);
  }
}
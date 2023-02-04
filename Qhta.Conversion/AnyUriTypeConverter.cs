using System.ComponentModel;
using System.Globalization;

namespace Qhta.Conversion;

public class AnyUriTypeConverter : BaseTypeConverter
{
  private static UriTypeConverter Base = new UriTypeConverter();

  public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null!;
    return Base.ConvertTo(context, culture, value, destinationType);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    if (value == null)
      return null;
    return Base.ConvertFrom(context, culture, value);
  }
}
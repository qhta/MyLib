using System.ComponentModel;
using System.Globalization;

namespace Qhta.Conversion;

public class GuidTypeConverter : BaseTypeConverter
{
  private GuidConverter Base = new GuidConverter();

  public GuidTypeConverter()
  {
    ExpectedType = typeof(Guid);
    XsdType = XsdSimpleType.String;
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null!;
    if (value is Guid guid)
      if (Format != null)
        return guid.ToString(Format);
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
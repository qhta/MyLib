namespace Qhta.Conversion;

/// <summary>
/// StringTypeConverter converts a Unicode string to its serializable equivalent string (and vice versa). It can operate in three modes (Mode property):
/// <list type="bullet">
/// </list>
/// </summary>
public class AnyUriTypeConverter : BaseTypeConverter
{
  private static UriTypeConverter Base = new UriTypeConverter();
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

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
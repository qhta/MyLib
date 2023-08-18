namespace Qhta.Conversion;

/// <summary>
/// Guid datatype converter that uses Format property (defined in BaseTypeConverter). Uses the standard GuidConverter.
/// </summary>
public class GuidTypeConverter : BaseTypeConverter
{
  private GuidConverter Base = new GuidConverter();

  /// <summary>
  /// Sets ExpectedType to Guid and XsdType to XsdSimpleType.String.
  /// </summary>
  public GuidTypeConverter()
  {
    ExpectedType = typeof(Guid);
    XsdType = XsdSimpleType.String;
  }

  /// <inheritdoc/>
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  /// <inheritdoc/>
  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  /// <inheritdoc/>
  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null!;
    if (value is Guid guid)
      if (Format != null)
        return guid.ToString(Format);
    return Base.ConvertTo(context, culture, value, destinationType);
  }

  /// <inheritdoc/>
  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (value == null)
      return null;
    if (value is String str)
      if (str == string.Empty)
        return null;
    return Base.ConvertFrom(context, culture, value);
  }
}
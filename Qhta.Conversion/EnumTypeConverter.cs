namespace Qhta.Conversion;

/// <summary>
/// Formattable converter for enum type. Uses standard EnumTypeConverter.
/// Uses Format string in ConvertTo (as defined in Enum.ToString method).
/// Converts empty string to null.
/// </summary>
public class EnumTypeConverter : BaseTypeConverter
{
  private EnumConverter Base = null!;

  /// <summary>
  /// Creates an instance of standard EnumConverter for specified enumType.
  /// Sets ExpectedType to Enum and XsdType to XsdSimpleType.String.
  /// </summary>
  /// <param name="enumType"></param>
  public EnumTypeConverter(Type enumType)
  {
    Base = new EnumConverter(enumType);
    ExpectedType = typeof(Enum);
    SimpleType = Xml.SimpleType.String;
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
    if (value is Enum Enum)
      if (Format != null)
        return Enum.ToString(Format);
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
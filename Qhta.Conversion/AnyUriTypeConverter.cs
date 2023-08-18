namespace Qhta.Conversion;

/// <summary>
/// The Visual Studio community provides a UriTypeConverter to convert Uri type to String. 
/// This converter just gives the original Uri string without any conversion. 
/// On its basis, the AnyUriTypeConverter converter was defined, which implements the ITypeConverter interface 
/// and returns null when reverse conversion is performed for the empty string.
/// </summary>
public class AnyUriTypeConverter : BaseTypeConverter
{
  /// <summary>
  /// Internal original UriTypeConverter
  /// </summary>
  private static UriTypeConverter Base = new UriTypeConverter();

  /// <summary>
  /// Sets ExpectedType to Uri
  /// </summary>
  public AnyUriTypeConverter()
  {
    ExpectedType = typeof(Uri);
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
  public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null!;
    return Base.ConvertTo(context, culture, value, destinationType);
  }

  /// <inheritdoc/>
  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (value == null)
      return null;
    return Base.ConvertFrom(context, culture, value);
  }
}
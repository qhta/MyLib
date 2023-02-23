namespace Qhta.Xml.Reflection;

/// <summary>
/// Converter needed for qualified name serialization
/// </summary>
/// <seealso cref="System.ComponentModel.TypeConverter" />
public class QualifiedNameTypeConverter : TypeConverter
{
  /// <summary>
  /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
  /// </summary>
  /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
  /// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type you want to convert from.</param>
  /// <returns>
  ///   <see langword="true" /> if this converter can perform the conversion; otherwise, <see langword="false" />.
  /// </returns>
  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  /// <summary>
  /// Converts the given object to the type of this converter, using the specified context and culture information.
  /// </summary>
  /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
  /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
  /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
  /// <returns>
  /// An <see cref="T:System.Object" /> that represents the converted value.
  /// </returns>
  /// <exception cref="System.NotSupportedException">Cannot convert {nameof(XmlQualifiedTagName)} from {value.GetType().Name}</exception>
  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (value is string str)
      return new XmlQualifiedTagName(str);
    throw new NotSupportedException($"Cannot convert {nameof(XmlQualifiedTagName)} from {value.GetType().Name}");
  }

  /// <summary>
  /// Returns whether this converter can convert the object to the specified type, using the specified context.
  /// </summary>
  /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
  /// <param name="destinationType">A <see cref="T:System.Type" /> that represents the type you want to convert to.</param>
  /// <returns>
  ///   <see langword="true" /> if this converter can perform the conversion; otherwise, <see langword="false" />.
  /// </returns>
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  /// <summary>
  /// Converts the given value object to the specified type, using the specified context and culture information.
  /// </summary>
  /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
  /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If <see langword="null" /> is passed, the current culture is assumed.</param>
  /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
  /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the <paramref name="value" /> parameter to.</param>
  /// <returns>
  /// An <see cref="T:System.Object" /> that represents the converted value.
  /// </returns>
  /// <exception cref="System.NotSupportedException">Cannot convert {nameof(XmlQualifiedTagName)} to {destinationType.Name}</exception>
  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value is XmlQualifiedTagName aName && destinationType == typeof(string))
      return aName.ToString();
    throw new NotSupportedException($"Cannot convert {nameof(XmlQualifiedTagName)} to {destinationType.Name}");
  }
}
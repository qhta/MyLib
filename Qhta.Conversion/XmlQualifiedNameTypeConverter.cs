namespace Qhta.Conversion;

/// <summary>
/// Converts an XmlQualifiedName (defined in System.Xml) value to string and backward.
/// </summary>
public class XmlQualifiedNameTypeConverter : BaseTypeConverter
{
  /// <summary>
  /// Sets ExpectedType to XmlQualifiedName and XsdType to QName.
  /// </summary>
  public XmlQualifiedNameTypeConverter()
  {

    ExpectedType = typeof(XmlQualifiedName);
    XsdType = XsdSimpleType.QName;
  }

  /// <inheritdoc/>
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  /// <inheritdoc/>
  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null;
    if (value is XmlQualifiedName xmlQualifiedName)
      if (destinationType == typeof(string))
        return xmlQualifiedName.ToString();
    return base.ConvertTo(context, culture, value, destinationType);
  }

  /// <inheritdoc/>
  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  /// <inheritdoc/>
  public new object? ConvertFrom(object value)
  {
    return ConvertFrom(null, null, value);
  }

  /// <inheritdoc/>
  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    if (value == null)
      return null;
    if (value is string str)
    {
      if (str == String.Empty)
        return null;

      if (ExpectedType == typeof(XmlQualifiedName))
      {
        var ss = str.Split(':');
        if (ss.Length == 2)
          return new XmlQualifiedName(ss[1], ss[0]);
        return new XmlQualifiedName(str);
      }
    }
    return base.ConvertFrom(context, culture, value);
  }
}
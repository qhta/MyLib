namespace Qhta.Conversion
{
  /// <summary>
  /// Double to string converter that uses the specific Culture and Format string (as defined in BaseTypeConverter).
  /// If culture is not declared, then Invariant culture is used.
  /// </summary>
  public class DoubleTypeConverter : BaseTypeConverter
  {

    /// <inheritdoc/>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
      if (value == null) return null;
      double x = (double)value;
      return x.ToString(Format, Culture ?? CultureInfo.InvariantCulture);
    }

    /// <inheritdoc/>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
      if (value == null)
        return null;
      if (value is string str)
      {
        if (str == string.Empty)
          return null;
        return Double.Parse(str, culture);
      }
      return base.ConvertFrom(null, Culture ?? CultureInfo.InvariantCulture, value);
    }
  }
}

namespace Qhta.Conversion
{
  /// <summary>
  /// Formater for double value.
  /// </summary>
  public class FloatDoubleFormater : IValueConverter
  {
    /// <summary>
    /// Converts value to string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double x = (double)value;
      return x.ToString((string)parameter, culture);
    }

    /// <summary>
    /// Converts back - not implemented.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}

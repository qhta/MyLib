using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// One-way comparing converter. It compares a value to the parameter and returns a boolean value.
  /// </summary>
  public class EqualityComparingConverter : IValueConverter
  {

    /// <summary>
    /// Compares a value to the parameter and returns a boolean value.
    /// </summary>
    public object Convert (object? value, Type targetType, object? parameter, CultureInfo culture)
    {

      if (value is string valStr && parameter is string parStr)
      {
        var result = String.Equals(valStr, parStr);
        return result;
      }
      if (value is string || parameter is string)
      {
        var valStr1 = value?.ToString();
        var parStr1 = parameter?.ToString();
        var result = String.Equals(valStr1, parStr1);
        return result;
      }
      if (value != null)
      {
        var result = value.Equals(parameter);
      }
      if (parameter != null)
      {
        var result = parameter.Equals(value);
        return result;
      }

      return true;
    }

    /// <summary>
    /// Unimplemented backward conversion.
    /// </summary>
    public object ConvertBack (object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }


  }
}

using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// Value converter for formatting string and arguments.
/// </summary>
public class StringFormattingValueConverter : IMultiValueConverter
{
  /// <summary>
  /// Value is a formatting string, parameter is the value to format.
  /// </summary>
  public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {
    try
    {
      return String.Format(parameter as string ?? "", values);
    } catch (Exception e)
    {
      Debug.WriteLine(e);
    }
    return null;
  }

  /// <summary>
  /// Not implemented for multi-value conversion.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetTypes"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  /// <summary>
  /// Konwerter porównujący wartość z parametrem. Wynikiem jest boolean
  /// </summary>
  public class EqualityComparingConverter : IValueConverter
  {

    /// <summary>
    /// Konwersja wprost
    /// </summary>
    public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
    {

      if (value is string valStr && parameter is string parStr)
      {
        var result = String.Equals(valStr, parStr);
        //Debug.WriteLine($"{valStr} == {parStr} = {result}");
        return result;
      }
      if (value is string || parameter is string)
      {
        var valStr1 = value?.ToString();
        var parStr1 = parameter?.ToString();
        var result = String.Equals(valStr1, parStr1);
        //Debug.WriteLine($"{valStr1} == {parStr1} = {result}");
        return result;
      }
      if (value != null)
      {
        var result = value.Equals(parameter);
        //Debug.WriteLine($"{value} == {parameter} = {result}"); return result;
      }
      if (parameter != null)
      {
        var result = parameter.Equals(value);
        //Debug.WriteLine($"{value} == {parameter} = {result}"); return result;
        return result;
      }
      //Debug.WriteLine($"{value} == {parameter} = {true}");

      return true;
    }

    /// <summary>
    /// Konwersja wstecz
    /// </summary>
    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new ArgumentNullException("value");
    }


  }
}

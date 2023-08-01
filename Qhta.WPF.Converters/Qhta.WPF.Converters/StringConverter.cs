using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// Abstract StringConverter. Convert method is abstract.
  /// ConvertBack method is unimplemented.
  /// Converter has string <see cref="Param"/> dependency property.
  /// It can be declared in XAML code.
  /// Other option is to declare ConverterParameter in Binding.
  /// </summary>
  public abstract class StringConverter: DependencyObject, IValueConverter
  {
    /// <summary>
    /// Static dependency property of double parameter.
    /// </summary>
    public static DependencyProperty ParamProperty = DependencyProperty.Register
      ("Param", typeof(string), typeof(StringConverter));

    /// <summary>
    /// Dependency property of double parameter.
    /// </summary>
    public string Param
    {
      get => (string)GetValue(ParamProperty);
      set => SetValue(ParamProperty, value);
    }

    /// <summary>
    /// Abstract method to implement an arithmetic operation.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

    /// <summary>
    /// Convert back unimplemented method.
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

    /// <summary>
    /// A method to convert an object parameter to string.
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected bool TryGetValue(object parameter, out string? value)
    {
      value = parameter.ToString();
      return true;
    }

  }
}

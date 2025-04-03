using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// Abstract ArithmeticConverter. Convert method is abstract.
  /// ConvertBack method is unimplemented.
  /// Converter has double <see cref="Param"/> dependency property.
  /// It is initialized to NaN value and can be declared in XAML code.
  /// Other option is to declare ConverterParameter in Binding.
  /// </summary>
  public abstract class ArithmeticConverter: DependencyObject, IValueConverter
  {
    /// <summary>
    /// Static dependency property of double parameter.
    /// </summary>
    public static DependencyProperty ParamProperty = DependencyProperty.Register
      (nameof(Param), typeof(double), typeof(ArithmeticConverter), new PropertyMetadata(Double.NaN));

    /// <summary>
    /// Dependency property of double parameter.
    /// </summary>
    public double Param
    {
      get => (double)GetValue(ParamProperty);
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
    public abstract object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture);

    /// <summary>
    /// Convert back unimplemented method.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// A method to convert an object? parameter to double.
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected bool TryGetValue(object? parameter, out double value)
    {
      value = double.NaN;
      if (parameter is string str)
        return double.TryParse(str, out value);
      if (parameter is Double)
        value = (Double)parameter;
      else if (parameter is Single)
        value = (Single)parameter;
      else if (parameter is Int32)
        value = (Int32)parameter;
      else if (parameter is Int16)
        value = (Int16)parameter;
      else if (parameter is Byte)
        value = (Byte)parameter;
      else if (parameter is Int64)
        value = (Int64)parameter;
      else if (parameter is UInt64)
        value = (UInt64)parameter;
      else if (parameter is UInt32)
        value = (UInt32)parameter;
      else if (parameter is UInt16)
        value = (UInt16)parameter;
      else if (parameter is SByte)
        value = (SByte)parameter;
      else
        return false;
      return true;
    }

  }
}

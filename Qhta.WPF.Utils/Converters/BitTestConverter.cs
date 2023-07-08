using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  /// <summary>
  /// One way converter to convert a specific bitset to bool.
  /// It return true value When a bitset value equals int mask,
  /// otherwise it returns false.
  /// </summary>
  public class BitTestConverter : DependencyObject, IValueConverter, IMultiValueConverter
  {
    /// <summary>
    /// Static mask property to test a bitset value
    /// </summary>
    public static DependencyProperty MaskProperty = DependencyProperty.Register
      ("Mask", typeof(int), typeof(BitTestConverter), new PropertyMetadata(0));

    /// <summary>
    /// Mask property to test a bitset value.
    /// </summary>
    public int Mask
    {
      get => (int)GetValue(MaskProperty);
      set => SetValue(MaskProperty, value);
    }

    /// <summary>
    /// Converts bitset value to bool.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        value = 0;
      if (ConvertArguments(value, parameter, out int val, out int mask))
      {
        return (val & mask) != 0;
      }
      throw new InvalidOperationException($"{this.GetType().Name} can't test {value} and {parameter}");
    }

    /// <summary>
    /// Converts an array of object to bool.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }

    /// <summary>
    /// Converts bool to bitset value. Not implemented.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Converts bool to an array of values. Not implemented.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetTypes"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// A method to convert an object to int value.
    /// The object value can be an enum or any integer type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="val"></param>
    /// <param name="mask"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected bool ConvertArguments(object value, object parameter, out int val, out int mask)
    {

      Dictionary<string, int> enums = null!;
      if (value is int n)
        val = n;
      else
      if (value.GetType().IsEnum)
      {
        val = (int)value;
        enums = GetEnumValues(value.GetType());
      }
      else if (value is byte b)
      {
        val = b;
      }
      else if (value is UInt16 w)
      {
        val = w;
      }
      else if (value is UInt32 u)
      {
        val = (int)u;
      }
      else
        throw new InvalidOperationException($"{this.GetType().Name} type {value.GetType().Name} can not be tested");

      return ConvertParameterArgument(parameter, enums, out mask);
    }

    /// <summary>
    /// Helper method to get enum type values as string-int dictionary.
    /// </summary>
    /// <param name="enumType"></param>
    /// <returns></returns>
    protected Dictionary<string, int> GetEnumValues(Type enumType)
    {
      var enums = new Dictionary<string, int>();
      foreach (var enumVal in enumType.GetEnumValues())
        enums.Add(enumVal.ToString() ?? string.Empty, (int)enumVal);
      return enums;
    }

    /// <summary>
    /// Helper method to convert an object parameter to int mask
    /// eventually using string-int dictionary.
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="dictionary"></param>
    /// <param name="mask"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected bool ConvertParameterArgument(object parameter, Dictionary<string, int> dictionary, out int mask)
    {
      if (parameter == null)
      {
        mask = Mask;
        return true;
      }
      if (parameter is int nMask)
        mask = nMask;
      else
      if (parameter.GetType().IsEnum)
      {
        mask = (int)parameter;
      }
      else if (parameter is byte b)
      {
        mask = b;
      }
      else if (parameter is UInt16 w)
      {
        mask = w;
      }
      else if (parameter is UInt32 u)
      {
        mask = (int)u;
      }
      else
      if (parameter is String str && Int32.TryParse(str, out var nParsed))
      {
        mask = nParsed;
      }
      else if (parameter is String str2 && dictionary != null && dictionary.TryGetValue(str2, out var nEnum))
      {
        mask = nEnum;
      }
      else
        throw new InvalidOperationException($"{this.GetType().Name} parameter {parameter} is invalid");
      return true;
    }

  }
}

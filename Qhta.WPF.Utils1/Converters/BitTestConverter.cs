using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;

namespace Qhta.WPF.Utils
{
  /// <summary>
  /// One way value converter (value & parameter) => bool
  /// </summary>
  public class BitTestConverter : DependencyObject, IValueConverter, IMultiValueConverter
  {
    public static DependencyProperty MaskProperty = DependencyProperty.Register
      ("Mask", typeof(int), typeof(BitTestConverter), new PropertyMetadata(0));

    public int Mask
    {
      get => (int)GetValue(MaskProperty);
      set => SetValue(MaskProperty, value);
    }
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        value = 0;
      if (ConvertArguments(value, parameter, out int val, out int mask))
      {
        return (val & mask) != 0;
      }
      throw new InvalidOperationException($"{this.GetType().Name} can't test {value} and {parameter}");
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    protected bool ConvertArguments(object value, object parameter, out int val, out int mask)
    {

      Dictionary<string, int> enums = null;
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

    protected Dictionary<string, int> GetEnumValues(Type enumType)
    {
      var enums = new Dictionary<string, int>();
      foreach (var enumVal in enumType.GetEnumValues())
        enums.Add(enumVal.ToString(), (int)enumVal);
      return enums;
    }
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

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  public class NumericValueConverter: DependencyObject, IValueConverter
  {
    #region Culture property
    public CultureInfo Culture
    {
      get => (CultureInfo)GetValue(CultureProperty);
      set => SetValue(CultureProperty, value);
    }

    public static readonly DependencyProperty CultureProperty = DependencyProperty.Register
      ("Culture", typeof(CultureInfo), typeof(NumericValueConverter));
    #endregion

    #region Format property
    public string Format
    {
      get => (string)GetValue(FormatProperty);
      set => SetValue(FormatProperty, value);
    }

    public static readonly DependencyProperty FormatProperty = DependencyProperty.Register
      ("Format", typeof(string), typeof(NumericValueConverter));
    #endregion

    public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (targetType==typeof(string))
        return ConvertBack(value, targetType, parameter, culture);
      if (value == null)
        return 0;
      else if (value is string)
      {
        string s = value as string;
        s = s.Trim ();
        if (s.Length==0)
          return 0;
        if (targetType==typeof(SByte))
        {
          if (SByte.TryParse(s, NumberStyles.Number, Culture ?? culture, out SByte d))
            return d;
        }
        else
        if (targetType==typeof(Int16))
        {
          if (Int16.TryParse(s, NumberStyles.Number, Culture ?? culture, out Int16 d))
            return d;
        }
        else
        if (targetType==typeof(Int32))
        {
          if (Int32.TryParse(s, NumberStyles.Number, Culture ?? culture, out Int32 d))
            return d;
        }
        else
        if (targetType==typeof(Int64))
        {
          if (Int64.TryParse(s, NumberStyles.Number, Culture ?? culture, out Int64 d))
            return d;
        }
        else
        if (targetType==typeof(Byte))
        {
          if (Byte.TryParse(s, NumberStyles.Number, Culture ?? culture, out Byte d))
            return d;
        }
        else
        if (targetType==typeof(UInt16))
        {
          if (UInt16.TryParse(s, NumberStyles.Number, Culture ?? culture, out UInt16 d))
            return d;
        }
        else
        if (targetType==typeof(UInt32))
        {
          if (UInt32.TryParse(s, NumberStyles.Number, Culture ?? culture, out UInt32 d))
            return d;
        }
        else
        if (targetType==typeof(UInt64))
        {
          if (UInt64.TryParse(s, NumberStyles.Number, Culture ?? culture, out UInt64 d))
            return d;
        }
        else
        if (targetType==typeof(Double))
        {
          if (Double.TryParse(s, NumberStyles.Number, Culture ?? culture, out Double d))
            return d;
        }
        else
        if (targetType==typeof(Single))
        {
          if (Single.TryParse(s, NumberStyles.Number, Culture ?? culture, out Single d))
            return d;
        }
        else
        if (targetType==typeof(Decimal))
        {
          if (Decimal.TryParse(s, NumberStyles.Number, Culture ?? culture, out Decimal d))
            return d;
        }
        return double.NaN;
      }
      else
        throw new NotImplementedException ();
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value==null || value is string)
        return Convert(value, targetType, parameter, culture);
      string format = Format ?? parameter as string;
      string result=null;
      if (value is SByte i8)
        result = (i8.ToString(format, Culture ?? culture));
      else
      if (value is Int16 i16)
        result = (i16.ToString(format, Culture ?? culture));
      else 
      if (value is Int32 i32)
        result = (i32.ToString(format, Culture ?? culture));
      else 
      if (value is Int64 i64)
        result = (i64.ToString(format, Culture ?? culture));
      else
      if (value is Byte u8)
        result = (u8.ToString(format, Culture ?? culture));
      else
      if (value is UInt16 u16)
        result = (u16.ToString(format, Culture ?? culture));
      else
      if (value is UInt32 u32)
        result = (u32.ToString(format, Culture ?? culture));
      else
      if (value is UInt64 u64)
        result = (u64.ToString(format, Culture ?? culture));
      else
      if (value is Double d)
        result = (d.ToString (format, Culture ?? culture));
      else 
      if (value is Single f)
        result = (f.ToString(format, Culture ?? culture));
      else 
      if (value is Decimal m)
        result = (m.ToString(format, Culture ?? culture));
      return result;
    }
  }
}

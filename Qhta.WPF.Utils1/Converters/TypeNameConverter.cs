using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  /// <summary>
  /// Konwerter nazwy typu
  /// </summary>
  public class TypeNameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return null;
      if (value is Type type)
      {
        return Qhta.TypeUtils.TypeNaming.GetTypeName(type);
      }
      if (value is String str)
        return str;
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}


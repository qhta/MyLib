using System;
using System.Collections.Generic;
using System.Globalization;

namespace Qhta.WPF.Utils
{
  /// <summary>
  /// Two way value converter (value & parameter) => bool
  /// </summary>
  public class BitSetConverter : BitTestConverter
  {

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return null;
      if (ConvertArguments(value, parameter, out int val, out int mask))
      {
        val = val | mask;
        value = val;
      }
      throw new InvalidOperationException($"{this.GetType().Name} can't convert {value} with parameter {parameter} to {targetType}");
    }

  }
}

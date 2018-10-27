using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  public class SafeCollectionConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is ICollection collection)
      {
        object[] array = new object[collection.Count];
        collection.CopyTo(array, 0);
        return array;
      }
      throw new NotImplementedException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}

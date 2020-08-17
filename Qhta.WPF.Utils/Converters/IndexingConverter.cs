using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  public class IndexingConverter: IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var obj = values[0];
      var indexProp = values[1];
      if (obj != DependencyProperty.UnsetValue)
      {
        string str = indexProp as string;
        if (indexProp is Binding)
        {

        }
        foreach (PropertyInfo property in obj.GetType().GetProperties())
        {
          ParameterInfo[] parameterInfos = property.GetIndexParameters();
          if (parameterInfos.Length == 1)
          {
            return property.GetValue(obj, new object[] { str });
          }
        }
      }
      return null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Qhta.UnicodeBuild.Helpers
{
  public class DictionaryBasedConverter: IValueConverter
  {

    public Dictionary<object, object> Dictionary { get; set; } = new Dictionary<object, object>();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return null;
      }
      return Dictionary.GetValueOrDefault(value);
    }


    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}

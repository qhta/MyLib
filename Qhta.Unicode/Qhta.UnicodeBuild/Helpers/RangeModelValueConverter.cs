using System.Globalization;
using System.Windows.Data;

namespace Qhta.UnicodeBuild.Helpers;

public class RangeModelValueConverter: IValueConverter
{
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is RangeModel range)
    {
      return range.ToString();
    }
    return null;
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is string str)
    {
      return RangeModel.Parse(str);
    }
    return null;
  }
}
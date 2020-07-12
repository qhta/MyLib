using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  public class IndexedAttachedPropertyConverter: IValueConverter
  {
    public IndexedAttachedPropertyConverter() { }

    public IndexedAttachedPropertyConverter(string propertyName) : this(propertyName, null) { }

    public IndexedAttachedPropertyConverter(string propertyName, object defaultValue)
    {
      PropertyName = propertyName;
      DefaultValue = defaultValue;
    }

    public string PropertyName { get; private set; }
    public object DefaultValue { get; private set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value!=null)
      {
        var sourceType = value.GetType();
        var indexers = sourceType.GetProperties().Where(info => info.GetIndexParameters().Length > 0);
        foreach (var indexer in indexers)
        {
          var source = indexer.GetValue(value, new object[] {(int)parameter});
          var sourceProperty = source.GetType().GetProperty(PropertyName);
          if (sourceProperty!=null)
            return sourceProperty.GetValue(source);
        }
      }
      return DefaultValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

  }
}

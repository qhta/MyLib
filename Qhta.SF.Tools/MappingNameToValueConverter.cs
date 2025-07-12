using System;
using System.Globalization;
using System.Windows.Data;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools
{
  public class MappingNameToValueConverter : IValueConverter
  {
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value is GridCell gridCell)
      {
        if (gridCell.ColumnBase?.GridColumn is GridTextColumn gridTextColumn)
        {
          var dataContext = gridCell.DataContext;
          string mappingName = gridTextColumn.MappingName;
          if (dataContext != null && !String.IsNullOrEmpty(mappingName))
          {
            var propertyInfo = dataContext.GetType().GetProperty(mappingName);
            return propertyInfo?.GetValue(dataContext)?.ToString() ?? string.Empty;
          }
        }
      }
      return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
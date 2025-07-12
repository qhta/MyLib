using System;
using System.Globalization;
using System.Windows.Data;
using Qhta.MVVM;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools
{
  public class MappingNameBinder : IValueConverter
  {
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value is Binding binding)
      {
        return binding.Source;
      }
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
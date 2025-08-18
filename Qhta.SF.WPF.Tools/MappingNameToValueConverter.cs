using System;
using System.Globalization;
using System.Windows.Data;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.WPF.Tools
{
  /// <summary>
  /// A value converter that retrieves the value of a property specified by the mapping name from a <see cref="GridCell"/>.
  /// </summary>
  public class MappingNameToValueConverter : IValueConverter
  {
    /// <summary>
    /// Converts a <see cref="GridCell"/> value to a string representation based on the value of the mapped property.
    /// </summary>
    /// <remarks>The method checks if the <paramref name="value"/> is a <see cref="GridCell"/> and if its
    /// column is a <see cref="GridTextColumn"/>. It then retrieves the property value from the data context using the
    /// column's mapping name. If the mapping name is null or empty, or if the property cannot be found, the method
    /// returns an empty string.</remarks>
    /// <param name="value">The value to convert, expected to be of type <see cref="GridCell"/>.</param>
    /// <param name="targetType">The type to which the value is being converted. This parameter is not used in the conversion process.</param>
    /// <param name="parameter">An optional parameter for the conversion. This parameter is not used in the conversion process.</param>
    /// <param name="culture">The culture to use in the conversion process. This parameter is not used in the conversion process.</param>
    /// <returns>A string representation of the property value specified by the column mapping name if the conversion is
    /// successful; otherwise, an empty string.</returns>
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

    /// <summary>
    /// Unimplemented method for converting back from the target type to the source type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
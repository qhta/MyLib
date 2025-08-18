using System;
using System.Globalization;
using System.Windows.Data;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.WPF.Tools
{
  /// <summary>
  /// A value converter that retrieves the mapping name from a <see cref="GridCell"/> or a <see cref="Binding"/>.
  /// </summary>
  public class MappingNameBinder : IValueConverter
  {
    /// <summary>
    /// Converts a given value to a specific target type using the provided culture information.
    /// </summary>
    /// <param name="value">The value to be converted. This can be a <see cref="Binding"/> or a <see cref="GridCell"/> object.</param>
    /// <param name="targetType">The type to which the value is to be converted. This parameter is not used in the conversion logic.</param>
    /// <param name="parameter">An optional parameter for the conversion. This parameter is not used in the conversion logic.</param>
    /// <param name="culture">The culture to be used in the conversion process. This parameter is not used in the conversion logic.</param>
    /// <returns>The source object if the value is a <see cref="Binding"/>; the string representation of the property value
    /// specified by the MappingName if the value is a <see cref="GridCell"/>; otherwise,
    /// an empty string.</returns>
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
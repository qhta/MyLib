using System.Globalization;
using System.Windows.Data;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.WPF.Tools;

/// <summary>
/// Provides a mechanism to convert <see cref="GridColumn"/> objects to specific property values for data binding in a
/// <see cref="SfDataGrid"/>.
/// </summary>
/// <remarks>This converter is used to extract specific properties from <see cref="GridColumn"/> instances, such
/// as the selection state.</remarks>
public class SfDataGridColumnConverter: IValueConverter
{
  /// <summary>
  /// Debug flag to enable or disable logging of conversion operations.
  /// </summary>
  public static bool LogIt;

  /// <summary>
  /// Converts a <see cref="GridColumn"/> object to a specified property value based on the provided parameter.
  /// </summary>
  /// <param name="value">The object to convert, expected to be of type <see cref="GridColumn"/>.</param>
  /// <param name="targetType">The type to which the value is being converted. This parameter is not used in the conversion process.</param>
  /// <param name="parameter">A string specifying the property name to retrieve from the <see cref="GridColumn"/>. Must be "IsSelected".</param>
  /// <param name="culture">The culture to use in the converter. This parameter is not used in the conversion process.</param>
  /// <returns>The value of the specified property from the <see cref="GridColumn"/>. For "IsSelected", returns the selection
  /// state.</returns>
  /// <exception cref="InvalidOperationException">Thrown if <paramref name="parameter"/> is not a string or if the property name is not recognized.</exception>
  /// <exception cref="NotImplementedException">Thrown if <paramref name="value"/> is not of type <see cref="GridColumn"/>.</exception>
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is GridColumn column)
    {
      if (LogIt)
        System.Diagnostics.Debug.WriteLine($"GridColumnConverter.Convert({value}, {targetType}, {parameter}, {culture})");
      if (parameter is not string propName)
        throw new InvalidOperationException("Parameter should be string");
      if (propName == "IsSelected")
        return SfDataGridColumnBehavior.GetIsSelected(column);
      else
        throw new InvalidOperationException("Property name not recognized");
    }
    throw new NotImplementedException();
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
  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
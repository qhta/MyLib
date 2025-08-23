using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.WPF.Tools;

/// <summary>
/// Converter for determining the visibility of the expander in a long text column.
/// </summary>
/// <remarks>Implementation similar to <see cref="GridColumnMappingConverter"/>, but instead of string value it returns Visibility.</remarks>
public class LongTextColumnExpanderVisibilityConverter : IMultiValueConverter
{
  /// <summary>
  /// Determines the visibility of the expander based on the length of the text in a long text column.
  /// </summary>
  /// <param name="values">Must be an array with first element bound to DataContext, second element bound to MappingName, and third element bound to GridCell itself.</param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public object? Convert(object?[]? values, Type targetType, object? parameter, CultureInfo culture)
  {
    if (values == null || values.Length < 3)
      return null;

    var dataContext = values[0];
    var propertyName = values[1]?.ToString();

    if (dataContext == null || string.IsNullOrEmpty(propertyName))
      return null;

    // Use reflection to get the property value dynamically
    var property = dataContext.GetType().GetProperty(propertyName);
    var value = property?.GetValue(dataContext);
    if (value is string longText && values[2] is GridCell cell && cell.ColumnBase.GridColumn is LongTextColumn column)
    {
      var cellHeight = column.EvaluateTextHeight(longText); 
      return cellHeight>cell.ActualHeight ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    }
    return System.Windows.Visibility.Collapsed; // Default to collapsed if conditions are not met
  }

  /// <summary>
  /// Converts a single value back to an array of values for binding purposes.
  /// </summary>
  /// <param name="value">The value produced by the binding target.</param>
  /// <param name="targetTypes">The array of target types to which the value is being converted. Must contain at least two elements.</param>
  /// <param name="parameter">An optional parameter to use during the conversion. This can be <see langword="null"/>.</param>
  /// <param name="culture">The culture to use in the conversion process.</param>
  /// <returns>An array of objects containing the converted values. Returns <see langword="null"/> if <paramref name="value"/> is
  /// <see langword="null"/> or if <paramref name="targetTypes"/> contains fewer than two elements.</returns>
  public object[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
  {
    if (value == null || targetTypes.Length < 3)
      return null;

    // Return the updated value for the property
    return [value, Binding.DoNothing];
  }
}
using System.Globalization;
using System.Windows.Data;

namespace Qhta.SF.WPF.Tools;

/// <summary>
/// Converter for getting and setting property values in a GridColumn based on the mapping name.
/// </summary>
public class GridColumnMappingConverter : IMultiValueConverter
{
  /// <summary>
  /// Gets the value of a property specified by the mapping name from the data context of a GridColumn.
  /// </summary>
  /// <param name="values">Must be an array with first element bound to DataContext, and second element bound to MappingName</param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public object? Convert(object?[]? values, Type targetType, object? parameter, CultureInfo culture)
  {
    if (values == null || values.Length < 2)
      return null;

    var dataContext = values[0];
    var propertyName = values[1]?.ToString();

    if (dataContext == null || string.IsNullOrEmpty(propertyName))
      return null;

    // Use reflection to get the property value dynamically
    var property = dataContext.GetType().GetProperty(propertyName);
    return property?.GetValue(dataContext);
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
    if (value == null || targetTypes.Length < 2)
      return null;

    // Return the updated value for the property
    return [value, Binding.DoNothing];
  }
}
using System.Globalization;
using System.Windows.Data;

namespace Qhta.SF.WPF.Tools;

/// <summary>
/// Value converter class for selectable item.
/// </summary>
public class SelectableItemValueConverter: IValueConverter
{
  /// <summary>
  /// Converts a selectable item to its display name.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is ISelectableItem selectableItem)
      return selectableItem.DisplayName;
    return value?.ToString();
  }

  /// <summary>
  /// Not implemented method for converting back from the target type to the source type.
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
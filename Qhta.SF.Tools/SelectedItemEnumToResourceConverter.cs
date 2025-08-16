using System.Globalization;

using Qhta.WPF.Converters;

namespace Qhta.SF.Tools;

/// <summary>
/// Works like <see cref="EnumToResourceConverter"/>, but if value is <see cref="ISelectableItem"/>, it takes its actual value.
/// </summary>
public class SelectedItemEnumToResourceConverter : EnumToResourceConverter
{
  /// <summary>
  /// Inherits from <see cref="EnumToResourceConverter"/> and overrides the Convert method to handle <see cref="ISelectableItem"/>.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public new object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is ISelectableItem selectableItem)
      value = selectableItem.ActualValue;
    return base.Convert(value, targetType, parameter, culture);
  }
}
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// One-way converter that gets a value from a dictionary.
  /// </summary>
  [Bindable(true)]
  [ContentProperty(nameof(Dictionary))]
  public class String2ObjectConverter: IValueConverter
  {
    /// <summary>
    /// Direct conversion.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value!=null)
      {
        var val = value.ToString() ?? string.Empty;
        if (Dictionary.TryGetValue(val, out var result))
          return result;
        return Dictionary[val.ToLowerInvariant()];
      }
      return null;
    }

    /// <summary>
    /// Returns a value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      return value;
    }

    /// <summary>
    /// A dictionary of string to object.
    /// </summary>
    public String2ObjectDictionary Dictionary { get; set; } = null!;
  }
}

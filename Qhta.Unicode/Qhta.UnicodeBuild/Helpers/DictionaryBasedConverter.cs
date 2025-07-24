using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.UnicodeBuild.Helpers
{
  /// <summary>
  /// Value converter that uses a dictionary to convert values to corresponding objects.  
  /// </summary>
  public class DictionaryBasedConverter: IValueConverter
  {
    /// <summary>
    /// Dictionary that maps keys to values for conversion.
    /// </summary>
    public Dictionary<object, object> Dictionary { [DebuggerStepThrough] get; set; } = new Dictionary<object, object>();

    /// <summary>
    /// Converts a value to its dictionary corresponding object.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return null;
      }
      return Dictionary.GetValueOrDefault(value);
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
}

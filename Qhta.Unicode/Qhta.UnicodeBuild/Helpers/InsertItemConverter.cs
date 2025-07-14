using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;


namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// This converter is used to insert an item into a collection.
/// Inserted item is passed as a parameter to the converter
/// and is inserted at the start of the collection.
/// It is used for adding a new item (usually a null item) to a collection of selectable objects.
/// </summary>
public class InsertItemConverter: IValueConverter
{
  /// <summary>
  /// Converts a value by inserting an item at the start of a collection.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is IList list)
    {
      list.Insert(0, parameter);
      //Debug.WriteLine("Item inserted at the start of the collection.");
    }
    return value;
  }

  /// <summary>
  /// Unimplemented method for converting back from the target type to the source type.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  /// <exception cref="NotSupportedException"></exception>
  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotSupportedException("ConvertBack is not supported for InsertItemConverter.");
  }
}
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;


namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// THis converter is used to insert an item into a collection.
/// Inserted item is passed as a parameter to the converter
/// and is inserted at the start of the collection.
/// </summary>
public class InsertItemConverter: IValueConverter
{
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is IList list)
    {
      list.Insert(0, parameter);
      //Debug.WriteLine("Item inserted at the start of the collection.");
    }
    return value;
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotSupportedException("ConvertBack is not supported for InsertItemConverter.");
  }
}
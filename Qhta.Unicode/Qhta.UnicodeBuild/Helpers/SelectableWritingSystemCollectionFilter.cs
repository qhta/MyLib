using System.Globalization;
using System.Windows.Data;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// This class is used to filter a collection of writing systems based on other field.
/// THe parameter should be the value of other CodePointViewModel property.
/// </summary>
public class SelectableWritingSystemCollectionFilter: IValueConverter
{
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is IEnumerable<WritingSystemViewModel?> collection)
    {
      var writingSystemType = collection.FirstOrDefault()?.Type;
      if (writingSystemType == null || parameter is WritingSystemType otherType)
      {

      }
      return collection;
    }
    throw new InvalidCastException("Value must be IEnumerable<WritingSystemViewModel?>");
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
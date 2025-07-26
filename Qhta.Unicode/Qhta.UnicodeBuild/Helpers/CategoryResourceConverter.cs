using System.Globalization;
using System.Windows.Data;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// Value converter that retrieves a localized resource of category name.
/// </summary>
public class CategoryResourceConverter: IValueConverter
{
  /// <summary>
  /// Converts a category name to its corresponding resource string.
  /// </summary>
  /// <param name="value">The category name as a string. Must be a non-null string to perform the conversion.</param>
  /// <param name="targetType">The type of the binding target property. This parameter is not used in the conversion.</param>
  /// <param name="parameter">An optional parameter to be used in the converter logic. This parameter is not used in the conversion.</param>
  /// <param name="culture">The culture to be used in the converter. This parameter is not used in the conversion.</param>
  /// <returns>The resource object corresponding to the category name, or <see langword="null"/> if the category name is not a
  /// string or the resource is not found.</returns>
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is not string categoryName)
      return null;
    return Resources.UcdCategoryStrings.ResourceManager.GetObject(categoryName) ?? null;
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
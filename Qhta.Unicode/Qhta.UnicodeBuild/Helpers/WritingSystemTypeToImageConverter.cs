using Qhta.Unicode.Models;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// Value converter that converts a <see cref="WritingSystemType"/> to a corresponding image URI.
/// </summary>
public class WritingSystemTypeToImageConverter : IValueConverter
{
  /// <summary>
  /// Converts a <see cref="WritingSystemType"/> to a URI string representing an image.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is WritingSystemType type)
    {
      return type switch
      {
        WritingSystemType.SymbolSet => "pack://application:,,,/Assets/SymbolSet.png",
        WritingSystemType.Area => "pack://application:,,,/Assets/Area.png",
        WritingSystemType.Subset => "pack://application:,,,/Assets/Subset.png",
        WritingSystemType.Language => "pack://application:,,,/Assets/Language.png",
        WritingSystemType.Script => "pack://application:,,,/Assets/Script.png",
        WritingSystemType.Notation => "pack://application:,,,/Assets/Notation.png",
        WritingSystemType.Group => "pack://application:,,,/Assets/Family.png",
        _ => "pack://application:,,,/Assets/_Empty.png"
      };
    }
    return "pack://application:,,,/Assets/_Empty.png";
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
  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
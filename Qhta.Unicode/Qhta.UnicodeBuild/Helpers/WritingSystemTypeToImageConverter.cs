using Qhta.Unicode.Models;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.UnicodeBuild.Helpers;

public class WritingSystemTypeToImageConverter : IValueConverter
{
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
        WritingSystemType.Family => "pack://application:,,,/Assets/Family.png",
        _ => "pack://application:,,,/Assets/_Empty.png"
      };
    }
    return "pack://application:,,,/Assets/_Empty.png";
  }

  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
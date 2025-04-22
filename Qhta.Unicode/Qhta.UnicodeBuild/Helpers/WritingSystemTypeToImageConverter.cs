using Qhta.Unicode.Models;
using System.Globalization;
using System.Windows.Data;

namespace Qhta.UnicodeBuild.Helpers;

public class WritingSystemTypeToImageConverter : IValueConverter
{
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is WritingSystemTypeEnum type)
    {
      return type switch
      {
        WritingSystemTypeEnum.Set => "pack://application:,,,/Assets/Set.png",
        WritingSystemTypeEnum.Area => "pack://application:,,,/Assets/Area.png",
        WritingSystemTypeEnum.Subset => "pack://application:,,,/Assets/Subset.png",
        WritingSystemTypeEnum.Script => "pack://application:,,,/Assets/Script.png",
        WritingSystemTypeEnum.Notation => "pack://application:,,,/Assets/Notation.png",
        WritingSystemTypeEnum.Family => "pack://application:,,,/Assets/Family.png",
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
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
        WritingSystemType.set => "pack://application:,,,/Assets/Set.png",
        WritingSystemType.area => "pack://application:,,,/Assets/Area.png",
        WritingSystemType.subset => "pack://application:,,,/Assets/Subset.png",
        WritingSystemType.script => "pack://application:,,,/Assets/Script.png",
        WritingSystemType.notation => "pack://application:,,,/Assets/Notation.png",
        WritingSystemType.family => "pack://application:,,,/Assets/Family.png",
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
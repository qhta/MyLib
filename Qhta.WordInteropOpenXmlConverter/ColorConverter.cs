using Word = Microsoft.Office.Interop.Word;

namespace Qhta.WordInteropOpenXmlConverter;

public static class ColorConverter
{
  public static string WordColorToHex(Word.WdColor color)
  {
    if (color == Word.WdColor.wdColorAutomatic)
    {
      return ""; // empty string means "automatic" in this case
    }
    // Assuming color is in ARGB format, extract the RGB components
    var colorValue = (int)color;
    int red = (colorValue >> 0) & 0xFF;
    int green = (colorValue >> 8) & 0xFF;
    int blue = (colorValue >> 16) & 0xFF;

    // Convert RGB components to a hex string (ignoring the alpha component)
    string hexColor = $"{red:X2}{green:X2}{blue:X2}";
    return hexColor;
  }

  public static string WordColorIndexToHex(Word.WdColorIndex colorIndex)
  {
    switch (colorIndex)
    {
      case Word.WdColorIndex.wdByAuthor:
        return ""; // empty string means "automatic" in this case
      case Word.WdColorIndex.wdAuto:
        return ""; // empty string means "automatic" in this case
      case Word.WdColorIndex.wdBlack:
        return "000000";
      case Word.WdColorIndex.wdBlue:
        return "0000FF";
      case Word.WdColorIndex.wdTurquoise:
        return "00FFFF";
      case Word.WdColorIndex.wdBrightGreen:
        return "00FF00";
      case Word.WdColorIndex.wdPink:
        return "FF00FF";
      case Word.WdColorIndex.wdRed:
        return "FF0000";
      case Word.WdColorIndex.wdYellow:
        // ReSharper disable once StringLiteralTypo
        return "FFFF00";
      case Word.WdColorIndex.wdWhite:
        // ReSharper disable once StringLiteralTypo
        return "FFFFFF";
      case Word.WdColorIndex.wdDarkBlue:
        return "0000A0";
      case Word.WdColorIndex.wdTeal:
        return "008080";
      case Word.WdColorIndex.wdGreen:
        return "00FF00";
      case Word.WdColorIndex.wdViolet:
        return "800080";
      case Word.WdColorIndex.wdDarkRed:
        return "800000";
      case Word.WdColorIndex.wdDarkYellow:
        return "808000";
      case Word.WdColorIndex.wdGray50:
        return "808080";
      case Word.WdColorIndex.wdGray25:
        return "C0C0C0";
      default:
        return "000000"; // Default to black if unspecified
    }
  }
}

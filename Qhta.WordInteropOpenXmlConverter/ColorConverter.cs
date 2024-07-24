using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using DocumentFormat.OpenXml;

using static Microsoft.Office.Interop.Word.WdThemeColorIndex;
using static Qhta.WordInteropOpenXmlConverter.NumberConverter;

using W = DocumentFormat.OpenXml.Wordprocessing;
using Word = Microsoft.Office.Interop.Word;

namespace Qhta.WordInteropOpenXmlConverter;

public static class ColorConverter
{
  public static Dictionary<Word.WdThemeColorIndex, string> ThemeColorIndexToHex = new()
  {
    {wdThemeColorMainDark1, "000000"},
    {wdThemeColorMainLight1, "FFFFFF"},
    {wdThemeColorMainDark2, "0E2841"},
    {wdThemeColorMainLight2, "E8E8E8"},
    {wdThemeColorAccent1, "156082"},
    {wdThemeColorAccent2, "E97132"},
    {wdThemeColorAccent3, "196B24"},
    {wdThemeColorAccent4, "0F9ED5"},
    {wdThemeColorAccent5, "A02B93"},
    {wdThemeColorAccent6, "4EA72E"},
    {wdThemeColorHyperlink, "467886"},
    {wdThemeColorHyperlinkFollowed, "800080"},
    {wdThemeColorBackground1, "F2F2F2"},
    {wdThemeColorText1, "000000"},
    {wdThemeColorBackground2, "D9D9D9"},
    {wdThemeColorText2, "000000"}
  };


  public static W.Color? ConvertColor(Word.WdColor color, Word.WdColorIndex colorIndex, Word.ColorFormat? colorFormat = null)
  {
    var xColor = new W.Color();
    var addColor = false;
    if ((int)color != wdUndefined && color != Word.WdColor.wdColorAutomatic)
    {
      xColor.Val = WordColorToHex(color);
      addColor = true;
    }
    else
    if ((int)colorIndex != wdUndefined && colorIndex != Word.WdColorIndex.wdAuto)
    {
      xColor.Val = WordColorIndexToHex(colorIndex);
      addColor = true;
    }
    if (colorFormat != null)
    {
      try
      {
        var themeColor = colorFormat.ObjectThemeColor;
        if (themeColor != wdNotThemeColor)
        {
          xColor.ThemeColor = new EnumValue<W.ThemeColorValues>(WdThemeColorToOpenXmlThemeColor(themeColor));
          addColor = true;
          var tintAndShade = colorFormat.TintAndShade;
          if (tintAndShade < 0)
          {
            var shadeValue = GetShadeValue(tintAndShade);
            xColor.ThemeShade = shadeValue.ToString("X2");
            xColor.Val = ApplyShade(ThemeColorIndexToHex[themeColor], shadeValue);
            addColor = true;
          }
          else if (tintAndShade > 0)
          {
            var tintValue = GetTintValue(tintAndShade);
            xColor.ThemeTint = tintValue.ToString("X2");
            xColor.Val = ApplyTint(ThemeColorIndexToHex[themeColor], tintValue);
            addColor = true;
          }
        }
      } catch (COMException) { }
    }

    if (addColor)
      return xColor;
    return null;
  }

  private static string WordColorToHex(Word.WdColor color)
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

  private static Word.WdColor HexToWordColor(string hexColor)
  {

    int red = hexColor.Length > 2 ? Convert.ToInt32(hexColor.Substring(0, 2), 16) : 0;
    int green = hexColor.Length > 4 ? Convert.ToInt32(hexColor.Substring(2, 2), 16) : 0;
    int blue = hexColor.Length > 6 ? Convert.ToInt32(hexColor.Substring(4, 2), 16) : 0;
    var colorValue = (red & 0xFF) | ((green & 0xFF) << 8) | ((blue & 0xFF) << 16);
    return (Word.WdColor)Enum.ToObject(typeof(Word.WdColor), colorValue);
  }

  private static string WordColorIndexToHex(Word.WdColorIndex colorIndex)
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

  private static W.ThemeColorValues WdThemeColorToOpenXmlThemeColor(Word.WdThemeColorIndex themeColor)
  {
    return themeColor switch
    {
      wdNotThemeColor => W.ThemeColorValues.None,
      wdThemeColorMainDark1 => W.ThemeColorValues.Dark1,
      wdThemeColorMainLight1 => W.ThemeColorValues.Light1,
      wdThemeColorMainDark2 => W.ThemeColorValues.Dark2,
      wdThemeColorMainLight2 => W.ThemeColorValues.Light2,
      wdThemeColorAccent1 => W.ThemeColorValues.Accent1,
      wdThemeColorAccent2 => W.ThemeColorValues.Accent2,
      wdThemeColorAccent3 => W.ThemeColorValues.Accent3,
      wdThemeColorAccent4 => W.ThemeColorValues.Accent4,
      wdThemeColorAccent5 => W.ThemeColorValues.Accent5,
      wdThemeColorAccent6 => W.ThemeColorValues.Accent6,
      wdThemeColorHyperlink => W.ThemeColorValues.Hyperlink,
      wdThemeColorHyperlinkFollowed => W.ThemeColorValues.FollowedHyperlink,
      wdThemeColorBackground1 => W.ThemeColorValues.Background1,
      wdThemeColorText1 => W.ThemeColorValues.Text1,
      wdThemeColorBackground2 => W.ThemeColorValues.Background2,
      wdThemeColorText2 => W.ThemeColorValues.Text2,
      _ => W.ThemeColorValues.None
    };
  }

  private static int GetShadeValue(float tintAndShade)
  {
    // Normalize the TintAndShade value to a 0-1 range
    float normalizedValue = (tintAndShade + 1);

    // Convert to a shade value (we'll treat the entire range as shading for simplicity)
    // This is an approximation and may need adjustment for your specific needs
    int shadeValue = (int)((normalizedValue) * 255);

    // Ensure the value is within the byte range
    shadeValue = Math.Max(0, Math.Min(shadeValue, 255));
    return shadeValue;
  }

  private static int GetTintValue(float tintAndShade)
  {
    // Normalize the TintAndShade value to a 0-1 range
    float normalizedValue = (tintAndShade);

    // Convert to a shade value (we'll treat the entire range as shading for simplicity)
    // This is an approximation and may need adjustment for your specific needs
    int tintValue = (int)((normalizedValue) * 255);

    // Ensure the value is within the byte range
    tintValue = Math.Max(0, Math.Min(tintValue, 255));
    return tintValue;
  }

  public static string ApplyShade(string hexColor, int shadeValue)
  {
    double factor = shadeValue / 255.0;
    var RGB = HexToRGB(hexColor);
    RGB.red = (int)(RGB.red * factor);
    RGB.green = (int)(RGB.green * factor);
    RGB.blue = (int)(RGB.blue * factor);
    return RGBToHex(RGB.red, RGB.green, RGB.blue);
  }

  public static string ApplyTint(string hexColor, int tintValue)
  {
    double factor = tintValue / 255.0;
    var RGB = HexToRGB(hexColor);
    RGB.red = (int)(RGB.red + (255 - RGB.red) * factor);
    RGB.green = (int)(RGB.green + (255 - RGB.green) * factor);
    RGB.blue = (int)(RGB.blue + (255 - RGB.blue) * factor);
    return RGBToHex(RGB.red, RGB.green, RGB.blue);
  }

  public static string RGBToHex(int red, int green, int blue)
  {
    return $"{red:X2}{green:X2}{blue:X2}";
  }

  public static (int red, int green, int blue) HexToRGB(string hexColor)
  {
    // Remove any leading # characters
    hexColor = hexColor.TrimStart('#');

    // Parse the hex color string into RGB components
    int red = Convert.ToInt32(hexColor.Substring(0, 2), 16);
    int green = Convert.ToInt32(hexColor.Substring(2, 2), 16);
    int blue = Convert.ToInt32(hexColor.Substring(4, 2), 16);

    return (red, green, blue);
  }

  public static (double hue, double lightness, double saturation) RGBToHLS(int red, int green, int blue)
  {
    // Convert RGB to a range of 0 to 1
    double r = red / 255.0;
    double g = green / 255.0;
    double b = blue / 255.0;

    double max = Math.Max(r, Math.Max(g, b));
    double min = Math.Min(r, Math.Min(g, b));

    double hue;
    double saturation;
    // Calculate lightness
    var lightness = (max + min) / 2.0;

    double delta = max - min;

    // Calculate saturation
    if (delta == 0) // It's a gray, no chroma...
    {
      // When there's no color, hue is undefined (set to zero), and saturation is zero
      hue = 0;
      saturation = 0;
    }
    else
    {
      // Saturation is more when lightness is less
      saturation = lightness > 0.5 ? delta / (2.0 - max - min) : delta / (max + min);

      // Calculate hue
      if (r == max)
      {
        hue = (g - b) / delta + (g < b ? 6 : 0);
      }
      else if (g == max)
      {
        hue = (b - r) / delta + 2;
      }
      else
      {
        hue = (r - g) / delta + 4;
      }

      hue /= 6.0; // To turn it into a fraction of 360
    }
    return (hue, lightness, saturation);
  }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace System.Drawing
{
  public static class KnownColors
  {
    private static Dictionary<uint, SolidBrush> s_solidBrushCache = new Dictionary<uint, SolidBrush>();
    private static Dictionary<string, KnownColor> s_knownArgbColors = new Dictionary<string, KnownColor>();

    static KnownColors()
    {
      foreach (KnownColor color in Enum.GetValues(typeof(KnownColor)))
      {
        string str = string.Format("#{0,8:X8}", (uint)color);
        s_knownArgbColors[str] = color;
      }
    }

    internal static KnownColor ArgbStringToKnownColor(string argbString)
    {
      KnownColor color;
      string key = argbString.Trim().ToUpper(CultureInfo.InvariantCulture);
      return (!s_knownArgbColors.TryGetValue(key, out color) ? KnownColor.UnknownColor : color);
    }

    public static SolidBrush ColorStringToKnownBrush(string s)
    {
      if (s != null)
      {
        KnownColor color = ColorStringToKnownColor(s);
        if (color != KnownColor.UnknownColor)
        {
          return SolidBrushFromUint((uint)color);
        }
      }
      return null;
    }

    internal static KnownColor ColorStringToKnownColor(string colorString)
    {
      return (KnownColor)Enum.Parse(typeof(KnownColor), colorString);
    }

    public static bool IsKnownSolidBrush(SolidBrush scp)
    {
      Dictionary<uint, SolidBrush> dictionary = s_solidBrushCache;
      lock (dictionary)
      {
        return s_solidBrushCache.ContainsValue(scp);
      }
    }

    internal static string MatchColor(string colorString, out bool isKnownColor, out bool isNumericColor, out bool isContextColor, out bool isScRgbColor)
    {
      string str = colorString.Trim();
      if ((((str.Length == 4) || ((str.Length == 5) || (str.Length == 7))) || (str.Length == 9)) && (str[0] == '#'))
      {
        isNumericColor = true;
        isScRgbColor = false;
        isKnownColor = false;
        isContextColor = false;
        return str;
      }
      isNumericColor = false;
      if (!str.StartsWith("sc#", StringComparison.Ordinal))
      {
        isScRgbColor = false;
      }
      else
      {
        isNumericColor = false;
        isScRgbColor = true;
        isKnownColor = false;
        isContextColor = false;
      }
      if (!str.StartsWith("ContextColor ", StringComparison.OrdinalIgnoreCase))
      {
        isContextColor = false;
        isKnownColor = true;
        return str;
      }
      isContextColor = true;
      isScRgbColor = false;
      isKnownColor = false;
      return str;
    }

    public static SolidBrush SolidBrushFromUint(uint argb)
    {
      SolidBrush brush = null;
      Dictionary<uint, SolidBrush> dictionary = s_solidBrushCache;
      lock (dictionary)
      {
        if (!s_solidBrushCache.TryGetValue(argb, out brush))
        {
          brush = new SolidBrush(Color.FromArgb((int)argb));
          //brush.Freeze();
          s_solidBrushCache[argb] = brush;
        }
      }
      return brush;
    }

  }
}

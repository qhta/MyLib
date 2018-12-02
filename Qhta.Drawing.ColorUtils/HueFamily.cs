using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.Drawing
{
  public enum HueFamily
  {
    /// <summary>
    /// Red family, hue degree in range (-5..25)
    /// </summary>
    Red,
    /// <summary>
    /// Orange family, hue degree in range (25..60)
    /// </summary>
    Orange,
    /// <summary>
    /// Yellow family, hue degree in range (60..80)
    /// </summary>
    Yellow,
    /// <summary>
    /// Green family, hue degree in range (80..175)
    /// </summary>
    Green,
    /// <summary>
    /// Cyan family, hue degree in range (175..210)
    /// </summary>
    Cyan,
    /// <summary>
    /// Blue family, hue degree in range (210..280)
    /// </summary>
    Blue,
    /// <summary>
    /// Violet family, hue degree in range (280..310)
    /// </summary>
    Violet,
    /// <summary>
    /// Violet family, hue degree in range (310..355)
    /// </summary>
    Magenta,
  }

  public static class HueRange
  {
    public static HueFamily ToHueFamily(this int hue)
    {
      if (hue>-5 && hue<=25)
        return HueFamily.Red;
      if (hue>25 && hue<=60)
        return HueFamily.Orange;
      if (hue>60 && hue<=80)
        return HueFamily.Yellow;
      if (hue>80 && hue<=175)
        return HueFamily.Green;
      if (hue>175 && hue<=210)
        return HueFamily.Cyan;
      if (hue>210 && hue<=280)
        return HueFamily.Blue;
      if (hue>280 && hue<=310)
        return HueFamily.Violet;
      if (hue>310 && hue<=355)
        return HueFamily.Magenta;
      return HueFamily.Red;
    }

    public static HueFamily ToHueFamily(this double h)
    {
      return ToHueFamily((int)(h*360));
    }

  }
}

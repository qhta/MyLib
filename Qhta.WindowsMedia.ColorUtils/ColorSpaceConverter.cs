using System;
using System.Windows.Media;

namespace Qhta.WindowsMedia.ColorUtils
{
  public struct ColorRGB
  {
    public double R; // 0..360
    public double G; // 0..1
    public double B; // 0..1

    public ColorRGB(byte R, byte G, byte B)
    {
      this.R = R/255.0;
      this.G = G/255.0;
      this.B = B/255.0;
    }
  }

  public struct ColorHSV
  {
    public double H; // 0..360
    public double S; // 0..1
    public double V; // 0..1

    public ColorHSV(double H, double S, double V)
    {
      this.H = H % 360;
      this.S = S;
      this.V = V;
    }

  }

  public struct ColorHLS
  {
    public double H; // 0..360
    public double L; // 0..1
    public double S; // 0..1

    public ColorHLS(double H, double L, double S)
    {
      this.H = H % 360;
      this.L = L;
      this.S = S;
    }
  }

  public static class ColorSpaceConverter
  {

    public static ColorHSV RGB2HSV(this Color value)
    {
      double r = value.ScR;
      double g = value.ScG;
      double b = value.ScB;
      double max = Math.Max(Math.Max(r, g), b);
      double min = Math.Min(Math.Min(r, g), b);
      double h = 0;
      double s;
      double v;

      if (min == max)
        h = 0;
      else
      {
        if (r==max)
          h = 0 + ((g-b)*60 / (max-min));
        if (g==max)
          h = 120 + ((b-r)*60 / (max-min));
        if (b==max)
          h = 240 + ((r-g)*60 / (max-min));
      }
     
      if (h<0)
        h = h+360;
     
      if (max==0)
        s = 0;
      else
        s = (max-min) / max;
     
      v = max;

      return new ColorHSV(h, s, v);
    }

    public static ColorHLS RGB2HLS(this Color value)
    {
      double r = value.ScR;
      double g = value.ScG;
      double b = value.ScB;
      double max = Math.Max(Math.Max(r, g), b);
      double min = Math.Min(Math.Min(r, g), b);
      double h = 0;
      double l;
      double s;

      if (min == max)
        h = 0;
      else
      {
        if (r == max)
          h = 0 + ((g - b) * 60 / (max - min));
        if (g == max)
          h = 120 + ((b - r) * 60 / (max - min));
        if (b == max)
          h = 240 + ((r - g) * 60 / (max - min));
      }

      if (h < 0)
        h = h + 360;

      l = (max + min) / 2;

      if (max == 0)
        s = 0;
      else if (l < 0.5)
        s = (max - min) / (2 * l);
      else
        s = (max - min) / (2 - 2 * l);


      return new ColorHLS(h, l, s);
    }

    public static Color HSV2RGB(ColorHSV value)
    {
      double v = value.V;
      double s = value.S;
      double h = value.H;
      double p, q, t, f;
      long i;
      double r;
      double g;
      double b;

      if (h >= 360.0) h = 0.0;
      h /= 60.0;
      i = (long)h;
      f = h - i;
      p = v * (1.0 - s);
      q = v * (1.0 - (s* f));
      t = v * (1.0 - (s * (1.0 - f)));

      switch (i)
      {
        case 0:
          r = v;
          g = t;
          b = p;
          break;
        case 1:
          r = q;
          g = v;
          b = p;
          break;
        case 2:
          r = p;
          g = v;
          b = t;
          break;
        case 3:
          r = p;
          g = q;
          b = v;
          break;
        case 4:
          r = t;
          g = p;
          b = v;
          break;
        case 5:
        default:
          r = v;
          g = p;
          b = q;
          break;
      }
      return Color.FromScRgb((float)1.0, (float)r, (float)g, (float)b);
    }
    public static Color Inverse(this Color value)
    {
      var aColor = value.RGB2HSV();
      aColor.V = 1-aColor.V;
      aColor.H = (aColor.H+180) % 360;
      var result = HSV2RGB(aColor);
      return result;
    }
  }
}

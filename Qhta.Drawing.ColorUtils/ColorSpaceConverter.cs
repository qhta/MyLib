using System;
using System.Drawing;

namespace Qhta.Drawing
{
  public struct ColorRGB
  {
    public double R; // 0..1
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
    public double H; // 0..1 = full angle
    public double S; // 0..1
    public double V; // 0..1

    public ColorHSV(double H, double S, double V)
    {
      this.H = H;
      this.S = S;
      this.V = V;
    }
  }

  public struct ColorHLS
  {
    public double H; // 0..1 = full angle
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

    public static ColorHSV Color2HSV(this Color value)
    {
      return RGB2HSV(value.R/255.0, value.G/255.0, value.B/255.0);
    }

    public static ColorHSV RGB2HSV(this ColorRGB value)
    {
      return RGB2HSV(value.R, value.G, value.B);
    }

    public static ColorHSV RGB2HSV(double R, double G, double B)
    {
      double r = R;
      double g = G;
      double b = B;
      double max = Math.Max(Math.Max(r, g), b);
      double min = Math.Min(Math.Min(r, g), b);
      double h = 0;
      double s;
      double v;

      if (min == max)
        h = 0;
      else
      {
        double delta = max-min;
        if (r == max)
          h = (g - b) / delta;        // between yellow & magenta
        else
       if (g == max)
          h = 2.0 + (b - r) / delta;  // between cyan & yellow
        else
          h = 4.0 + (r - g) / delta;  // between magenta & cyan
        h /= 6;
      }

      if (h<0)
        h = h+1.0;

      if (max==0)
        s = 0;
      else
        s = (max-min) / max;

      v = max;

      return new ColorHSV(h, s, v);
    }

    public static ColorHLS Color2HLS(this Color value)
    {
      return RGB2HLS(value.R/255.0, value.G/255.0, value.B/255.0);
    }

    public static ColorHLS RGB2HLS(this ColorRGB value)
    {
      return RGB2HLS(value.R, value.G, value.B);
    }

    public static ColorHLS RGB2HLS(double R, double G, double B)
    {
      double r = R;
      double g = G;
      double b = B;
      double max = Math.Max(Math.Max(r, g), b);
      double min = Math.Min(Math.Min(r, g), b);
      double h = 0;
      double l;
      double s;

      if (min == max)
        h = 0;
      else
      {
        double delta = max-min;
        if (r == max)
          h = (g - b) / delta;        // between yellow & magenta
        else
       if (g == max)
          h = 2.0 + (b - r) / delta;  // between cyan & yellow
        else
          h = 4.0 + (r - g) / delta;  // between magenta & cyan
        h /= 6;
      }

      if (h < 0)
        h = h + 1.0;

      l = (max + min) / 2;

      if (max == 0)
        s = 0;
      else if (l < 0.5)
        s = (max - min) / (2 * l);
      else
        s = (max - min) / (2 - 2 * l);


      return new ColorHLS(h, l, s);
    }

    public static Color HSV2Color(ColorHSV value)
    {
      var c = HSV2RGB(value);
      return Color.FromArgb((int)(c.R*255), (int)(c.G*255), (int)(c.B*255));
    }

    public static ColorRGB HSV2RGB(ColorHSV value)
    {
      double v = value.V;
      double s = value.S;
      double h = value.H;
      double p, q, t, f;
      int i;
      double r;
      double g;
      double b;

      if (h >= 1.0) h = 0.0;
      i = (int)(h*6);
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
      return new ColorRGB { R = r, G = g, B = b };
    }

    public static Color HLS2Color(ColorHLS value)
    {
      var c = HLS2RGB(value);
      return Color.FromArgb((int)(c.R*255), (int)(c.G*255), (int)(c.B*255));
    }

    public static ColorRGB HLS2RGB(this ColorHLS value)
    {
      double r, g, b;
      double h = value.H;
      double s = value.S;
      double l = value.L;

      if (s == 0)
      {
        r = g = b = l; // achromatic
      }
      else
      {
        double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
        double p = 2 * l - q;
        r = hue2rgb(p, q, h + 1/3);
        g = hue2rgb(p, q, h);
        b = hue2rgb(p, q, h - 1/3);
      }
      return new ColorRGB { R = r, G = g, B = b };
    }

    private static double hue2rgb(double p, double q, double h)
    {
      if (h < 0) h += 1;
      if (h > 1) h -= 1;
      if (h < 1.0/6) return p + (q - p) * 6 * h;
      if (h < 1.0/2) return q;
      if (h < 2.0/3) return p + (q - p) * (2.0/3 - h) * 6;
      return p;
    }

    public static Color Inverse(this Color value)
    {
      var aColor = value.Color2HSV();
      aColor.V = 1-aColor.V;
      aColor.H = (aColor.H+0.5) % 1.0;
      var result = HSV2Color(aColor);
      return result;
    }
  }

}

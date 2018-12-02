using System;
using System.Drawing;

namespace Qhta.Drawing
{
  public struct ArgbColor
  {
    /// <summary>
    /// Alpha channel in range 0..1
    /// </summary>
    public double A; 
    /// <summary>
    /// Red channel in range 0..1
    /// </summary>
    public double R; 
    /// <summary>
    /// Green channel in range 0..1
    /// </summary>
    public double G; 
    /// <summary>
    /// Blue channel in range 0..1
    /// </summary>
    public double B; 

    public ArgbColor(byte R, byte G, byte B)
    {
      this.A = 1;
      this.R = R/255.0;
      this.G = G/255.0;
      this.B = B/255.0;
    }

    public ArgbColor(byte A, byte R, byte G, byte B)
    {
      this.A = A;
      this.R = R/255.0;
      this.G = G/255.0;
      this.B = B/255.0;
    }

    public ArgbColor(double R, double G, double B)
    {
      this.A = 1;
      this.R = R;
      this.G = G;
      this.B = B;
    }

    public ArgbColor(double A, double R, double G, double B)
    {
      this.A = A;
      this.R = R;
      this.G = G;
      this.B = B;
    }
  }

  public struct AhsvColor
  {
    /// <summary>
    ///  Alpha in range 0..1 (full opacity)
    /// </summary>
    public double A;
    /// <summary>
    ///  Hue in range 0..1 (full angle)
    /// </summary>
    public double H;
    /// <summary>
    ///  Saturation in range 0..1
    /// </summary>
    public double S;
    /// <summary>
    ///  Value in range 0..1
    /// </summary>
    public double V;

    public int Alpha { get => (int)(A*255); set => A = value/255.0; }
    public int Hue { get => (int)(H*360); set => H = value/360.0; }
    public int Saturation { get => (int)(S*255); set => S = value/255.0; }
    public int Value { get => (int)(V*255); set => V = value/255.0; }

    public AhsvColor(double H, double S, double V)
    {
      this.A = 1;
      this.H = H;
      this.S = S;
      this.V = V;
    }
    public AhsvColor(double A, double H, double S, double V)
    {
      this.A = A;
      this.H = H;
      this.S = S;
      this.V = V;
    }
  }

  public struct AhlsColor
  {
    /// <summary>
    ///  Alpha in range 0..1 (full opacity)
    /// </summary>
    public double A;
    /// <summary>
    ///  Hue in range 0..1 (full angle)
    /// </summary>
    public double H;
    /// <summary>
    ///  Lightness in range 0..1
    /// </summary>
    public double L;
    /// <summary>
    ///  Saturation in range 0..1
    /// </summary>
    public double S;

    public int Alpha { get => (int)(A*255); set => A = value/255.0; }
    public int Hue { get => (int)(H*360); set => H = value/360.0; }
    public int Saturation { get => (int)(S*255); set => S = value/255.0; }
    public int Lightness { get => (int)(L*255); set => L = value/255.0; }


    public AhlsColor(double H, double L, double S)
    {
      this.A = 1;
      this.H = H;
      this.L = L;
      this.S = S;
    }
    public AhlsColor(double A, double H, double L, double S)
    {
      this.A = A;
      this.H = H;
      this.L = L;
      this.S = S;
    }
  }

  public static class ColorSpaceConverter
  {

    public static ArgbColor ToArgb(this Color value)
    {
      return new ArgbColor(value.A/255.0, value.R/255.0, value.G/255.0, value.B/255.0);
    }

    public static AhsvColor ToAhsv(this Color value)
    {
      return ToAhsv(value.A/255.0, value.R/255.0, value.G/255.0, value.B/255.0);
    }

    public static AhsvColor ToAhsv(this ArgbColor value)
    {
      return ToAhsv(value.A, value.R, value.G, value.B);
    }


    public static AhsvColor ToAhsv(double A, double R, double G, double B)
    {
      double a = A;
      double r = R;
      double g = G;
      double b = B;
      double max = Math.Max(Math.Max(r, g), b);
      double min = Math.Min(Math.Min(r, g), b);
      double h = 0;
      double s;
      double v;

      if (min == max)
        h = double.NaN;
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
      if (h==1.0)
        h = 0;

      if (max==0)
        s = 0;
      else
        s = (max-min) / max;

      v = max;

      return new AhsvColor(a, h, s, v);
    }

    public static AhlsColor ToAhls(this Color value)
    {
      return ToAhls(value.A/255.0, value.R/255.0, value.G/255.0, value.B/255.0);
    }

    public static AhlsColor ToAhls(this ArgbColor value)
    {
      return ToAhls(value.A, value.R, value.G, value.B);
    }

    public static AhlsColor ToAhls(double a, double r, double g, double b)
    {
      double max = Math.Max(Math.Max(r, g), b);
      double min = Math.Min(Math.Min(r, g), b);
      double h, s; 
      double l = (max + min) / 2;

      if (max == min)
      {
        h = double.NaN;
        s = 0; // achromatic
      }
      else
      {
        var d = max - min;
        s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
        if (max==r)
          h = (g - b) / d + (g < b ? 6 : 0);
        else 
        if (max==g)
          h = (b - r) / d + 2; 
        else //if (max==b)
          h = (r - g) / d + 4;
        h /= 6;
      }
      return new AhlsColor(a, h, l, s);
    }

    public static Color ToColor(this AhsvColor value)
    {
      var c = ToArgb(value);
      return Color.FromArgb((int)(c.A*255), (int)(c.R*255), (int)(c.G*255), (int)(c.B*255));
    }

    public static ArgbColor ToArgb(this AhsvColor value)
    {
      double a = value.A;
      double v = value.V;
      double s = value.S;
      double h = (Double.IsNaN(value.H)) ? 0 : value.H;
      double p, q, t, f;
      double r;
      double g;
      double b;

      double hh = h*360.0;
      if (hh >= 360.0) hh = 0.0;
      hh /= 60.0;
      var i = (int)hh;
      double ff = hh - i;
      p = v * (1.0 - s);
      q = v * (1.0 - (s* ff));
      t = v * (1.0 - (s * (1.0 - ff)));

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
      return new ArgbColor { A= a, R = r, G = g, B = b };
    }

    public static Color ToColor(this AhlsColor value)
    {
      var c = ToArgb(value);
      return Color.FromArgb((int)(c.A*255), (int)(c.R*255), (int)(c.G*255), (int)(c.B*255));
    }

    public static ArgbColor ToArgb(this AhlsColor value)
    {
      double r, g, b;
      double a = value.A;
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
        r = h2rgb(p, q, h + 1/3);
        g = h2rgb(p, q, h);
        b = h2rgb(p, q, h - 1/3);
      }
      return new ArgbColor { A= a, R = r, G = g, B = b };
    }

    private static double h2rgb(double p, double q, double h)
    {
      if (h < 0) h += 1;
      if (h > 1) h -= 1;
      if (h < 1.0/6) return p + (q - p) * 6 * h;
      if (h < 1.0/2) return q;
      if (h < 2.0/3) return p + (q - p) * (2.0/3 - h) * 6;
      return p;
    }

  }

}

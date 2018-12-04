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

  public struct AcmykColor
  {
    /// <summary>
    /// Alpha channel in range 0..1
    /// </summary>
    public double A;
    /// <summary>
    /// Cyan channel in range 0..1
    /// </summary>
    public double C;
    /// <summary>
    /// Magenta channel in range 0..1
    /// </summary>
    public double M;
    /// <summary>
    /// Yellow channel in range 0..1
    /// </summary>
    public double Y;
    /// <summary>
    /// Black channel in range 0..1. Is NaN in CMY model
    /// </summary>
    public double K;

    public AcmykColor(byte C, byte M, byte Y)
    {
      this.A = 1;
      this.C = C/255.0;
      this.M = M/255.0;
      this.Y = Y/255.0;
      this.K = double.NaN;
    }

    public AcmykColor(byte A, byte C, byte M, byte Y)
    {
      this.A = A;
      this.C = C/255.0;
      this.M = M/255.0;
      this.Y = Y/255.0;
      this.K = double.NaN;
    }

    public AcmykColor(byte A, byte C, byte M, byte Y, byte K)
    {
      this.A = A;
      this.C = C/255.0;
      this.M = M/255.0;
      this.Y = Y/255.0;
      this.K = K/255.0;
    }

    public AcmykColor(double C, double M, double Y, double K = double.NaN)
    {
      this.A = 1;
      this.C = C;
      this.M = M;
      this.Y = Y;
      this.K = K;
    }

    public AcmykColor(double A, double C, double M, double Y, double K=double.NaN)
    {
      this.A = A;
      this.C = C;
      this.M = M;
      this.Y = Y;
      this.K = K;
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

    public static AcmykColor ToAcmyk(this Color value)
    {
      return new AcmykColor(value.A/255.0, value.R/255.0, value.G/255.0, value.B/255.0);
    }

    public static AcmykColor ToAcmy(this Color value)
    {
      return new AcmykColor(value.A/255.0, value.R/255.0, value.G/255.0, value.B/255.0);
    }

    public static AhsvColor ToAhsv(this Color value)
    {
      return ToAhsv(value.A/255.0, value.R/255.0, value.G/255.0, value.B/255.0);
    }

    public static AhsvColor ToAhsv(this ArgbColor value)
    {
      return ToAhsv(value.A, value.R, value.G, value.B);
    }

    public static AcmykColor ToAmy(double alpha, double red, double green, double blue)
    {
      var cyan = 1 - red;
      var magenta = 1 - green;
      var yellow = 1 - blue;
      return new AcmykColor(alpha, cyan, magenta, yellow);
    }

    public static AcmykColor ToAmyk(double alpha, double red, double green, double blue)
    {
      var black = Math.Min(Math.Min(1 - red, 1 - green), 1 - blue);

      if (black==1)
      {
        var cyan = 1 - red;
        var magenta = 1 - green;
        var yellow = 1 - blue;
        return new AcmykColor(alpha, cyan, magenta, yellow, black);
      }
      else
      {
        var cyan = (1-red-black)/(1-black);
        var magenta = (1-green-black)/(1-black);
        var yellow = (1-blue-black)/(1-black);
        return new AcmykColor(alpha, cyan, magenta, yellow, black);
      }
    }

    public static ArgbColor ToArgb(this AcmykColor value)
    {
      if (double.IsNaN(value.K) || value.K==1)
      {
        var red = 1 - value.C;
        var green = 1 - value.M;
        var blue = 1 - value.Y;
        return new ArgbColor(value.A, red, green, blue);
      }
      else
      {
        var red = (1-value.C) * (1-value.K);
        var green = (1-value.M) * (1-value.K);
        var blue = (1-value.Y) * (1-value.K);
        return new ArgbColor(value.A, red, green, blue);
      }
    }

    public static AhsvColor ToAhsv(double alpha, double red, double green, double blue)
    {
      double max = Math.Max(Math.Max(red, green), blue);
      double min = Math.Min(Math.Min(red, green), blue);
      double hue = 0;
      double saturation;
      double value;

      if (min == max)
        hue = double.NaN;
      else
      {
        double delta = max-min;
        if (red == max)
          hue = (green - blue) / delta;        // between yellow & magenta
        else
       if (green == max)
          hue = 2.0 + (blue - red) / delta;  // between cyan & yellow
        else
          hue = 4.0 + (red - green) / delta;  // between magenta & cyan
        hue /= 6;
      }

      if (hue<0)
        hue = hue+1.0;
      if (hue==1.0)
        hue = 0;

      if (max==0)
        saturation = 0;
      else
        saturation = (max-min) / max;

      value = max;

      return new AhsvColor(alpha, hue, saturation, value);
    }

    public static AhlsColor ToAhls(this Color value)
    {
      return ToAhls(value.A/255.0, value.R/255.0, value.G/255.0, value.B/255.0);
    }

    public static AhlsColor ToAhls(this ArgbColor value)
    {
      return ToAhls(value.A, value.R, value.G, value.B);
    }

    public static AhlsColor ToAhls(double alpha, double red, double green, double blue)
    {
      double max = Math.Max(Math.Max(red, green), blue);
      double min = Math.Min(Math.Min(red, green), blue);
      double hue, saturation; 
      double lightness = (max + min) / 2;

      if (max == min)
      {
        hue = double.NaN;
        saturation = 0; // achromatic
      }
      else
      {
        var delta = max - min;
        saturation = lightness > 0.5 ? delta / (2 - max - min) : delta / (max + min);
        if (max==red)
          hue = (green - blue) / delta + (green < blue ? 6 : 0);
        else 
        if (max==green)
          hue = (blue - red) / delta + 2; 
        else //if (max==b)
          hue = (red - green) / delta + 4;
        hue /= 6;
      }
      return new AhlsColor(alpha, hue, lightness, saturation);
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

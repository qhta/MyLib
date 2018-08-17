using System;
using System.Windows.Media;

namespace MyLib.ColorUtils
{
  public struct ColorHSV
  {
    public int H; // 0..359
    public byte S; // 0..100
    public byte V; // 0..100

    public ColorHSV(int H, byte S, byte V)
    {
      this.H = H % 360;
      this.S = S;
      this.V = V;
    }

  }

  public struct ColorHLS
  {
    public int H; // 0..359
    public byte L; // 0..100
    public byte S; // 0..100

    public ColorHLS(int H, byte L, byte S)
    {
      this.H = H % 360;
      this.L = L;
      this.S = S;
    }

  }

  public class ColorSpaceConverter
  {

    public static ColorHSV RGB2HSV(Color aColor)
    {
      int R = aColor.R;
      int G = aColor.G;
      int B = aColor.B;
      int Max = Math.Max(Math.Max(R, G), B);
      int Min = Math.Min(Math.Min(R, G), B);
      int H = 0;
      int S;
      int V;

      if (Min == Max)
        H = 0;
      else
      {
        if (R==Max)
          H = 0 + ((G-B)*60 / (Max-Min));
        if (G==Max)
          H = 120 + ((B-R)*60 / (Max-Min));
        if (B==Max)
          H = 240 + ((R-G)*60 / (Max-Min));
      }
     
      if (H<0)
        H = H+360;
     
      if (Max==0)
        S = 0;
      else
        S = (Max-Min)*100 / Max;
     
      V = (100*Max) / 255;

      return new ColorHSV(H, (byte)S, (byte)V);
    }

    public static ColorHLS RGB2HLS(Color aColor)
    {
      int R = aColor.R;
      int G = aColor.G;
      int B = aColor.B;
      int Max = Math.Max(Math.Max(R, G), B);
      int Min = Math.Min(Math.Min(R, G), B);
      int H = 0;
      int L;
      int S;

      if (Min == Max)
        H = 0;
      else
      {
        if (R == Max)
          H = 0 + ((G - B) * 60 / (Max - Min));
        if (G == Max)
          H = 120 + ((B - R) * 60 / (Max - Min));
        if (B == Max)
          H = 240 + ((R - G) * 60 / (Max - Min));
      }

      if (H < 0)
        H = H + 360;

      Max = Max * 100 / 255;
      Min = Min * 100 / 255;
      L = (Max + Min) / 2;

      if (Max == 0)
        S = 0;
      else if (L < 50)
        S = (Max - Min) / (2 * L);
      else
        S = (Max - Min) / (200 - 2 * L);


      return new ColorHLS(H, (byte)L, (byte)S);
    }
  }
}

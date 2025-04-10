﻿using System;
using System.Collections.Generic;
using System.Windows.Media;
using Qhta.Drawing;

namespace Qhta.WPF
{
  public class ColorComparer : IComparer<Color>
  {
    public int Compare(Color first, Color second)
    {
      return ColorComparison(first, second);
    }

    private int ColorComparison(Color first, Color second)
    {
      var hsv1 = first.ToDrawingColor().ToAhsv();
      var hsv2 = second.ToDrawingColor().ToAhsv();
      return CompareAlpha(hsv1, hsv2);
    }

    private int CompareAlpha(AhsvColor hsv1, AhsvColor hsv2)
    {
      if (hsv1.A<hsv2.A)
        return -1;
      else
      if (hsv1.A>hsv2.A)
        return +1;
      return CompareAchromatic(hsv1, hsv2);
    }

    private int CompareAchromatic(AhsvColor hsv1, AhsvColor hsv2)
    {
      // Compare achromatic colors
      if (hsv1.S==0 && hsv2.S==0)
        return -hsv1.V.CompareTo(hsv2.V);
      if (hsv1.S==0 && hsv2.S>0)
        return -1;
      if (hsv1.S>0 && hsv2.S==0)
        return +1;
      return ComparePastel(hsv1, hsv2);
    }

    private int ComparePastel(AhsvColor hsv1, AhsvColor hsv2)
    {
      //Compare pastele colors
      var p1 = ToPastelFamily(hsv1);
      var p2 = ToPastelFamily(hsv2);
      if (p1>p2)
        return +1;
      if (p1<p2)
        return -1;
      if (p1==p2 && p1==0)
        return CompareSaturation(hsv1, hsv2);
      return CompareHue(hsv1, hsv2);
    }

    private int CompareSaturation(AhsvColor hsv1, AhsvColor hsv2)
    {
      if (hsv1.S==hsv2.S)
        return hsv2.V.CompareTo(hsv1.V);
      return CompareHue(hsv1, hsv2);
    }

    private int ToPastelFamily(AhsvColor hsv)
    {
      var p = Math.Sqrt(hsv.S*hsv.S+(1-hsv.V)*(1-hsv.H));
      if (p<=0.33)
        return 0;
      return 1;
    }

    private int CompareValue(AhsvColor hsv1, AhsvColor hsv2)
    {
      return hsv2.V.CompareTo(hsv1.V);
    }

    private int CompareHue(AhsvColor hsv1, AhsvColor hsv2)
    {
      var h1 = hsv1.H;// ToHueFamily(hsv1);
      var h2 = hsv2.H;// ToHueFamily(hsv2);
      if (h1>h2)
        return +1;
      if (h1<h2)
        return -1;
      return CompareValue(hsv1, hsv2);
    }

    private int ToHueFamily(AhsvColor hsv)
    {
      return (int)hsv.H.ToHueFamily();
    }

  }
}

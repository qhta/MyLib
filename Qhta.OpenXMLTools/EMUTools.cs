using System;
using DocumentFormat.OpenXml;

namespace Qhta.OpenXmlTools;

public static class EMUTools
{
  public static double ToMM(this Int32Value x, int prec=0)
  {
    return Math.Round(x / 56.6952380952381, prec);
  }

  public static double ToMM(this UInt32Value x, int prec=0)
  {
    return Math.Round(x / 56.6952380952381, prec);
  }

  public static string ToStringMM(this Int32Value x, int prec=0)
  {
    return $"{Math.Round(x / 56.6952380952381, prec)}mm";
  }

  public static string ToStringMM(this UInt32Value x, int prec=0)
  {
    return $"{Math.Round(x / 56.6952380952381, prec)}mm";
  }
}
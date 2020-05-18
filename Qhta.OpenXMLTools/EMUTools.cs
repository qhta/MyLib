using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;

namespace Qhta.OpenXMLTools
{
  public static class EMUTools
  {
    public static double ToMM(this Int32Value x)
    {
      return x / 56.6952380952381;
    }

    public static double ToMM(this Int32Value x, int prec)
    {
      return Math.Round(x / 56.6952380952381, prec);
    }

    public static double ToMM(this UInt32Value x)
    {
      return x / 56.6952380952381;
    }

    public static double ToMM(this UInt32Value x, int prec)
    {
      return Math.Round(x / 56.6952380952381, prec);
    }
  }
}

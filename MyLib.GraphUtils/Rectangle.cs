using System;
using System.Collections.Generic;
using System.Text;

namespace MyLib.GraphUtils
{
  public class Rectangle: Figure
  {
    public override List<DPoint> GetPoints()
    {
      return new List<DPoint> { new DPoint(Left, Top), new DPoint(Left+Width, Top), new DPoint(Left+Width, Top+Height), new DPoint(Left, Top+Height) };
    }
  }
}

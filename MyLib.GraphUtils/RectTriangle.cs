using System.Collections.Generic;

namespace MyLib.GraphUtils
{

  public class RectTriangle: Figure
  {

    public override List<DPoint> GetPoints()
    {
      return new List<DPoint> { new DPoint(Left, Top), new DPoint(Left+Width, Top), new DPoint(Left+Width, Top+Height), new DPoint(Left, Top+Height) };
    }

  }
}

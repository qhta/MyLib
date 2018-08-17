using System;
using System.Collections.Generic;

namespace MyLib.GraphUtils
{
  /// <summary>
  /// Abstrakcyjna figura
  /// </summary>
  public abstract class Figure
  {
    /// <summary>
    /// Współrzędna X lewego, górnego rogu
    /// </summary>
    public double Left { get; set; }
    /// <summary>
    /// Współrzędna Y lewego, górnego rogu
    /// </summary>
    public double Top { get; set; }
    /// <summary>
    /// Szerokość
    /// </summary>
    public double Width { get; set; }
    /// <summary>
    /// Wysokość
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Punkt środkowy
    /// </summary>
    public DPoint Center => new DPoint((Left+Width)/2, (Top+Height)/2);

    public abstract List<DPoint> GetPoints();
  }
}

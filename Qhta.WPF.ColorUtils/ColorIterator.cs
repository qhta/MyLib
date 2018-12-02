using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using Qhta.Drawing;
using MediaColor = System.Windows.Media.Color;

namespace Qhta.WPF
{

  /// <summary>
  /// Iterator class for <c>System.Windows.Media.Color</c> type.
  /// May be used in <c>foreach</c> instructions.
  /// It is a wrapper for <see cref="Qhta.Drawing.ColorIterator"/>
  /// </summary>
  public class ColorIterator : IEnumerator<Color>, IEnumerable<Color>
  {
    /// <summary>
    /// Constructor for iterate from <paramref name="startColor"/> to <paramref name="endColor"/>
    /// giving <paramref name="steps"/> color values. The first value is <paramref name="startColor"/>,
    /// the last is <paramref name="endColor"/>. Both <paramref name="startColor"/> and <paramref name="endColor"/>
    /// are translated to AHSV color space (with each dimension scaled from 0 to 1). 
    /// The differences between A, H, S and V dimensions of <paramref name="endColor"/> and 
    /// A, H, S and V of <paramref name="startColor"/> are divided by n=<paramref name="steps"/>
    /// resulting in aDelta, sDelta and vDelta respectively.
    /// If hDelta equals 0 and <paramref name="hueChange"/> is <see cref="HueGradient.Positive"/>
    /// then hDelta is set to 1/n, and if it is <see cref="HueGradient.Positive"/> - then to -1/n;
    /// In other case if <paramref name="hueChange"/> 
    /// is <see cref="HueGradient.Positive"/> then hDelta is evaluated as a positive value, 
    /// and if it is <see cref="HueGradient.Negative"/> then hDelta is evaluated as a negative value.
    /// </summary>
    /// <param name="startColor"></param>
    /// <param name="endColor"></param>
    /// <param name="steps"></param>
    /// <param name="hueChange"></param>
    public ColorIterator(MediaColor startColor, MediaColor endColor, int steps, HueChange hueChange)
    {
      if (startColor==Colors.Lime && endColor==Colors.Lime)
        Debug.Assert(true);
      Iterator = new Qhta.Drawing.ColorIterator(startColor.ToDrawingColor(), endColor.ToDrawingColor(), steps, (Qhta.Drawing.HueChange)hueChange);
    }

    private Qhta.Drawing.ColorIterator Iterator;

    public void Reset()
    {
      Iterator.Reset();
    }

    public Color Current => Iterator.Current.ToMediaColor();

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
      return Iterator.MoveNext();
    }

    public void Dispose()
    {
      //throw new NotImplementedException();
    }

    public IEnumerator<Color> GetEnumerator()
    {
      return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}

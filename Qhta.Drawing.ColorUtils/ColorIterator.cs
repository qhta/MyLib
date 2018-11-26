using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Qhta.Drawing
{
  /// <summary>
  /// Enumeration type describing how the Hue value is treated by <see cref="ColorIterator"/> 
  /// when there is no difference between hue of start and end color.
  /// </summary>
  public enum HueGradient
  {
    /// <summary>
    /// No Hue increment
    /// </summary>
    None,
    /// <summary>
    /// Positive Hue increment (from red to violet)
    /// </summary>
    Positive,
    /// <summary>
    /// Negative Hue increment (from violet to red)
    /// </summary>
    Negative
  }

  /// <summary>
  /// Iterator class for <c>System.Windows.Media.Color</c> type.
  /// May be used in <c>foreach</c> instructions.
  /// </summary>
  public class ColorIterator : IEnumerator<Color>, IEnumerable<Color>
  {
    /// <summary>
    /// Default constructor for iterate from <paramref name="startColor"/> to <paramref name="endColor"/>
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
    public ColorIterator(Color startColor, Color endColor, int steps, HueGradient hueChange)
    {
      this.Color0=startColor;
      this.Color1=endColor;
      this.steps = steps;
      this.hueChange = hueChange;

      this.startColor = System.Drawing.Color.FromArgb(startColor.A, startColor.R, startColor.G, startColor.B);
      this.endColor = System.Drawing.Color.FromArgb(endColor.A, endColor.R, endColor.G, endColor.B);
      startHSV = Qhta.Drawing.ColorSpaceConverter.Color2HSV(this.startColor);
      endHSV = Qhta.Drawing.ColorSpaceConverter.Color2HSV(this.endColor);
      int n = steps;
      hDelta = (endHSV.H-startHSV.H)/n;
      if (hDelta==0)
      {
        if (hueChange==HueGradient.Positive)
          hDelta = 1.0/n;
        else
        if (hueChange==HueGradient.Negative)
          hDelta = -1.0/n;
      }
      else
      {
        if (hDelta>0 && hueChange==HueGradient.Negative)
          hDelta = (endHSV.H-(1+startHSV.H))/n;
        else if (hDelta<0 && hueChange==HueGradient.Positive)
          hDelta = -(startHSV.H-(1+endHSV.H))/n;
      }
      sDelta = (endHSV.S-startHSV.S)/n;
      vDelta = (endHSV.V-startHSV.V)/n;
      aStart = (this.startColor.A/255.0);
      var aEnd = (this.endColor.A/255.0);
      aDelta = (aEnd-aStart)/n;
      Reset();
    }

    private Color Color0;
    private Color Color1;
    private int steps;
    private HueGradient hueChange;
    private int counter;

    private System.Drawing.Color startColor;
    private System.Drawing.Color endColor;
    private Qhta.Drawing.ColorHSV startHSV;
    private Qhta.Drawing.ColorHSV endHSV;
    double hDelta;
    double sDelta;
    double vDelta;
    double aDelta;
    double aStart;

    public void Reset()
    {
      Current = Color0;
      counter=0;
    }

    public Color Current { get; private set; }

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
      if (counter>=steps)
        return false;
      if (counter==0)
        Current=Color0;
      else
      if (counter==steps-1)
        Current=Color1;
      else
      {
        var i = counter;
        var hue = (startHSV.H+i*hDelta) % 1.0;
        if (hue<0)
          hue+=1.0;
        var sat = startHSV.S+i*sDelta;
        var val = startHSV.V+i*vDelta;
        var alpha = aStart+i*aDelta;
        var newColor = Qhta.Drawing.ColorSpaceConverter.HSV2Color(new Drawing.ColorHSV(hue, sat, val));
        Current = Color.FromArgb((byte)(alpha*255), newColor.R, newColor.G, newColor.B);
        //Debug.WriteLine($"CurrentColor = {Current}");
      }
      counter++;
      return true;
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

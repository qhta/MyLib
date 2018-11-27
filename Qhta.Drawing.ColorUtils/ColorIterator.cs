using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Qhta.Drawing
{
  /// <summary>
  /// Iterator class for <c>System.Windows.Media.Color</c> type.
  /// May be used in <c>foreach</c> instructions.
  /// </summary>
  public class ColorIterator : IEnumerator<Color>, IEnumerator<AhsvColor>, IEnumerable<Color>, IEnumerable<AhsvColor>
  {
    /// <summary>
    /// Constructor for iterate from <paramref name="startColor"/> to <paramref name="endColor"/>
    /// giving <paramref name="steps"/> color values. The first value is <paramref name="startColor"/>,
    /// the last is <paramref name="endColor"/>. Both <paramref name="startColor"/> and <paramref name="endColor"/>
    /// are translated to AHSV color space (with each dimension scaled from 0 to 1). 
    /// The differences between A, H, S and V dimensions of <paramref name="endColor"/> and 
    /// A, H, S and V of <paramref name="startColor"/> are divided by n=<paramref name="steps"/>
    /// resulting in aDelta, sDelta and vDelta respectively.
    /// If hDelta equals 0 and <paramref name="hueChange"/> is <see cref="HueChange.Positive"/>
    /// then hDelta is set to 1/n, and if it is <see cref="HueChange.Positive"/> - then to -1/n;
    /// In other case if <paramref name="hueChange"/> 
    /// is <see cref="HueChange.Positive"/> then hDelta is evaluated as a positive value, 
    /// and if it is <see cref="HueChange.Negative"/> then hDelta is evaluated as a negative value.
    /// </summary>
    /// <param name="startColor"></param>
    /// <param name="endColor"></param>
    /// <param name="steps"></param>
    /// <param name="hueChange"></param>
    public ColorIterator(Color startColor, Color endColor, int steps, HueChange hueChange)
    {
      this.Color0=startColor;
      this.Color1=endColor;
      this.steps = steps;
      this.hueChange = hueChange;
      this.startArgb = ColorSpaceConverter.ToArgb(startColor);
      this.endArgb = ColorSpaceConverter.ToArgb(endColor);
      this.startAhsv = startArgb.ToAhsv();
      this.endAhsv = endArgb.ToAhsv();
      Init();
    }

    /// <summary>
    /// Constructor for iterate from <paramref name="startColor"/> to <paramref name="endColor"/>
    /// giving <paramref name="steps"/> color values. The first value is <paramref name="startColor"/>,
    /// the last is <paramref name="endColor"/>. Both <paramref name="startColor"/> and <paramref name="endColor"/>
    /// are in AHSV color space (with each dimension scaled from 0 to 1). 
    /// The differences between A, H, S and V dimensions of <paramref name="endColor"/> and 
    /// A, H, S and V of <paramref name="startColor"/> are divided by n=<paramref name="steps"/>
    /// resulting in aDelta, sDelta and vDelta respectively.
    /// If hDelta equals 0 and <paramref name="hueChange"/> is <see cref="HueChange.Positive"/>
    /// then hDelta is set to 1/n, and if it is <see cref="HueChange.Positive"/> - then to -1/n;
    /// In other case if <paramref name="hueChange"/> 
    /// is <see cref="HueChange.Positive"/> then hDelta is evaluated as a positive value, 
    /// and if it is <see cref="HueChange.Negative"/> then hDelta is evaluated as a negative value.
    /// </summary>
    /// <param name="startColor"></param>
    /// <param name="endColor"></param>
    /// <param name="steps"></param>
    /// <param name="hueChange"></param>
    public ColorIterator(AhsvColor startColor, AhsvColor endColor, int steps, HueChange hueChange)
    {
      this.Color0=startColor.ToColor();
      this.Color1=endColor.ToColor();
      this.steps = steps;
      this.hueChange = hueChange;
      this.startArgb = ColorSpaceConverter.ToArgb(startColor);
      this.endArgb = ColorSpaceConverter.ToArgb(endColor);
      this.startAhsv = startColor;
      this.endAhsv = endColor;
      Init();
    }

    private void Init()
    { 
      int n = steps;
      hDelta = (endAhsv.H-startAhsv.H)/n;
      if (hDelta==0)
      {
        if (hueChange==HueChange.Positive)
          hDelta = 1.0/n;
        else
        if (hueChange==HueChange.Negative)
          hDelta = -1.0/n;
      }
      else
      {
        if (hDelta>0 && hueChange==HueChange.Negative)
          hDelta = (endAhsv.H-(1+startAhsv.H))/n;
        else if (hDelta<0 && hueChange==HueChange.Positive)
          hDelta = -(startAhsv.H-(1+endAhsv.H))/n;
      }
      sDelta = (endAhsv.S-startAhsv.S)/n;
      vDelta = (endAhsv.V-startAhsv.V)/n;
      aDelta = (endAhsv.A-startAhsv.A)/n;
      Reset();
    }

    private Color Color0;
    private Color Color1;
    private int steps;
    private HueChange hueChange;
    private int counter;

    private ArgbColor startArgb;
    private ArgbColor endArgb;
    private AhsvColor startAhsv;
    private AhsvColor endAhsv;
    double hDelta;
    double sDelta;
    double vDelta;
    double aDelta;

    public void Reset()
    {
      currentColor = Color0;
      counter=0;
    }

    public Color Current => currentColor;
    AhsvColor IEnumerator<AhsvColor>.Current => currentAhsv;

    private Color currentColor;
    private AhsvColor currentAhsv;

    object IEnumerator.Current => currentColor;


    public bool MoveNext()
    {
      if (counter>=steps)
        return false;
      if (counter==0)
      {
        currentColor=Color0;
        currentAhsv = startAhsv;
      }
      else
      if (counter==steps-1)
      {
        currentColor=Color1;
        currentAhsv = endAhsv;
      }
      else
      {
        var i = counter;
        var hue = (startAhsv.H+i*hDelta) % 1.0;
        if (hue<0)
          hue+=1.0;
        var sat = startAhsv.S+i*sDelta;
        var val = startAhsv.V+i*vDelta;
        var alpha = startAhsv.A+i*aDelta;
        currentAhsv = new Drawing.AhsvColor(alpha, hue, sat, val);
        currentColor = ColorSpaceConverter.ToColor(currentAhsv);
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
      return this;
    }

    public IEnumerator<AhsvColor> AsAhsvIterator()
    {
      return this as IEnumerator<AhsvColor>;
    }

    IEnumerator<AhsvColor> IEnumerable<AhsvColor>.GetEnumerator()
    {
      return this as IEnumerator<AhsvColor>;
    }
  }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Qhta.WPF
{
  public enum HueGradient
  {
    None,
    Positive,
    Negative
  }

  public class ColorIterator : IEnumerator<Color>, IEnumerable<Color>
  {
    public ColorIterator(Color Color0, Color Color1, int steps, HueGradient hueChange)
    {
      this.Color0=Color0;
      this.Color1=Color1;
      this.steps = steps;
      this.hueChange = hueChange;

      startColor = System.Drawing.Color.FromArgb(Color0.A, Color0.R, Color0.G, Color0.B);
      endColor = System.Drawing.Color.FromArgb(Color1.A, Color1.R, Color1.G, Color1.B);
      startHSV = Qhta.Drawing.ColorSpaceConverter.Color2HSV(startColor);
      endHSV = Qhta.Drawing.ColorSpaceConverter.Color2HSV(endColor);
      int n = steps;
      hDelta = Math.Abs((endHSV.H-startHSV.H)/n);
      if (hDelta==0 && hueChange!=HueGradient.None)
        hDelta = 1.0/n;
      if (hueChange==HueGradient.Negative)
        hDelta = -hDelta;
      sDelta = (endHSV.S-startHSV.S)/n;
      vDelta = (endHSV.V-startHSV.V)/n;
      aStart = (startColor.A/255.0);
      var aEnd = (endColor.A/255.0);
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

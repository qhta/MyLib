using System;
using System.Diagnostics;
using System.Windows.Media;
using Qhta.Drawing;

namespace Qhta.WPF
{
  public class KnownColor
  {
    public string Name { get; set; }
    public Color Color { get; set; }

    public bool IsSelected
    {
      get
      {
        Debug.WriteLine($"GetIsSelected[{Name}]({_IsSelected})");
        return _IsSelected;
      }
      set
      {
        _IsSelected = value;
        Debug.WriteLine($"SetIsSelected[{Name}]({value})");
      }
    }
    private bool _IsSelected;

    public override string ToString()
    {
      return $"{Name} ({Color})";
    }

    public string NameAndARGB
    {
      get
      {
        var argb = Color;
        return $"{Name} (A={argb.A} R={argb.R} G={argb.G} B={argb.B})";
      }
    }

    public string NameAndAHSV
    {
      get
      {
        var ahsv = Color.ToDrawingColor().ToAhsv();
        return $"{Name} (A={Math.Round(ahsv.A*255)} H={Math.Round(ahsv.H*360)} S={Math.Round(ahsv.S*255)} V={Math.Round(ahsv.V*255)})";
      }
    }

    public string NameAndAHLS
    {
      get
      {
        var ahls = Color.ToDrawingColor().ToAhls();
        return $"{Name} (A={Math.Round(ahls.A*255)} H={Math.Round(ahls.H*360)} L={Math.Round(ahls.L*255)} S={Math.Round(ahls.S*255)})";
      }
    }

    public static implicit operator Color(KnownColor value)
    {
      return value.Color;
    }
  }
}

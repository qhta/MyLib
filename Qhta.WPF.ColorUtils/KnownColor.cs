using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using Qhta.Drawing;

namespace Qhta.WPF
{
  public class KnownColor : INotifyPropertyChanged
  {
    public string Name
    {
      get => _Name;
      set
      {
        if (_Name!=value)
        {
          _Name=value;
          NotifyPropertyChanged(nameof(Name));
        }
      }
    }
    private string _Name;

    public Color Color { get; set; }

    public bool IsSelected
    {
      get => _IsSelected;
      set
      {
        if (_IsSelected!=value)
        {
          _IsSelected = value;
          NotifyPropertyChanged(nameof(IsSelected));
        }
      }
    }
    private bool _IsSelected;

    public event PropertyChangedEventHandler PropertyChanged;
    public void NotifyPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

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

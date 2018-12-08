using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using Qhta.Drawing;

namespace Qhta.WPF
{
  public class KnownBrush : INotifyPropertyChanged
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

    public Brush Brush { get; set; }

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
      return $"{Name}";
    }

    public static implicit operator Brush(KnownBrush value)
    {
      return value.Brush;
    }
  }
}

using System.ComponentModel;
using System.Drawing;

namespace Qhta.Drawing
{
  public abstract class DrawingItem: INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propertyName)
    {
      if (PropertyChanged!=null)
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public double Left
    {
      get => _Left;
      set { if (_Left!=value) { _Left=value; NotifyPropertyChanged(nameof(Left)); } }
    }
    private double _Left;

    public double Top
    {
      get => _Top;
      set { if (_Top!=value) { _Top=value; NotifyPropertyChanged(nameof(Top)); } }
    }
    private double _Top;

    public double Width
    {
      get => _Width;
      set { if (_Width!=value) { _Width=value; NotifyPropertyChanged(nameof(Width)); } }
    }
    private double _Width;

    public double Height
    {
      get => _Height;
      set { if (_Height!=value) { _Height=value; NotifyPropertyChanged(nameof(Height)); } }
    }
    private double _Height;

    public abstract void Draw(DrawingContext context);
  }
}

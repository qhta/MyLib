using System.ComponentModel;
using System.Drawing;

namespace Qhta.Drawing
{
  public abstract class ClosedFigure: DrawingItem
  {
    [TypeConverter(typeof(BrushConverter))]
    public Brush Fill
    {
      get => _Fill;
      set { if (_Fill!=value) { _Fill=value; NotifyPropertyChanged(nameof(Fill)); } }
    }
    private Brush _Fill = Brushes.Black;

    //public Color FillColor
    //{
    //  get => _FillColor;
    //  set { if (_FillColor!=value) { _FillColor=value; NotifyPropertyChanged(nameof(Fill)); } }
    //}
    //private Color _FillColor = Color.Black;

    [TypeConverter(typeof(PenConverter))]
    public Pen Outline
    {
      get => _Outline;
      set { if (_Outline!=value) { _Outline=value; NotifyPropertyChanged(nameof(Outline)); } }
    }
    private Pen _Outline = new Pen(Brushes.Black,1);

    //public Color LineColor
    //{
    //  get => _LineColor;
    //  set { if (_LineColor!=value) { _LineColor=value; NotifyPropertyChanged(nameof(LineColor)); } }
    //}
    //private Color _LineColor = Color.Black;

    //public virtual double LineWidth
    //{
    //  get => _LineWidth;
    //  set { if (_LineWidth!=value) { _LineWidth=value; NotifyPropertyChanged(nameof(LineWidth)); } }
    //}
    //private double _LineWidth;

    public override void Draw(DrawingContext context)
    {
      var graphics = context.Graphics;
      var left = (float)context.TransformX(this.Left);
      var top = (float)context.TransformY(this.Top);
      var width = (float)context.TransformX(this.Width);
      var height = (float)context.TransformY(this.Height);
      if (Fill!=null && Fill!=Brushes.Transparent)
      {
        var brush = Fill;
        FillInterior(graphics, brush, left, top, width, height);
      }
      if (Outline!=null)
      {
        var lineWidth = Outline.Width;
        lineWidth=(float)context.ScaleXY(lineWidth);
        Pen pen = Outline.Clone() as Pen;
        pen.Width = lineWidth;
        DrawOutline(graphics, pen, left, top, width, height);
      }
    }

    protected abstract void FillInterior(Graphics graphics, Brush brush, float left, float top, float width, float height);


    protected abstract void DrawOutline(Graphics graphics, Pen pen, float left, float top, float width, float height);

  }
}

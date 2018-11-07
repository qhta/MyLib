using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Qhta.Drawing
{
  public abstract class Shape: DrawingItem
  {
    [TypeConverter(typeof(BrushConverter))]
    public Brush Fill
    {
      get => _Fill;
      set { if (_Fill!=value) { _Fill=value; NotifyPropertyChanged(nameof(Fill)); } }
    }
    private Brush _Fill = Brushes.Black;

    [TypeConverter(typeof(BrushConverter))]
    public Pen Outline
    {
      get => _Outline;
      set { if (_Outline!=value) { _Outline=value; NotifyPropertyChanged(nameof(Outline)); } }
    }
    private Pen _Outline = new Pen(Brushes.Black,1);


    #region Stroke property
    public Brush Stroke
    {
      get => Outline?.Brush;
      set { if (Outline!=null) {  Outline.Brush = value; NotifyPropertyChanged(nameof(Outline)); }
      }
    }
    #endregion

    #region StrokeThickness property
    public double StrokeThickness
    {
      get => (double)Outline?.Width;
      set { if (Outline!=null) { Outline.Width = (float)value; NotifyPropertyChanged(nameof(Outline)); }
      }
    }
    #endregion

    #region StrokeStartCap property
    public LineCap StrokeStartCap
    {
      get => (LineCap)Outline?.StartCap;
      set
      {
        if (Outline!=null) { Outline.StartCap = value; NotifyPropertyChanged(nameof(Outline)); }
      }
    }

    #endregion

    #region StrokeEndCap property
    public LineCap StrokeEndCap
    {
      get => (LineCap)Outline?.EndCap;
      set
      {
        if (Outline!=null) { Outline.EndCap = value; NotifyPropertyChanged(nameof(Outline)); }
      }
    }
    #endregion

    #region StrokeDashCap property
    public DashCap StrokeDashCap
    {
      get => (DashCap)Outline?.DashCap;
      set
      {
        if (Outline!=null) { Outline.DashCap = value; NotifyPropertyChanged(nameof(Outline)); }
      }
    }
    #endregion

    #region StrokeLineJoin property
    public LineJoin StrokeLineJoin
    {
      get => (LineJoin)Outline?.LineJoin;
      set
      {
        if (Outline!=null) { Outline.LineJoin = value; NotifyPropertyChanged(nameof(Outline)); }
      }
    }
    #endregion

    #region StrokeMiterLimit
    public double StrokeMiterLimit
    {
      get => (double)Outline?.MiterLimit;
      set
      {
        if (Outline!=null) { Outline.MiterLimit = (float)value; NotifyPropertyChanged(nameof(Outline)); }
      }
    }
    #endregion

    #region StrokeDashOffset
    public double StrokeDashOffset
    {
      get => (double)Outline?.DashOffset;
      set
      {
        if (Outline!=null) { Outline.DashOffset = (float)value; NotifyPropertyChanged(nameof(Outline)); }
      }
    }

    #endregion

    #region StrokeLineJoin property
    public DashStyle StrokeDashStyle
    {
      get => (DashStyle)Outline?.DashStyle;
      set
      {
        if (Outline!=null) { Outline.DashStyle = value; NotifyPropertyChanged(nameof(Outline)); }
      }
    }
    #endregion

    #region StrokeDashArray property
    public double[] StrokeDashArray
    {
      get
      {
        if (Outline!=null && Outline.DashPattern!=null)
          return Outline.DashPattern.Select(item=>(double)item).ToArray();
        return null;
      }
      set
      {
        if (Outline!=null)
        {
          if (value==null && Outline.DashPattern!=null)
          {
            Outline.DashPattern=null;
            NotifyPropertyChanged(nameof(Outline));
          }
          else if (value!=null)
          {
            Outline.DashPattern = value.Select(item=>(float)item).ToArray();
            NotifyPropertyChanged(nameof(Outline));
          }
        }
      }
      }
    #endregion

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

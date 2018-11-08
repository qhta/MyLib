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

    //[TypeConverter(typeof(BrushConverter))]
    public Pen Outline
    {
      get => _Outline;
      set { if (_Outline!=value) { _Outline=value; NotifyPropertyChanged(nameof(Outline)); } }
    }
    private Pen _Outline = new Pen(Brushes.Black,1);


    #region Stroke property
    public Brush Stroke
    {
      get => _Brush;
      set
      {
        if (_Brush!=value)
        {
          _Brush = value;
          NotifyPropertyChanged(nameof(Brush));
          if (Outline!=null) { Outline.Brush = value; NotifyPropertyChanged(nameof(Outline)); }
        }
      }
    }
    private Brush _Brush;
    #endregion

    #region StrokeThickness property
    public double StrokeThickness
    {
      get => _StrokeThickness;
      set
      {
        if (_StrokeThickness!=value)
        {
          _StrokeThickness = value;
          NotifyPropertyChanged(nameof(StrokeThickness));
          if (Outline!=null) { Outline.Width = (float)value; NotifyPropertyChanged(nameof(Outline)); }
        }
      }
    }
    private double _StrokeThickness = 1.0;
    #endregion

    #region StrokePenAlignment property
    public PenAlignment StrokePenAlignment
    {
      get => _StrokePenAlignment;
      set
      {
        if (_StrokePenAlignment!=value)
        {
          _StrokePenAlignment = value;
          NotifyPropertyChanged(nameof(StrokePenAlignment));
          if (Outline!=null) { Outline.Alignment = (PenAlignment)value; NotifyPropertyChanged(nameof(Outline)); }
        }
      }
    }
    private PenAlignment _StrokePenAlignment = PenAlignment.Center;
    #endregion

    #region StrokeStartCap property
    public LineCap StrokeStartCap
    {
      get => _StrokeStartCap;
      set
      {
        if (_StrokeStartCap!=value)
        {
          _StrokeStartCap = value;
          NotifyPropertyChanged(nameof(StrokeStartCap));
          if (Outline!=null) { Outline.StartCap = value; NotifyPropertyChanged(nameof(Outline)); }
        }
      }
    }
    private LineCap _StrokeStartCap = LineCap.Flat;
    #endregion

    #region StrokeEndCap property
    public LineCap StrokeEndCap
    {
      get => _StrokeEndCap;
      set
      {
        if (_StrokeEndCap!=value)
        {
          _StrokeEndCap=value;
          NotifyPropertyChanged(nameof(StrokeEndCap));
          if (Outline!=null) { Outline.EndCap = value; NotifyPropertyChanged(nameof(Outline)); }
        }
      }
    }
    private LineCap _StrokeEndCap = LineCap.Flat;
    #endregion

    #region StrokeDashCap property
    public DashCap StrokeDashCap
    {
      get => _StrokeDashCap;
      set
      {
        if (_StrokeDashCap!=value)
        {
          _StrokeDashCap = value;
          NotifyPropertyChanged(nameof(StrokeDashCap));
          if (Outline!=null) { Outline.DashCap = value; NotifyPropertyChanged(nameof(Outline)); }
        }
      }
    }
    private DashCap _StrokeDashCap = DashCap.Flat;
    #endregion

    #region StrokeLineJoin property
    public LineJoin StrokeLineJoin
    {
      get => _StrokeLineJoin;
      set
      {
        if (_StrokeLineJoin!=value)
        {
          _StrokeLineJoin=value;
          NotifyPropertyChanged(nameof(StrokeLineJoin));
          if (Outline!=null) { Outline.LineJoin = value; NotifyPropertyChanged(nameof(Outline)); }
        }
      }
    }
    private LineJoin _StrokeLineJoin = LineJoin.Miter;
    #endregion

    #region StrokeMiterLimit
    public double StrokeMiterLimit
    {
      get => _StrokeMiterLimit;
      set
      {
        if (_StrokeMiterLimit!=value)
        {
          _StrokeMiterLimit = value;
          NotifyPropertyChanged(nameof(StrokeMiterLimit));
          if (Outline!=null) { Outline.MiterLimit = (float)value; NotifyPropertyChanged(nameof(Outline)); }
        }
      }
    }
    private double _StrokeMiterLimit = 0;
    #endregion

    #region StrokeDashOffset
    public double StrokeDashOffset
    {
      get => _StrokeDashOffset;
      set
      {
        if (_StrokeDashOffset!=value)
        {
          _StrokeDashOffset=value;
          NotifyPropertyChanged(nameof(StrokeDashOffset));
          double notNaN = double.IsNaN(value) ? 0.0 : value;
          if (Outline!=null) { Outline.DashOffset = (float)notNaN; NotifyPropertyChanged(nameof(Outline)); }
        }
      }
    }
    private double _StrokeDashOffset = 0.0;
    #endregion

    #region StrokeDashArray property
    public double[] StrokeDashArray
    {
      get
      {
        return _DashPattern;
      }
      set
      {
        if (Outline!=null)
        {
          if (value==null && Outline.DashPattern!=null)
          {
            Outline.DashPattern=null;
          }
          else if (value!=null)
          {
            Outline.DashPattern = value.Select(item=>(float)item).ToArray();
          }
          NotifyPropertyChanged(nameof(Outline));
        }
        _DashPattern=value;
        NotifyPropertyChanged(nameof(StrokeDashArray));
      }
    }
    private double[] _DashPattern;
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
        FillInterior(context, brush, left, top, width, height);
      }
      if (Outline!=null)
      {
        var lineWidth = Outline.Width;
        lineWidth=(float)context.ScaleXY(lineWidth);
        Pen pen = Outline.Clone() as Pen;
        pen.Width = lineWidth;
        DrawOutline(context, pen, left, top, width, height);
      }
    }

    public abstract void FillInterior(DrawingContext context, Brush brush, float left, float top, float width, float height);


    public abstract void DrawOutline(DrawingContext context, Pen pen, float left, float top, float width, float height);

  }
}

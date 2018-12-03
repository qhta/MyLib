using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Qhta.Drawing;

namespace Qhta.WPF.Controls
{

  public class ColorSlider: FrameworkElement
  {
    public ColorSlider()
    {
      this.DefaultStyleKey = typeof(ColorSlider);
      this.PreviewMouseLeftButtonDown+=ColorSlider_MouseLeftButtonDown;
      this.MouseLeftButtonUp+=ColorSlider_MouseLeftButtonUp;
      this.MouseMove+=ColorSlider_MouseMove;
    }

    #region SelectedColor property
    public Color SelectedColor
    {
      get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
      ("SelectedColor", typeof(Color), typeof(ColorSlider),
        new FrameworkPropertyMetadata(Colors.Transparent,
        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
          SelectedColorPropertyChanged));

    private static void SelectedColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c = (sender as ColorSlider);
      if (!c.isSelectedColorChanged)
      {
        c.isSelectedColorChanged=true;
        c.SelectedColorChanged();
        c.ValueChanged?.Invoke(c, new ValueChangedEventArgs<Color>(c.SelectedColor));
        c.isSelectedColorChanged=false;
      }
    }

    private bool isSelectedColorChanged;

    private void SelectedColorChanged()
    {
      if (!isMouseDown)
      {
        if (!isPositionChanged)
          SetPosition(SelectedColor);
      }
    }
    #endregion

    #region Color0 property
    public Color Color0
    {
      get => (Color)GetValue(Color0Property);
      set => SetValue(Color0Property, value);
    }

    public static readonly DependencyProperty Color0Property = DependencyProperty.Register
      ("Color0", typeof(Color), typeof(ColorSlider),
        new FrameworkPropertyMetadata(Colors.Transparent, ChangeBrushProperty));
    #endregion

    #region Color1 property
    public Color Color1
    {
      get => (Color)GetValue(Color1Property);
      set => SetValue(Color1Property, value);
    }

    public static readonly DependencyProperty Color1Property = DependencyProperty.Register
      ("Color1", typeof(Color), typeof(ColorSlider),
        new FrameworkPropertyMetadata(Colors.Transparent, ChangeBrushProperty));
    #endregion

    #region Orientation property
    public Orientation Orientation
    {
      get => (Orientation)GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register
      ("Orientation", typeof(Orientation), typeof(ColorSlider),
        new FrameworkPropertyMetadata(Orientation.Horizontal, ChangeBrushProperty));
    #endregion

    #region HueChange property
    public HueChange HueChange
    {
      get => (HueChange)GetValue(HueChangeProperty);
      set => SetValue(HueChangeProperty, value);
    }

    public static readonly DependencyProperty HueChangeProperty = DependencyProperty.Register
      ("HueChange", typeof(HueChange), typeof(ColorSlider),
        new FrameworkPropertyMetadata(HueChange.None, ChangeBrushProperty));
    #endregion

    #region Resolution property
    public int Resolution
    {
      get => (int)GetValue(ResolutionProperty);
      set => SetValue(ResolutionProperty, value);
    }

    public static readonly DependencyProperty ResolutionProperty = DependencyProperty.Register
      ("Resolution", typeof(int), typeof(ColorSlider),
        new FrameworkPropertyMetadata(100, ChangeBrushProperty));
    #endregion

    #region Brush
    protected Brush Brush { get; private set; } = null;

    private static void ChangeBrushProperty(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c = (sender as ColorSlider);
      c.Brush=null;
      c.colorMap=null;
      c.InvalidateVisual();
    }

    private void CreateBrush()
    {
      colorMap=null;
      int n = Resolution;
      var xDelta = 1.0/n;
      GradientStop[] stops = new GradientStop[n];
      int i = 0;
      if (Name=="_HSlider")
        Debug.Assert(true);
      foreach (var color in new ColorIterator(Color0, Color1, Resolution, HueChange))
      {
        int k = i;
        double xPos = i*xDelta;
        if (Orientation==Orientation.Vertical)
        {
          k = n-i-1;
          xPos = 1-xPos;
        }
        stops[k]=(new GradientStop(color, xPos));
        i++;
      }
      Brush = new LinearGradientBrush(new GradientStopCollection(stops),
        Orientation==Orientation.Horizontal ? 0 : 90);
      InvalidateVisual();
    }
    #endregion

    public event ValueChangedEventHandler<Color> ValueChanged;

    #region Position property
    /// <summary>
    /// Relative position of pointer over ColorSlider.
    /// Range from 0.0 (leftmost/bottom) to 1.0 (rightmost/top)
    /// </summary>
    public double Position
    {
      get => (double)GetValue(PositionProperty);
      set => SetValue(PositionProperty, value);
    }

    public static readonly DependencyProperty PositionProperty = DependencyProperty.Register
      ("Position", typeof(double), typeof(ColorSlider),
        new FrameworkPropertyMetadata(0.0, PositionPropertyChanged));

    private static void PositionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c = (sender as ColorSlider);
      if (!c.isPositionChanged)
      {
        c.isPositionChanged=true;
        c.PositionChanged((double)args.OldValue, (double)args.NewValue);
        c.isPositionChanged=false;
      }
    }

    private bool isPositionChanged;

    private void PositionChanged(double oldValue, double newValue)
    {
      if (!isSelectedColorChanged)
        SelectedColor = Position2Color(Position);
      InvalidateVisual();
      ValueChanged?.Invoke(this, new ValueChangedEventArgs<Color>(SelectedColor));
    }

    #endregion

    private readonly Pen BackPen = new Pen(Brushes.White, 4);
    private readonly Pen FrontPen = new Pen(Brushes.Black, 1);

    protected override void OnRender(DrawingContext drawingContext)
    {
      if (Visibility!=Visibility.Visible)
        return;
      if (Brush==null)
        CreateBrush();
      double top;
      double left;
      double width;
      double height;
      if (Orientation==Orientation.Vertical)
      {
        left = 0;
        top = ActualHeight*(1-Position)-4;
        height = 8;
        width = ActualWidth;
      }
      else
      {
        left = ActualWidth*Position-4;
        top = 0;
        width = 8;
        height = ActualHeight;
      }
      drawingContext.DrawRectangle(Brush, null, new Rect(0, 0, ActualWidth, ActualHeight));
      drawingContext.DrawRectangle(null, BackPen, new Rect(left, top, width, height));
      drawingContext.DrawRectangle(null, FrontPen, new Rect(left, top, width, height));
    }

    #region MouseMove handling
    private bool isMouseDown;

    private void ColorSlider_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs args)
    {
      isMouseDown=true;
      CaptureMouse();
      ColorSlider_MouseMove(sender, args);
    }

    private void ColorSlider_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs args)
    {
      isMouseDown=false;
      ReleaseMouseCapture();
    }

    private void ColorSlider_MouseMove(object sender, System.Windows.Input.MouseEventArgs args)
    {
      if (isMouseDown)
      {
        var pos = args.GetPosition(this);
        if (Orientation==Orientation.Vertical)
        {
          Position = Math.Max(Math.Min(1-pos.Y/this.ActualHeight, 1), 0);
        }
        else
        {
          Position = Math.Max(Math.Min(pos.X/this.ActualWidth, 1), 0);
        }
        var c = Position2Color(Position);
        if (c!=SelectedColor)
        {
          SelectedColor = c;
        }
      }
    }
    #endregion

    #region Color selection

    public void SetPosition(Color color)
    {
      //Debug.WriteLine($"SetPosition({color})");
      Position = Color2Position(color);
    }

    Color[] colorMap;

    bool isHueMode => Color0==Color1 && HueChange!=HueChange.None;

    public double Color2Position(Color color)
    {
      if (color==Color0)
        return 0;
      if (color==Color1)
        return 1;
      double result = double.NaN;
      if (isHueMode)
      {
        var HSV = Qhta.Drawing.ColorSpaceConverter.ToAhsv(color.ToDrawingColor());
        result = HSV.H;
        return result;
      }
      var n = Resolution;
      if (colorMap==null)
      {
        colorMap = (new ColorIterator(Color0, Color1, n, HueChange)).ToArray();
      }
      double minDiff = double.MaxValue;
      for (int i=0; i<n; i++)
      {
        Color pixelColor;
        pixelColor=colorMap[i]; 
        var diff = ColorUtils.Distance(pixelColor, color);
        if (diff<minDiff)
        {
          minDiff=diff;
          result=i/(double)n;
        }
      }
      return result;
    }

    public Color Position2Color(double value)
    {
      if (value==0)
        return Color0;
      if (value==1)
        return Color1;
      Color result = Colors.Transparent;
      if (isHueMode)
      {
        var HSV = Qhta.Drawing.ColorSpaceConverter.ToAhsv(Color0.ToDrawingColor());
        HSV.H = value;
        result = Qhta.Drawing.ColorSpaceConverter.ToColor(HSV).ToMediaColor();
        return result;
      }
      var n = Resolution;
      if (colorMap==null)
      {
        colorMap = (new ColorIterator(Color0, Color1, n, HueChange)).ToArray();
      }
      int i = Math.Max(Math.Min((int)(n * value), n), 0);
      result=colorMap[i];
      return result;
    }

    #endregion
  }
}

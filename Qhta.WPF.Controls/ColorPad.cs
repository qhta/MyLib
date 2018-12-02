using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Qhta.Drawing;
using Qhta.WPF.Utils;

namespace Qhta.WPF.Controls
{
  using ColorIterator = Qhta.Drawing.ColorIterator;


  public class ColorPad: Control
  {
    public ColorPad()
    {
      this.DefaultStyleKey = typeof(ColorPad);
      this.PreviewMouseLeftButtonDown+=ColorPad_MouseLeftButtonDown;
      this.MouseLeftButtonUp+=ColorPad_MouseLeftButtonUp;
      this.MouseMove+=ColorPad_MouseMove;
    }

    #region SelectedColor property
    public Color SelectedColor
    {
      get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
      ("SelectedColor", typeof(Color), typeof(ColorPad),
        new FrameworkPropertyMetadata(Colors.Transparent,
        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
          SelectedColorPropertyChanged));

    private static void SelectedColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c = (sender as ColorPad);
      c.SelectedColorChanged();
      c.ValueChanged?.Invoke(c, new RoutedPropertyChangedEventArgs<Point>(c.Position, c.Position));
    }

    private void SelectedColorChanged()
    {
      if (!isMouseDown && !isPositionChanged)
      {
        SetPosition(SelectedColor);
      }
    }

    #endregion

    #region Color00 property
    public Color Color00
    {
      get => (Color)GetValue(Color00Property);
      set => SetValue(Color00Property, value);
    }

    public static readonly DependencyProperty Color00Property = DependencyProperty.Register
      ("Color00", typeof(Color), typeof(ColorPad),
        new FrameworkPropertyMetadata(Colors.Transparent,
          FrameworkPropertyMetadataOptions.AffectsRender,
          BitmapParametersChanged));
    #endregion

    #region Color01 property
    public Color Color01
    {
      get => (Color)GetValue(Color01Property);
      set => SetValue(Color01Property, value);
    }

    public static readonly DependencyProperty Color01Property = DependencyProperty.Register
      ("Color01", typeof(Color), typeof(ColorPad),
        new FrameworkPropertyMetadata(Colors.Transparent, 
          FrameworkPropertyMetadataOptions.AffectsRender,
          BitmapParametersChanged));
    #endregion

    #region Color10 property
    public Color Color10
    {
      get => (Color)GetValue(Color10Property);
      set => SetValue(Color10Property, value);
    }

    public static readonly DependencyProperty Color10Property = DependencyProperty.Register
      ("Color10", typeof(Color), typeof(ColorPad),
        new FrameworkPropertyMetadata(Colors.Transparent,
          FrameworkPropertyMetadataOptions.AffectsRender,
          BitmapParametersChanged));
    #endregion

    #region Color11 property
    public Color Color11
    {
      get => (Color)GetValue(Color11Property);
      set => SetValue(Color11Property, value);
    }

    public static readonly DependencyProperty Color11Property = DependencyProperty.Register
      ("Color11", typeof(Color), typeof(ColorPad),
        new FrameworkPropertyMetadata(Colors.Transparent,
          FrameworkPropertyMetadataOptions.AffectsRender,
          BitmapParametersChanged));
    #endregion

    #region HueChange property
    public HueChange HueChange
    {
      get => (HueChange)GetValue(HueChangeProperty);
      set => SetValue(HueChangeProperty, value);
    }

    public static readonly DependencyProperty HueChangeProperty = DependencyProperty.Register
      ("HueChange", typeof(HueChange), typeof(ColorPad),
        new FrameworkPropertyMetadata(HueChange.None, BitmapParametersChanged));
    #endregion

    #region InvariantHue property
    /// <summary>
    /// If <see cref="HueChange"/> = <see cref="HueChange.None">None</see>
    /// then InvariantHue implies constant hue in the whole pad area.
    /// In this case only A, S and V members are taken 
    /// from <see cref="Color00"/>..<see cref="Color11"/> properties to evaluate color in each point.
    /// InvariantHue must be in the range between 0 and 360.
    /// The default value is null.
    /// </summary>
    public int? InvariantHue
    {
      get => (int?)GetValue(InvariantHueProperty);
      set => SetValue(InvariantHueProperty, value);
    }

    public static readonly DependencyProperty InvariantHueProperty = DependencyProperty.Register
      ("InvariantHue", typeof(int?), typeof(ColorPad),
        new FrameworkPropertyMetadata(null, BitmapParametersChanged));
    #endregion

    #region Gamma property
    /// <summary>
    /// Gamma property enables gamma correction in the whole area. 
    /// If Gamma is less than 1 then dark regions are brightened.
    /// If Gamma is greater than 1 then dark regions are even more dark.
    /// Gamma should be greater than 0.
    /// The default value is 1.
    /// </summary>
    public double Gamma
    {
      get => (double)GetValue(GammaProperty);
      set => SetValue(GammaProperty, value);
    }

    public static readonly DependencyProperty GammaProperty = DependencyProperty.Register
      ("Gamma", typeof(double), typeof(ColorPad),
        new FrameworkPropertyMetadata(1.0, BitmapParametersChanged));
    #endregion

    #region Resolution property
    public int Resolution
    {
      get => (int)GetValue(ResolutionProperty);
      set => SetValue(ResolutionProperty, value);
    }

    public static readonly DependencyProperty ResolutionProperty = DependencyProperty.Register
      ("Resolution", typeof(int), typeof(ColorPad),
        new FrameworkPropertyMetadata(100, BitmapParametersChanged));
    #endregion

    #region Bitmap generation

    private static void BitmapParametersChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      //Debug.WriteLine("ChangeBrushProperty");
      var c = (sender as ColorPad);
      c.bitmap=null;
      c.InvalidateVisual();
    }

    BitmapSource bitmap;

    private void CreateBitmap()
    {
      colorMap=null;
      var map = CreateColorMap(Resolution);
      int n = Resolution;
      var writeableBitmap = new WriteableBitmap(n, n, 96, 96, PixelFormats.Pbgra32, null);
      writeableBitmap.SetColorArray(map);
      if (n==360)
        colorMap = map;
      bitmap = writeableBitmap;
      InvalidateVisual();
    }

    private Color[,] CreateColorMap(int resolution)
    {
      AhsvColor hsv00 = Color00.ToDrawingColor().ToAhsv();
      AhsvColor hsv01 = Color01.ToDrawingColor().ToAhsv();
      AhsvColor hsv10 = Color10.ToDrawingColor().ToAhsv();
      AhsvColor hsv11 = Color11.ToDrawingColor().ToAhsv();
      Qhta.Drawing.HueChange hueChange = (Qhta.Drawing.HueChange)HueChange;
      if (HueChange==HueChange.None && InvariantHue!=null)
      {
        double h = (int)InvariantHue/360.0;
        hsv00.H=h;
        hsv01.H=h;
        hsv10.H=h;
        hsv00.H=h;
      }
      int n = resolution;
      AhsvColor[] leftEdge = new AhsvColor[n];
      int i = 0;
      foreach (var hsv in 
        new ColorIterator(hsv01, hsv00, Resolution, hueChange, Gamma)
        as IEnumerable<AhsvColor>)
      {
        //if (hsv.A==1)
        //  Debug.WriteLine($"leftEdge[{i}]=A={hsv.A} H={hsv.H} S={hsv.S} V={hsv.V}, {hsv.ToColor()}");
        leftEdge[i]=hsv;
        i++;
      }
      AhsvColor[] rightEdge = new AhsvColor[n];
      i = 0;
      foreach (var hsv 
        in new ColorIterator(hsv11, hsv10, Resolution, hueChange, Gamma)
        as IEnumerable<AhsvColor>)
      {
        //if (hsv.A==1)
        //  Debug.WriteLine($"rightEdge[{i}]=A={hsv.A} H={hsv.H} S={hsv.S} V={hsv.V}, {hsv.ToColor()}");
        rightEdge[i]=hsv;
        i++;
      }
      Color[,] result = new Color [n,n];
      for (int j=0; j<n; j++)
      {
        var hsv0 = leftEdge[j];
        var hsv1 = rightEdge[j];
        i = 0;
        foreach (var hsv in new ColorIterator(hsv0, hsv1, Resolution, hueChange, Gamma)
                 as IEnumerable<AhsvColor>)
        {
          if (hsv.A==1)
          {
            //if (j==0)
            //  Debug.WriteLine($"topEdge[{i}]=A={hsv.A} H={hsv.H} S={hsv.S} V={hsv.V}, {hsv.ToColor()}");
            //if (j==n-1)
            //  Debug.WriteLine($"bottomEdge[{i}]=A={hsv.A} H={hsv.H} S={hsv.S} V={hsv.V}, {hsv.ToColor()}");
          }
          result[i,j] = hsv.ToColor().ToMediaColor();
          i++;
        }
      }
      return result;
    }

    #endregion

    public event RoutedPropertyChangedEventHandler<Point> ValueChanged;

    #region Position property
    /// <summary>
    /// Relative position of pointer over ColorPad.
    /// Range from 0.0 (left-bottom corner) to 1.0 (right-top corner)
    /// </summary>
    public Point Position
    {
      get => (Point)GetValue(PositionProperty);
      set => SetValue(PositionProperty, value);
    }

    public static readonly DependencyProperty PositionProperty = DependencyProperty.Register
      ("Position", typeof(Point), typeof(ColorPad),
        new FrameworkPropertyMetadata(new Point(0.0,0.0), PositionPropertyChanged));

    private static void PositionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c = (sender as ColorPad);
      c.PositionChanged((Point)args.OldValue, (Point)args.NewValue);
    }

    bool isPositionChanged;
    private void PositionChanged(Point oldValue, Point newValue)
    {
      isPositionChanged=true;
      SelectedColor = Position2Color(Position);
      InvalidateVisual();
      ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<Point>(oldValue, newValue));
      isPositionChanged=false;
    }

    #endregion

    private readonly Pen BackPen = new Pen(Brushes.White, 4);
    private readonly Pen FrontPen = new Pen(Brushes.Black, 1);

    protected override void OnRender(DrawingContext drawingContext)
    {
      //Debug.WriteLine("OnRender");
      if (bitmap==null)
        CreateBitmap();
      double top;
      double left;
      double width;
      double height;
      left = ActualWidth*Position.X-4; 
      top = ActualHeight*(1-Position.Y)-4;
      height = 8;
      width = 8;
      //drawingContext.DrawRectangle(Brush, null, new Rect(0, 0, ActualWidth, ActualHeight));
      drawingContext.DrawImage(bitmap, new Rect(0, 0, ActualWidth, ActualHeight));
      drawingContext.DrawRectangle(null, BackPen, new Rect(left, top, width, height));
      drawingContext.DrawRectangle(null, FrontPen, new Rect(left, top, width, height));
      //Debug.WriteLine("OnRender end");
    }

    #region MouseMove handling
    private bool isMouseDown;

    private void ColorPad_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs args)
    {
      isMouseDown=true;
      CaptureMouse();
      ColorPad_MouseMove(sender, args);
    }

    private void ColorPad_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs args)
    {
      isMouseDown=false;
      ReleaseMouseCapture();
    }

    private void ColorPad_MouseMove(object sender, System.Windows.Input.MouseEventArgs args)
    {
      if (isMouseDown)
      {
        var pos = args.GetPosition(this);
          Position = new Point(
                        Math.Max(Math.Min(pos.X/this.ActualWidth, 1), 0),
                        Math.Max(Math.Min(1-pos.Y/this.ActualHeight, 1), 0));
        SelectedColor = Position2Color(Position);
      }
    }
    #endregion

    #region Color selection

    public void SetPosition(Color color)
    {
      Position = Color2Position(color);
    }

    Color[,] colorMap;

    public Point Color2Position(Color color)
    {
      if (color==Color00)
        return new Point(0,0);
      if (color==Color01)
        return new Point(0, 1);
      if (color==Color10)
        return new Point(1, 0);
      if (color==Color11)
        return new Point(1,1);
      var n = Resolution;
      if (colorMap==null)
        colorMap = CreateColorMap(n);
      double minDiff = double.MaxValue;
      Point result = new Point(double.NaN, double.NaN);
      for (int j = 0; j<n; j++)
        for (int i = 0; i<n; i++)
      {
        Color pixelColor;
        pixelColor=colorMap[i,j];
        var diff = ColorUtils.Distance(pixelColor, color);
        if (diff<minDiff)
        {
          minDiff=diff;
          result=new Point(i/(double)n, (n-j-1)/(double)n);
        }
      }
      return result;
    }

    public Color Position2Color(Point value)
    {
      if (value.X==0 && value.Y==0)
        return Color00;
      if (value.X==0 && value.Y==1)
        return Color01;
      if (value.X==1 && value.Y==0)
        return Color10;
      if (value.X==1 && value.Y==1)
        return Color11;
      var n = Resolution;
      if (colorMap==null)
        colorMap = CreateColorMap(n);
      Color result = Colors.Transparent;
      int i = Math.Max(Math.Min((int)(n * value.X), n-1), 0);
      int j = Math.Max(Math.Min((int)(n * value.Y), n-1), 0);
      result=colorMap[i,n-j-1];
      return result;
    }

    #endregion
  }
}

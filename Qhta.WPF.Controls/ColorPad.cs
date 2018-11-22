using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Qhta.Drawing;

namespace Qhta.WPF.Controls
{
 
  public class ColorPad: Control
  {
    public ColorPad()
    {
      this.DefaultStyleKey = typeof(ColorPad);
    }


    private Canvas BackCanvas;
    private Shape PosPointer;

    public override void OnApplyTemplate()
    {
      BackCanvas = Template.FindName("BackCanvas", this) as Canvas;
      PosPointer = Template.FindName("PosPointer", this) as Shape;
      PositionChanged();
      this.PreviewMouseLeftButtonDown+=ColorPad_MouseLeftButtonDown;
      this.MouseLeftButtonUp+=ColorPad_MouseLeftButtonUp;
      this.MouseMove+=ColorPad_MouseMove;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      var result = base.ArrangeOverride(finalSize);
      PositionChanged();
      return result;
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      base.OnRenderSizeChanged(sizeInfo);
      PositionChanged();
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
      (sender as ColorPad).SelectedColorChanged();
    }

    private void SelectedColorChanged()
    {
      if (!isMouseDown)
      {
        SelectColor(SelectedColor);
        //Debug.WriteLine($"SelectedColorChanged({SelectedColor})");
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
        new FrameworkPropertyMetadata(Colors.Transparent, ChangeBrushProperty));
    #endregion

    #region Color01 property
    public Color Color01
    {
      get => (Color)GetValue(Color01Property);
      set => SetValue(Color01Property, value);
    }

    public static readonly DependencyProperty Color01Property = DependencyProperty.Register
      ("Color01", typeof(Color), typeof(ColorPad),
        new FrameworkPropertyMetadata(Colors.Transparent, ChangeBrushProperty));
    #endregion

    #region Color10 property
    public Color Color10
    {
      get => (Color)GetValue(Color10Property);
      set => SetValue(Color10Property, value);
    }

    public static readonly DependencyProperty Color10Property = DependencyProperty.Register
      ("Color10", typeof(Color), typeof(ColorPad),
        new FrameworkPropertyMetadata(Colors.Transparent, ChangeBrushProperty));
    #endregion

    #region Color11 property
    public Color Color11
    {
      get => (Color)GetValue(Color11Property);
      set => SetValue(Color11Property, value);
    }

    public static readonly DependencyProperty Color11Property = DependencyProperty.Register
      ("Color11", typeof(Color), typeof(ColorPad),
        new FrameworkPropertyMetadata(Colors.Transparent, ChangeBrushProperty));
    #endregion

    #region HueChangeX property
    public HueGradient HueChangeX
    {
      get => (HueGradient)GetValue(HueChangeXProperty);
      set => SetValue(HueChangeXProperty, value);
    }

    public static readonly DependencyProperty HueChangeXProperty = DependencyProperty.Register
      ("HueChangeX", typeof(HueGradient), typeof(ColorPad),
        new FrameworkPropertyMetadata(HueGradient.None, ChangeBrushProperty));
    #endregion

    #region HueChangeY property
    public HueGradient HueChangeY
    {
      get => (HueGradient)GetValue(HueChangeYProperty);
      set => SetValue(HueChangeYProperty, value);
    }

    public static readonly DependencyProperty HueChangeYProperty = DependencyProperty.Register
      ("HueChangeY", typeof(HueGradient), typeof(ColorPad),
        new FrameworkPropertyMetadata(HueGradient.None, ChangeBrushProperty));
    #endregion

    #region ResolutionX property
    public int ResolutionX
    {
      get => (int)GetValue(ResolutionXProperty);
      set => SetValue(ResolutionXProperty, value);
    }

    public static readonly DependencyProperty ResolutionXProperty = DependencyProperty.Register
      ("ResolutionX", typeof(int), typeof(ColorPad),
        new FrameworkPropertyMetadata(100, ChangeBrushProperty));
    #endregion

    #region ResolutionX property
    public int ResolutionY
    {
      get => (int)GetValue(ResolutionYProperty);
      set => SetValue(ResolutionYProperty, value);
    }

    public static readonly DependencyProperty ResolutionYProperty = DependencyProperty.Register
      ("ResolutionY", typeof(int), typeof(ColorPad),
        new FrameworkPropertyMetadata(100, ChangeBrushProperty));
    #endregion

    #region Brush
    protected Brush Brush { get; private set; } = new SolidColorBrush(Colors.Transparent);

    private static void ChangeBrushProperty(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as ColorPad).ChangeBrush();
    }

    PixelArray pixels;

    private void ChangeBrush()
    {
      int n = ResolutionX;
      pixels = new PixelArray(n, n);
      var color00 = System.Drawing.Color.FromArgb(Color00.A, Color00.R, Color00.G, Color00.B);
      var color01 = System.Drawing.Color.FromArgb(Color01.A, Color01.R, Color01.G, Color01.B);
      var color10 = System.Drawing.Color.FromArgb(Color01.A, Color10.R, Color10.G, Color10.B);
      var color11 = System.Drawing.Color.FromArgb(Color11.A, Color11.R, Color11.G, Color11.B);
      var hsv00 = Qhta.Drawing.ColorSpaceConverter.Color2HSV(color00);
      var hsv01 = Qhta.Drawing.ColorSpaceConverter.Color2HSV(color01);
      var hsv10 = Qhta.Drawing.ColorSpaceConverter.Color2HSV(color10);
      var hsv11 = Qhta.Drawing.ColorSpaceConverter.Color2HSV(color11);
      var hueLeftDelta = Math.Abs(((hsv01.H-hsv00.H)/n);
      if (hueLeftDelta==0)
          hueLeftDelta = 1.0/n;
        if (HueChangeY==HueGradient.Negative)
          hueYDelta = -hueYDelta;
        var sat00Start = start00HSV.S;
        var sat01End = end01HSV.S;
        var satYDelta = (satEnd-satStart)/n;
        var valStart = start00HSV.V;
        var valEnd = end01HSV.V;
        var valDelta = (valEnd-valStart)/n;
        var alpStart = (Color00.A/255.0);
        var alpEnd = (Color11.A/255.0);
        var alpDelta = (alpEnd-alpStart)/n;
        var xDelta = 1.0/n;
        for (int x = 0; x<n; x++)
        {
          var hue = (hueStart+x*hueDelta) % 1.0;
          if (hue<0)
            hue+=1.0;
          var sat = satStart+x*satDelta;
          var val = valStart+x*valDelta;
          var alp = alpStart+x*alpDelta;
          var newColor = Qhta.Drawing.ColorSpaceConverter.HSV2Color(new ColorHSV(hue, sat, val));
          int k = i;
          stops[k]=(new GradientStop(Color.FromArgb((byte)(alp*255), newColor.R, newColor.G, newColor.B), xPos));
        }
      DrawingVisual drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();
      drawingContext.DrawRectangle(Brush, null, new Rect(0, 0, 101, 101));
      drawingContext.Close();
      var bmp = new RenderTargetBitmap(101, 101, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(drawingVisual);
       = bmp.GetPixelArray();
      Brush = new BitmapCacheBrush();
      Background=Brush;
      InvalidateVisual();
    }
    #endregion

    #region PositionX property
    public double PositionX
    {
      get => (double)GetValue(PositionXProperty);
      set => SetValue(PositionXProperty, value);
    }

    public static readonly DependencyProperty PositionXProperty = DependencyProperty.Register
      ("PositionX", typeof(double), typeof(ColorPad),
        new FrameworkPropertyMetadata(0.0,
        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
          PositionXPropertyChanged));

    private static void PositionXPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as ColorPad).PositionChanged();
    }
    #endregion

    #region PositionY property
    public double PositionY
    {
      get => (double)GetValue(PositionYProperty);
      set => SetValue(PositionYProperty, value);
    }

    public static readonly DependencyProperty PositionYProperty = DependencyProperty.Register
      ("PositionY", typeof(double), typeof(ColorPad),
        new FrameworkPropertyMetadata(0.0,
        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
          PositionYPropertyChanged));

    private static void PositionYPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as ColorPad).PositionChanged();
    }
    #endregion
    private void PositionChanged()
    {
      if (!isMouseDown)
      {
        SelectColor(PositionX, PositionY);
        //Debug.WriteLine($"PositionXChanged({PositionX})");
      }
    }
    #endregion

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
        var pos = args.GetPosition(BackCanvas);
        Position = Math.Max(Math.Min(pos.X/BackCanvas.ActualWidth, 1), 0);
        //Debug.WriteLine($"Value = {Value}");
        Color color;
        SelectedColor = color = Value2Color(Value);
        //Debug.WriteLine($"Scolor = {color}");
        Debug.WriteLine($"SelectedColor = {SelectedColor}");
      }
    }
    #endregion

    #region Color selection

    public void SelectColor(Color color)
    {
      //Debug.WriteLine($"SelectColor({color})");
      Value=Color2Value(color);
      //Debug.WriteLine($"SelectedValue({Value})");
    }

    public double Color2Value(Color color)
    {
      if (color==Color00)
        return 0;
      if (color==Color11)
        return 0;
      DrawingVisual drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();
      drawingContext.DrawRectangle(Brush, null, new Rect(0,0,101,101));
      drawingContext.Close();

      var bmp = new RenderTargetBitmap(101, 101, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(drawingVisual);
      PixelArray pixels = bmp.GetPixelArray();
      double minDiff = double.MaxValue;
      double result = double.NaN;
      for (int i=0; i<=100; i++)
      {
        Pixel pixel = pixel=pixels[i, 0];
        var pixelColor = Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B);
        var diff = ColorDistance(pixelColor, color);
        if (diff<minDiff)
        {
          minDiff=diff;
          result=i/100.0;
        }
      }
      return result;
    }

    public Color Value2Color(double x, double y)
    {
      if (x==0 && y==0)
        return Color00;
      if (x==1)
        return Color11;
      DrawingVisual drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();
      drawingContext.DrawRectangle(Brush, null, new Rect(0, 0, 101, 101));
      drawingContext.Close();
      var bmp = new RenderTargetBitmap(101, 101, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(drawingVisual);
      PixelArray pixels = bmp.GetPixelArray();
      Color result = Colors.Transparent;
      int i = Math.Max(Math.Min((int)(100 * x), 100), 0);
      Pixel pixel = pixel=pixels[i, 0];
      result = Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B);
      return result;
    }

    public static double ColorDistance(Color Color11, Color color2)
    {
      var A1 = Color11.A/255.0;
      var R1 = Color11.R/255.0;
      var G1 = Color11.G/255.0;
      var B1 = Color11.B/255.0;
      var A2 = color2.A/255.0;
      var R2 = color2.R/255.0;
      var G2 = color2.G/255.0;
      var B2 = color2.B/255.0;
      var diff = Math.Sqrt((A1-A2)*(A1-A2) + (R1-R2)*(R1-R2) + (G1-G2)*(G1-G2) + (B1-B2)*(B1-B2));
      return diff;
    }

    #endregion
  }
}

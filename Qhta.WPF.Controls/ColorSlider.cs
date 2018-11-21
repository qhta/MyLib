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
  public enum HueGradient
  {
    None,
    Positive,
    Negative
  }

  public class ColorSlider: Slider
  {
    public ColorSlider()
    {
      this.DefaultStyleKey = typeof(ColorSlider);
    }

    static ColorSlider()
    {
      Slider.OrientationProperty.OverrideMetadata(typeof(ColorSlider), 
        new FrameworkPropertyMetadata(Orientation.Horizontal,ChangeBrushProperty));
      Slider.ValueProperty.OverrideMetadata(typeof(ColorSlider),
        new FrameworkPropertyMetadata(0.0, ValuePropertyChanged));
    }

    private Canvas BackCanvas;
    private Shape PosPointer;

    public override void OnApplyTemplate()
    {
      BackCanvas = Template.FindName("BackCanvas", this) as Canvas;
      PosPointer = Template.FindName("PosPointer", this) as Shape;
      PositionChanged();
      this.PreviewMouseLeftButtonDown+=ColorSlider_MouseLeftButtonDown;
      this.MouseLeftButtonUp+=ColorSlider_MouseLeftButtonUp;
      this.MouseMove+=ColorSlider_MouseMove;
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
      ("SelectedColor", typeof(Color), typeof(ColorSlider),
        new FrameworkPropertyMetadata(Colors.Transparent,
        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
          SelectedColorPropertyChanged));

    private static void SelectedColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as ColorSlider).SelectedColorChanged();
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

    #region ColorFrom property
    public Color ColorFrom
    {
      get => (Color)GetValue(ColorFromProperty);
      set => SetValue(ColorFromProperty, value);
    }

    public static readonly DependencyProperty ColorFromProperty = DependencyProperty.Register
      ("ColorFrom", typeof(Color), typeof(ColorSlider),
        new FrameworkPropertyMetadata(Colors.Transparent, ChangeBrushProperty));
    #endregion

    #region ColorTo property
    public Color ColorTo
    {
      get => (Color)GetValue(ColorToProperty);
      set => SetValue(ColorToProperty, value);
    }

    public static readonly DependencyProperty ColorToProperty = DependencyProperty.Register
      ("ColorTo", typeof(Color), typeof(ColorSlider),
        new FrameworkPropertyMetadata(Colors.Transparent, ChangeBrushProperty));
    #endregion

    #region HueChange property
    public HueGradient HueChange
    {
      get => (HueGradient)GetValue(HueChangeProperty);
      set => SetValue(HueChangeProperty, value);
    }

    public static readonly DependencyProperty HueChangeProperty = DependencyProperty.Register
      ("HueChange", typeof(HueGradient), typeof(ColorSlider),
        new FrameworkPropertyMetadata(HueGradient.None, ChangeBrushProperty));
    #endregion

    #region Stripes property
    public int Stripes
    {
      get => (int)GetValue(StripesProperty);
      set => SetValue(StripesProperty, value);
    }

    public static readonly DependencyProperty StripesProperty = DependencyProperty.Register
      ("Stripes", typeof(int), typeof(ColorSlider),
        new FrameworkPropertyMetadata(20, ChangeBrushProperty));
    #endregion

    #region Brush
    protected Brush Brush { get; private set; } = new SolidColorBrush(Colors.Transparent);

    private static void ChangeBrushProperty(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as ColorSlider).ChangeBrush();
    }

    private void ChangeBrush()
    {
      if (HueChange!=HueGradient.None)
      {
        var startColor = System.Drawing.Color.FromArgb(ColorFrom.A, ColorFrom.R, ColorFrom.G, ColorFrom.B);
        var endColor = System.Drawing.Color.FromArgb(ColorTo.A, ColorTo.R, ColorTo.G, ColorTo.B);
        var startHSV = Qhta.Drawing.ColorSpaceConverter.Color2HSV(startColor);
        var endHSV = Qhta.Drawing.ColorSpaceConverter.Color2HSV(endColor);
        int n = Stripes;
        var hueStart = startHSV.H;
        var hueEnd = endHSV.H;
        var hueDelta = Math.Abs((hueEnd-hueStart)/n);
        if (hueDelta==0)
          hueDelta = 1.0/n;
        if (HueChange==HueGradient.Negative)
          hueDelta = -hueDelta;
        var satStart = startHSV.S;
        var satEnd = endHSV.S;
        var satDelta = (satEnd-satStart)/n;
        var valStart = startHSV.V;
        var valEnd = endHSV.V;
        var valDelta = (valEnd-valStart)/n;
        var alpStart = (ColorFrom.A/255.0);
        var alpEnd = (ColorTo.A/255.0);
        var alpDelta = (alpEnd-alpStart)/n;
        var xDelta = 1.0/n;
        GradientStop[] stops = new GradientStop[n];
        for (int i = 0; i<n; i++)
        {
          var hue = (hueStart+i*hueDelta) % 1.0;
          if (hue<0)
            hue+=1.0;
          var sat = satStart+i*satDelta;
          var val = valStart+i*valDelta;
          var alp = alpStart+i*alpDelta;
          var newColor = Qhta.Drawing.ColorSpaceConverter.HSV2Color(new ColorHSV(hue, sat, val));
          int k = i;
          double xPos = i*xDelta;
          if (Orientation==Orientation.Vertical)
          {
            k = n-i-1;
            xPos = 1-xPos;
          }
          stops[k]=(new GradientStop(Color.FromArgb((byte)(alp*255), newColor.R, newColor.G, newColor.B), xPos));
        }
        Brush = new LinearGradientBrush(new GradientStopCollection(stops),
          Orientation==Orientation.Horizontal ? 0 : 90);
      }
      else
      {
        var colorStart = ColorFrom;
        var colorEnd = ColorTo;
        if (Orientation==Orientation.Vertical)
        {
          colorStart = ColorTo;
          colorEnd = ColorFrom;
        }
        Brush = new LinearGradientBrush(colorStart, colorEnd, Orientation==Orientation.Horizontal ? 0 : 90);
      }
      Background=Brush;
      InvalidateVisual();
    }
    #endregion

    #region Position property
    private static void ValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as ColorSlider).PositionChanged();
    }

    private void ChangePosPointerSize()
    {
      if (PosPointer!=null)
      {
        if (Orientation==Orientation.Vertical)
        {
          PosPointer.Height=6;
          PosPointer.Width=BackCanvas?.ActualWidth ?? this.ActualWidth;
        }
        else
        {
          PosPointer.Width=6;
          PosPointer.Height=BackCanvas?.ActualHeight ?? this.ActualHeight;
        }
        //Debug.WriteLine($"PosPointer.Width={PosPointer.Width}");
        //Debug.WriteLine($"PosPointer.Height={PosPointer.Height}");
      }
    }

    private void PositionChanged()
    {
      if (PosPointer!=null)
      {
        ChangePosPointerSize();
        if (Orientation==Orientation.Vertical)
        {
          Canvas.SetTop(PosPointer, ActualHeight*(1-Value)-4);
          Canvas.SetLeft(PosPointer, 0);
        }
        else
        {
          Canvas.SetLeft(PosPointer, ActualWidth*Value-4);
          Canvas.SetTop(PosPointer, 0);
        }
        //Debug.WriteLine($"PosPointer.Left={Canvas.GetLeft(PosPointer)}");
        //Debug.WriteLine($"PosPointer.Top={Canvas.GetTop(PosPointer)}");
      }
    }
    #endregion

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
        var pos = args.GetPosition(BackCanvas);
        if (Orientation==Orientation.Vertical)
        {
          Value = Math.Max(Math.Min(1-pos.Y/BackCanvas.ActualHeight, 1), 0);
        }
        else
        {
          Value = Math.Max(Math.Min(pos.X/BackCanvas.ActualWidth, 1), 0);
        }
        Debug.WriteLine($"Value = {Value}");
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
      if (color==ColorFrom)
        return 0;
      if (color==ColorTo)
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
        Pixel pixel;
        if (Orientation==Orientation.Vertical)
          pixel=pixels[0, 100-i];
        else
          pixel=pixels[i, 0];
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

    public Color Value2Color(double value)
    {
      if (value==0)
        return ColorFrom;
      if (value==1)
        return ColorTo;
      DrawingVisual drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();
      drawingContext.DrawRectangle(Brush, null, new Rect(0, 0, 101, 101));
      drawingContext.Close();
      var bmp = new RenderTargetBitmap(101, 101, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(drawingVisual);
      PixelArray pixels = bmp.GetPixelArray();
      Color result = Colors.Transparent;
      int i = Math.Max(Math.Min((int)(100 * value), 100), 0);
      Pixel pixel;
      if (Orientation==Orientation.Vertical)
        pixel=pixels[0, 100-i];
      else
        pixel=pixels[i, 0];
      result = Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B);
      return result;
    }

    private double ColorDistance(Color color1, Color color2)
    {
      var A1 = color1.A/255.0;
      var R1 = color1.R/255.0;
      var G1 = color1.G/255.0;
      var B1 = color1.B/255.0;
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

using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Qhta.Drawing;
using Qhta.WPF.Utils;

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
      SelectedColor = Position2Color(Position);
    }

    private static void ValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as ColorSlider).Position=(double)args.NewValue;
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
      c.SelectedColorChanged();
      c.ValueChanged?.Invoke(c, new RoutedPropertyChangedEventArgs<double>(c.Position, c.Position));
    }

    private void SelectedColorChanged()
    {
      if (!isMouseDown)
      {
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
    public HueGradient HueChange
    {
      get => (HueGradient)GetValue(HueChangeProperty);
      set => SetValue(HueChangeProperty, value);
    }

    public static readonly DependencyProperty HueChangeProperty = DependencyProperty.Register
      ("HueChange", typeof(HueGradient), typeof(ColorSlider),
        new FrameworkPropertyMetadata(HueGradient.None, ChangeBrushProperty));
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
    protected Brush Brush { get; private set; } = new SolidColorBrush(Colors.Transparent);

    private static void ChangeBrushProperty(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c = (sender as ColorSlider);
      c.ChangeBrush();
    }

    private void ChangeBrush()
    {
      if (HueChange!=HueGradient.None)
      {
        int n = Resolution;
        var xDelta = 1.0/n;
        GradientStop[] stops = new GradientStop[n];
        int i = 0;
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
      }
      else
      {
        var colorStart = Color0;
        var colorEnd = Color1;
        if (Orientation==Orientation.Vertical)
        {
          colorStart = Color1;
          colorEnd = Color0;
        }
        Brush = new LinearGradientBrush(colorStart, colorEnd, Orientation==Orientation.Horizontal ? 0 : 90);
      }
      //Background=Brush;
      InvalidateVisual();
    }
    #endregion

    public event RoutedPropertyChangedEventHandler<double> ValueChanged;

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
      c.PositionChanged((double)args.OldValue, (double)args.NewValue);

    }

    private void PositionChanged(double oldValue, double newValue)
    {
      SelectedColor = Position2Color(Position);
      InvalidateVisual();
      ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
    }

    #endregion

    private readonly Pen BackPen = new Pen(Brushes.White, 4);
    private readonly Pen Pen = new Pen(Brushes.Black, 1);

    protected override void OnRender(DrawingContext drawingContext)
    {
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
      //var pColor = SelectedColor.Inverse();
      //Pen Pen = new Pen(new SolidColorBrush(pColor), 1);
      drawingContext.DrawRectangle(null, BackPen, new Rect(left, top, width, height));
      drawingContext.DrawRectangle(null, Pen, new Rect(left, top, width, height));
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
        SelectedColor = Position2Color(Position);
      }
    }
    #endregion

    #region Color selection

    public void SetPosition(Color color)
    {
      Position = Color2Position(color);
    }

    public double Color2Position(Color color)
    {
      if (color==Color0)
        return 0;
      if (color==Color1)
        return 1;
      var resolution = 360;
      Color[] colorArray = (new ColorIterator(Color0, Color1, resolution, HueChange)).ToArray();
      double minDiff = double.MaxValue;
      double result = double.NaN;
      for (int i=0; i<resolution; i++)
      {
        Color pixelColor;
        pixelColor=colorArray[i]; 
        var diff = ColorUtils.Distance(pixelColor, color);
        if (diff<minDiff)
        {
          minDiff=diff;
          result=i/(double)resolution;
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
      var resolution = 360;
      Color[] colorArray = (new ColorIterator(Color0, Color1, resolution, HueChange)).ToArray();
      Color result = Colors.Transparent;
      int i = Math.Max(Math.Min((int)(360 * value), 360), 0);
      result=colorArray[i];
      return result;
    }

    #endregion
  }
}

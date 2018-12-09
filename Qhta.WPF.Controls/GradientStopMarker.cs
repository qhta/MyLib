using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Qhta.WPF.Controls
{
  public partial class GradientStopMarker : FrameworkElement, IComparable<GradientStopMarker>
  {
    public GradientStopMarker()
    {
      DataContextChanged+=GradientStopMarker_DataContextChanged;
    }

    private void GradientStopMarker_DataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
      //if (args.NewValue is GradientStop gradientStop)
      //{
      //  SetBinding(ColorProperty, new Binding("Color") { Source=gradientStop, Mode=BindingMode.TwoWay });
      //  SetBinding(OffsetProperty, new Binding("Offset") { Source=gradientStop, Mode=BindingMode.TwoWay });
      //}
    }


    #region Color property
    public Color Color
    {
      get => (Color)GetValue(ColorProperty);
      set => SetValue(ColorProperty, value);
    }

    public static DependencyProperty ColorProperty = DependencyProperty.Register
      ("Color", typeof(Color), typeof(GradientStopMarker),
        new FrameworkPropertyMetadata(Colors.Transparent,
            FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region IsSelected property
    public bool IsSelected
    {
      get => (bool)GetValue(IsSelectedProperty);
      set => SetValue(IsSelectedProperty, value);
    }

    public static DependencyProperty IsSelectedProperty = DependencyProperty.Register
      ("IsSelected", typeof(bool), typeof(GradientStopMarker),
        new FrameworkPropertyMetadata(false,
            FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region Offset property
    public double Offset
    {
      get => (double)GetValue(OffsetProperty);
      set => SetValue(OffsetProperty, value);
    }

    public static DependencyProperty OffsetProperty = DependencyProperty.Register
      ("Offset", typeof(double), typeof(GradientStopMarker),
        new FrameworkPropertyMetadata(double.NaN,
          FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      var result = base.ArrangeOverride(arrangeBounds);
      pos = result.Width*Offset;
      return result;
    }

    double pos;

    protected override void OnRender(DrawingContext drawingContext)
    {
      if (Parent is FrameworkElement parent)
      {
        var brush = new SolidColorBrush(Color);
        var pen = new Pen(Brushes.Black, 1);
        drawingContext.DrawRectangle(brush, pen, new Rect(pos-8, 0, 16, 16));
        brush = new SolidColorBrush(IsSelected ? Colors.Black : Colors.White);
        var path = (Geometry)new GeometryConverter().ConvertFromInvariantString("M3,3L13,3L8,10Z");
        drawingContext.PushTransform(new TranslateTransform(pos-8, 16));
        drawingContext.DrawGeometry(brush, pen, path);
        drawingContext.Pop();
      }
    }

    public int CompareTo(GradientStopMarker other)
    {
      return this.Offset.CompareTo(other.Offset);
    }
  }
}

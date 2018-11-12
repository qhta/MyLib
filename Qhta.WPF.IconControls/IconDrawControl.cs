using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Qhta.WPF.IconDefinition;
using Qhta.WPF.Utils;

namespace Qhta.WPF.IconControls
{
  public class IconDrawControl : Control
  {
    #region IconDef property
    public IconDef IconDef
    {
      get => (IconDef)GetValue(IconDefProperty);
      set => SetValue(IconDefProperty, value);
    }

    public static readonly DependencyProperty IconDefProperty = DependencyProperty.Register
      ("IconDef", typeof(IconDef), typeof(IconDrawControl),
        new FrameworkPropertyMetadata(null, 
          FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
          IconDefPropertyChanged)
      );

    private static void IconDefPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as IconDrawControl).IconDefChanged();
    }

    private void IconDefChanged()
    {
      if (IconDef!=null)
        IconDef.Invalidated+=IconDef_Invalidated;
    }

    private void IconDef_Invalidated(object sender, System.EventArgs e)
    {
      this.InvalidateVisual();
    }
    #endregion

    #region IconDrawMode property
    public IconDrawMode IconDrawMode
    {
      get => (IconDrawMode)GetValue(IconDrawModeProperty);
      set => SetValue(IconDrawModeProperty, value);
    }

    public static readonly DependencyProperty IconDrawModeProperty = DependencyProperty.Register
      ("IconDrawMode", typeof(IconDrawMode), typeof(IconDrawControl),
        new FrameworkPropertyMetadata(IconDrawMode.Normal, FrameworkPropertyMetadataOptions.AffectsRender)
      );
    #endregion

    #region SketchColor property
    public Color SketchColor
    {
      get => (Color)GetValue(SketchColorProperty);
      set => SetValue(SketchColorProperty, value);
    }
    public static readonly DependencyProperty SketchColorProperty = DependencyProperty.Register
      ("SketchColor", typeof(Color), typeof(IconDrawControl),
       new FrameworkPropertyMetadata(Colors.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region SketchThickness property
    [TypeConverter(typeof(LengthConverter))]
    public double SketchThickness
    {
      get =>
          ((double)base.GetValue(SketchThicknessProperty));
      set =>
          base.SetValue(SketchThicknessProperty, value);
    }
    public static readonly DependencyProperty SketchThicknessProperty = DependencyProperty.Register
      ("SketchThickness", typeof(double), typeof(IconDrawControl), 
      new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region SketchStyle property
    [TypeConverter(typeof(DashStyleConverter))]
    public DashStyle SketchStyle
    {
      get => (DashStyle)GetValue(SketchStyleProperty);
      set => SetValue(SketchStyleProperty, value);
    }
    public static readonly DependencyProperty SketchStyleProperty = DependencyProperty.Register
      ("SketchStyle", typeof(DashStyle), typeof(IconDrawControl),
       new FrameworkPropertyMetadata(DashStyles.Solid, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    protected override void OnRender(DrawingContext drawingContext)
    {
      if (IconDef==null)
        return;
      if (IconDef.Drawing==null)
        return;
      var scaleTransform = new ScaleTransform(ActualWidth/IconDef.Drawing.Width, ActualHeight/IconDef.Drawing.Height);
      switch (IconDrawMode)
      {
        case IconDrawMode.Normal:
          drawingContext.PushTransform(scaleTransform);
          IconDef.Drawing.Draw(drawingContext);
          drawingContext.Pop();
          break;
        case IconDrawMode.Sketch:
          var geometry = IconDef.Drawing.GetGeometry();
          geometry.Transform = scaleTransform;
          var pen = new Pen(new SolidColorBrush(SketchColor), SketchThickness);
          pen.DashStyle = SketchStyle;
          drawingContext.DrawGeometry(null, pen, geometry);
          break;
      }
    }
  }
}

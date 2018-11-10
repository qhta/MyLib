using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Windows.Data;
using Qhta.WPF.Utils;

namespace Qhta.WPF.Controls
{
  public class GridControl: Grid
  {

    #region Stroke property
    /// <summary>
    /// Brush (color) of grid lines
    /// </summary>
    public Brush Stroke
    {
      get => (Brush)GetValue(StrokeProperty);
      set => SetValue(StrokeProperty, value);
    }
    public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register
      ("Stroke", typeof(Brush), typeof(GridControl),
       new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region StrokeThickness property
    /// <summary>
    /// Thickness (width) or grid lines
    /// </summary>
    [TypeConverter(typeof(LengthConverter))]
    public double StrokeThickness
    {
      get =>
          ((double)base.GetValue(StrokeThicknessProperty));
      set =>
          base.SetValue(StrokeThicknessProperty, value);
    }

    public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register
      ("StrokeThickness", typeof(double), typeof(GridControl),
       new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region ResX
    /// <summary>
    /// How many vertical grid lines (one is added at the right boundary)
    /// </summary>
    public int ResX
    {
      get =>
          ((int)base.GetValue(ResXProperty));
      set =>
          base.SetValue(ResXProperty, value);
    }

    public static readonly DependencyProperty ResXProperty = DependencyProperty.Register
      ("ResX", typeof(int), typeof(GridControl),
      new FrameworkPropertyMetadata(8, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region ResY
    /// <summary>
    /// How many horizontal grid lines (one is added at the bottom boundary)
    /// </summary>
    public int ResY
    {
      get =>
          ((int)base.GetValue(ResYProperty));
      set =>
          base.SetValue(ResYProperty, value);
    }

    public static readonly DependencyProperty ResYProperty = DependencyProperty.Register
      ("ResY", typeof(int), typeof(GridControl),
      new FrameworkPropertyMetadata(8, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region TickX
    /// <summary>
    /// How many vertical grid are counted before thick line is drawn
    /// </summary>
    public int TickX
    {
      get =>
          ((int)base.GetValue(TickXProperty));
      set =>
          base.SetValue(TickXProperty, value);
    }

    public static readonly DependencyProperty TickXProperty = DependencyProperty.Register
      ("TickX", typeof(int), typeof(GridControl),
      new FrameworkPropertyMetadata(8, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region TickY
    /// <summary>
    /// How many horizontal grid are counted before thick line is drawn
    /// </summary>
    public int TickY
    {
      get =>
          ((int)base.GetValue(TickYProperty));
      set =>
          base.SetValue(TickYProperty, value);
    }

    public static readonly DependencyProperty TickYProperty = DependencyProperty.Register
      ("TickY", typeof(int), typeof(GridControl),
      new FrameworkPropertyMetadata(8, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region TickThickness property
    /// <summary>
    /// Thickness (width) of thick lines
    /// </summary>
    [TypeConverter(typeof(LengthConverter))]
    public double TickThickness
    {
      get =>
          ((double)base.GetValue(TickThicknessProperty));
      set =>
          base.SetValue(TickThicknessProperty, value);
    }

    public static readonly DependencyProperty TickThicknessProperty = DependencyProperty.Register
      ("TickThickness", typeof(double), typeof(GridControl),
       new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    public GridControl()
    {
      GridShape = new GridShape();
      Children.Add(GridShape);
      GridShape.SetBinding(Shape.StrokeProperty, new Binding("Stroke") { Source=this });
      GridShape.SetBinding(Shape.StrokeThicknessProperty, new Binding("StrokeThickness") { Source=this });
      GridShape.SetBinding(GridShape.ResXProperty, new Binding("ResX") { Source=this });
      GridShape.SetBinding(GridShape.ResYProperty, new Binding("ResY") { Source=this });

      TickShape = new GridShape();
      Children.Add(TickShape);
      TickShape.SetBinding(Shape.StrokeProperty, new Binding("Stroke") { Source=this });
      TickShape.SetBinding(Shape.StrokeThicknessProperty, new Binding("TickThickness") { Source=this });
      TickShape.SetBinding(GridShape.ResXProperty, new Binding("ResX") { Source=this,
        Converter=DividingConverter, ConverterParameter=TickX });
      TickShape.SetBinding(GridShape.ResYProperty, new Binding("ResY") { Source=this,
        Converter=DividingConverter, ConverterParameter=TickY });

    }

    protected GridShape GridShape { get; private set; }

    protected GridShape TickShape { get; private set; }

    protected DividingConverter DividingConverter { get; private set; } = new DividingConverter();
  }
}

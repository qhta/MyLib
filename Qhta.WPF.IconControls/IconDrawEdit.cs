using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Qhta.WPF.Controls;
using Qhta.WPF.IconDefinition;

namespace Qhta.WPF.IconControls
{
  public class IconDrawEdit: Grid
  {

    #region IconDef property
    public IconDef IconDef
    {
      get => (IconDef)GetValue(IconDefProperty);
      set => SetValue(IconDefProperty, value);
    }

    public static readonly DependencyProperty IconDefProperty = DependencyProperty.Register
      ("IconDef", typeof(IconDef), typeof(IconDrawEdit),
        new FrameworkPropertyMetadata(null,
          FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender)
      );
    #endregion

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
      ("Stroke", typeof(Brush), typeof(IconDrawEdit),
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
      ("StrokeThickness", typeof(double), typeof(IconDrawEdit),
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
      ("ResX", typeof(int), typeof(IconDrawEdit),
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
      ("ResY", typeof(int), typeof(IconDrawEdit),
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
      ("TickX", typeof(int), typeof(IconDrawEdit),
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
      ("TickY", typeof(int), typeof(IconDrawEdit),
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
      ("TickThickness", typeof(double), typeof(IconDrawEdit),
       new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    public IconDrawEdit()
    {
      IconDrawControl = new IconDrawControl();
      Children.Add(IconDrawControl);
      IconDrawControl.SetBinding(IconDrawControl.IconDefProperty, new Binding("IconDef") { Source=this });
      GridControl = new GridControl();
      Children.Add(GridControl);
      GridControl.SetBinding(GridControl.StrokeProperty, new Binding("Stroke") { Source=this });
      GridControl.SetBinding(GridControl.StrokeThicknessProperty, new Binding("StrokeThickness") { Source=this });
      GridControl.SetBinding(GridControl.ResXProperty, new Binding("ResX") { Source=this });
      GridControl.SetBinding(GridControl.ResYProperty, new Binding("ResY") { Source=this });
      GridControl.SetBinding(GridControl.TickThicknessProperty, new Binding("TickThickness") { Source=this });
      GridControl.SetBinding(GridControl.TickXProperty, new Binding("TickX") { Source=this });
      GridControl.SetBinding(GridControl.TickYProperty, new Binding("TickY") { Source=this });

    }

    protected IconDrawControl IconDrawControl { get; private set; }

    protected GridControl GridControl { get; private set; }

  }
}

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Qhta.WPF.Controls;
using Qhta.WPF.IconDefinition;
using Qhta.WPF.Utils;

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
          FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
          IconDefPropertyChanged
          )
      );

    private static void IconDefPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as IconDrawEdit).IconDefChanged();
    }

    private void IconDefChanged()
    {
      SketchControl.IconDef.Drawing.Items.Clear();
      if (IconDef!=null)
      {
        SketchControl.IconDef.Drawing.Width = IconDef.Drawing.Width;
        SketchControl.IconDef.Drawing.Height = IconDef.Drawing.Height;
        foreach (var item in IconDef.Drawing.Items)
        {
          SketchControl.IconDef.Drawing.Items.Add(item);
          break;
        }
      }
    }

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

    #region SketchColor property
    public Color SketchColor
    {
      get => (Color)GetValue(SketchColorProperty);
      set => SetValue(SketchColorProperty, value);
    }
    public static readonly DependencyProperty SketchColorProperty = DependencyProperty.Register
      ("SketchColor", typeof(Color), typeof(IconDrawEdit),
       new FrameworkPropertyMetadata(Colors.Gray, FrameworkPropertyMetadataOptions.AffectsRender));
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
      ("SketchThickness", typeof(double), typeof(IconDrawEdit),
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
      ("SketchStyle", typeof(DashStyle), typeof(IconDrawEdit),
       new FrameworkPropertyMetadata(DashStyles.Solid, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    public IconDrawEdit()
    {
      IconDrawControl = new IconDrawControl();
      Children.Add(IconDrawControl);
      IconDrawControl.SetBinding(IconDrawControl.IconDefProperty, new Binding(nameof(IconDef)) { Source=this });
      GridControl = new GridControl();
      Children.Add(GridControl);
      GridControl.SetBinding(GridControl.StrokeProperty, new Binding(nameof(Stroke)) { Source=this });
      GridControl.SetBinding(GridControl.StrokeThicknessProperty, new Binding(nameof(StrokeThickness)) { Source=this });
      GridControl.SetBinding(GridControl.ResXProperty, new Binding(nameof(ResX)) { Source=this });
      GridControl.SetBinding(GridControl.ResYProperty, new Binding(nameof(ResY)) { Source=this });
      GridControl.SetBinding(GridControl.TickThicknessProperty, new Binding(nameof(TickThickness)) { Source=this });
      GridControl.SetBinding(GridControl.TickXProperty, new Binding(nameof(TickX)) { Source=this });
      GridControl.SetBinding(GridControl.TickYProperty, new Binding(nameof(TickY)) { Source=this });
      SketchControl = new IconDrawControl();
      Children.Add(SketchControl);
      SketchControl.IconDrawMode = IconDrawMode.Sketch;
      //SketchControl.SetBinding(IconDrawControl.IconDefProperty, new Binding(nameof(IconDef)) { Source=this });
      SketchControl.IconDef = new IconDef { Drawing = new IconDefinition.Drawing() };
      //SketchControl.IconDef.Drawing.SetBinding(IconDefinition.Drawing.WidthProperty, new Binding("IconDef.Drawing.Width") { Source=this });
      //SketchControl.IconDef.Drawing.SetBinding(IconDefinition.Drawing.HeightProperty, new Binding("IconDef.Drawing.Height") { Source=this });
      SketchControl.SetBinding(IconDrawControl.SketchColorProperty, new Binding(nameof(SketchColor)) { Source=this });
      SketchControl.SetBinding(IconDrawControl.SketchThicknessProperty, new Binding(nameof(SketchThickness)) { Source=this });
      SketchControl.SetBinding(IconDrawControl.SketchStyleProperty, new Binding(nameof(SketchStyle)) { Source=this });
    }

    protected IconDrawControl IconDrawControl { get; private set; }

    protected GridControl GridControl { get; private set; }

    protected IconDrawControl SketchControl { get; private set; }

    public DrawingItemsCollection SelectedItems => SketchControl.IconDef.Drawing.Items;
  }
}

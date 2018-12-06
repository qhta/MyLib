using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Qhta.WPF.Controls
{
  public class ColorPickerDropDown: Control
  {
    public ColorPickerDropDown()
    {
      this.DefaultStyleKey=typeof(ColorPickerDropDown);
    }

    Button aButton;
    public override void OnApplyTemplate()
    {
      aButton = Template.FindName("aButton", this) as Button;
      if (aButton!=null)
        aButton.Click+=button_Click;
    }

    #region ProbeVisiblity property
    public double ButtonWidth
    {
      get => (double)GetValue(ButtonWidthProperty);
      set => SetValue(ButtonWidthProperty, value);
    }

    public static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.Register
      ("ButtonWidth", typeof(double), typeof(ColorPickerDropDown),
       new FrameworkPropertyMetadata(20.0,
         FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region ProbeVisibility property
    public Visibility ProbeVisibility
    {
      get => (Visibility)GetValue(ProbeVisibilityProperty);
      set => SetValue(ProbeVisibilityProperty, value);
    }

    public static readonly DependencyProperty ProbeVisibilityProperty = DependencyProperty.Register
      ("ProbeVisibility", typeof(Visibility), typeof(ColorPickerDropDown),
       new FrameworkPropertyMetadata(Visibility.Visible,
       FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region TextVisible property
    public bool TextVisible
    {
      get => (bool)GetValue(TextVisibleProperty);
      set => SetValue(TextVisibleProperty, value);
    }

    public static readonly DependencyProperty TextVisibleProperty = DependencyProperty.Register
      ("TextVisible", typeof(bool), typeof(ColorPickerDropDown),
       new FrameworkPropertyMetadata(true,
       FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region TextEditEnabled property
    public bool TextReadOnly
    {
      get => (bool)GetValue(TextReadOnlyProperty);
      set => SetValue(TextReadOnlyProperty, value);
    }

    public static readonly DependencyProperty TextReadOnlyProperty = DependencyProperty.Register
      ("TextReadOnly", typeof(bool), typeof(ColorPickerDropDown),
       new FrameworkPropertyMetadata(false,
       FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion
    public event Action<Color> SelectedColorChanged;

    #region SelectedColor property
    public Color SelectedColor
    {
      get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
      ("SelectedColor", typeof(Color), typeof(ColorPickerDropDown),
       new FrameworkPropertyMetadata(Colors.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    #endregion

    private void button_Click(object sender, RoutedEventArgs e)
    {
      var window = new ColorPickerWindow
      {
        SelectedColor = this.SelectedColor,
      };
      window.ShowDialog();
      if (window.SelectedColor!=this.SelectedColor)
      {
        this.SelectedColor = window.SelectedColor;
        SelectedColorChanged?.Invoke(SelectedColor);
      }
    }

  }
}

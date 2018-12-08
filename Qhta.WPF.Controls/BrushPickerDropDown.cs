using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Qhta.WPF.Controls
{
  public class BrushPickerDropDown: Control
  {
    public BrushPickerDropDown()
    {
      this.DefaultStyleKey=typeof(BrushPickerDropDown);
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
      ("ButtonWidth", typeof(double), typeof(BrushPickerDropDown),
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
      ("ProbeVisibility", typeof(Visibility), typeof(BrushPickerDropDown),
       new FrameworkPropertyMetadata(Visibility.Visible,
       FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    public event Action<Brush> SelectedBrushChanged;

    #region SelectedBrush property
    public Brush SelectedBrush
    {
      get => (Brush)GetValue(SelectedBrushProperty);
      set => SetValue(SelectedBrushProperty, value);
    }

    public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register
      ("SelectedBrush", typeof(Brush), typeof(BrushPickerDropDown),
       new FrameworkPropertyMetadata(Brushes.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    #endregion

    private void button_Click(object sender, RoutedEventArgs e)
    {
      var window = new BrushPickerWindow { SelectedBrush = this.SelectedBrush };
      window.ShowDialog();
      if (window.SelectedBrush!=this.SelectedBrush)
      {
        this.SelectedBrush = window.SelectedBrush;
        SelectedBrushChanged?.Invoke(SelectedBrush);
      }
    }

  }
}

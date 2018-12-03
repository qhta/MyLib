using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Qhta.WPF.Utils;
using Qhta.Drawing;

namespace Qhta.WPF.Controls
{
  public partial class ColorEditForm : UserControl
  {
    public ColorEditForm()
    {
      InitializeComponent();
    }

    public override void OnApplyTemplate()
    {
      CurrentColor = Colors.Red;
    }

    #region SelectedColor property
    public Color SelectedColor
    {
      get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
      ("SelectedColor", typeof(Color), typeof(ColorEditForm),
       new FrameworkPropertyMetadata(Colors.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
         SelectedColorPropertyChanged));

    private static void SelectedColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      //(sender as CustomColorForm).UpdatePreview();
    }
    #endregion

    #region CurrentColor property
    public Color CurrentColor
    {
      get => (Color)GetValue(CurrentColorProperty);
      set => SetValue(CurrentColorProperty, value);
    }

    public static readonly DependencyProperty CurrentColorProperty = DependencyProperty.Register
      ("CurrentColor", typeof(Color), typeof(ColorEditForm),
       new FrameworkPropertyMetadata(Colors.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
         CurrentColorPropertyChanged));

    private static void CurrentColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      //(sender as CustomColorForm).UpdatePreview();
    }
    #endregion


    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
      SelectedColor = CurrentColor;
      CloseForm(true);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      CloseForm(false);
    }

    private void CloseForm(bool ok)
    {
      var window = this.FindParentWindow();
      if (window!=null)
      {
        window.DialogResult = ok;
        window.Close();
      }
    }

  }
}

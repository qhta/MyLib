using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Qhta.WPF.Controls
{

  public partial class ColorPickerWindow : Window
  {
    public ColorPickerWindow()
    {
      InitializeComponent();
      DefinedColorsPicker.SelectionChanged+=DefinedColorsPicker_SelectionChanged;
    }

    private void DefinedColorsPicker_SelectionChanged(object sender, ValueChangedEventArgs<KnownColor> args)
    {
      Close();
    }

    #region SelectedColor property
    public Color SelectedColor
    {
      get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
      ("SelectedColor", typeof(Color), typeof(ColorPickerWindow),
       new FrameworkPropertyMetadata(Colors.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    #endregion

  }

}


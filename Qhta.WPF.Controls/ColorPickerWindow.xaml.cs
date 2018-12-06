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
      ColorSelectionForm.SelectedColorChanged+=ColorSelectionForm_SelectedColorChanged;
      ColorSelectionForm.CloseFormRequest+=ColorSelectionForm_CloseFormRequest;
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

    private void ColorSelectionForm_SelectedColorChanged(object sender, ValueChangedEventArgs<Color> args)
    {
      SelectedColor=args.NewValue;
    }

    private void ColorSelectionForm_CloseFormRequest(object sender, EventArgs e)
    {
      Close();
    }

  }

}


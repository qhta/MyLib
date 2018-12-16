using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Qhta.WPF.Controls
{

  public partial class BrushPickerWindow : Window
  {
    public BrushPickerWindow()
    {
      InitializeComponent();
      BrushSelectionForm.BrushSelected+=BrushSelectionForm_SelectedBrushChanged;
      BrushSelectionForm.CloseFormRequest+=BrushSelectionForm_CloseFormRequest;
    }

    #region SelectedBrush property
    public Brush SelectedBrush
    {
      get => (Brush)GetValue(SelectedBrushProperty);
      set => SetValue(SelectedBrushProperty, value);
    }

    public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register
      ("SelectedBrush", typeof(Brush), typeof(BrushPickerWindow),
       new FrameworkPropertyMetadata(Brushes.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    #endregion

    private void BrushSelectionForm_SelectedBrushChanged(object sender, ValueChangedEventArgs<Brush> args)
    {
      SelectedBrush=args.NewValue;
    }

    private void BrushSelectionForm_CloseFormRequest(object sender, EventArgs e)
    {
      Close();
    }

  }

}


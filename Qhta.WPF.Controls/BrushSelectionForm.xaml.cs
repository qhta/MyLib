using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Qhta.WPF.Controls
{
  public partial class BrushSelectionForm : UserControl
  {
    public BrushSelectionForm()
    {
      InitializeComponent();
      DefinedBrushesPicker.SelectionChanged+=DefinedBrushsPicker_SelectionChanged;
      SolidBrushForm.SelectedColorChanged+=SolidBrushForm_SelectedColorChanged;
      DefinedBrushesPicker.CloseFormRequest+=DefinedBrushsPicker_CloseFormRequest;
      SolidBrushForm.CloseFormRequest+=SolidBrushForm_CloseFormRequest;
    }

    #region SelectedBrush property
    public Brush SelectedBrush
    {
      get => (Brush)GetValue(SelectedBrushProperty);
      set => SetValue(SelectedBrushProperty, value);
    }

    public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register
      ("SelectedBrush", typeof(Brush), typeof(BrushSelectionForm),
       new FrameworkPropertyMetadata(Brushes.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    #endregion

    public event ValueChangedEventHandler<Brush> SelectedBrushChanged;
    public event EventHandler CloseFormRequest;

    private void DefinedBrushsPicker_SelectionChanged(object sender, ValueChangedEventArgs<KnownBrush> args)
    {
      SelectedBrush = args.NewValue.Brush;
      SelectedBrushChanged?.Invoke(this, new ValueChangedEventArgs<Brush>(SelectedBrush));
    }

    private void SolidBrushForm_SelectedColorChanged(object sender, ValueChangedEventArgs<Color> args)
    {
      SelectedBrush = new SolidColorBrush(args.NewValue);
      if (KnownBrushes.Instance.FirstOrDefault(item => item.Brush.Equals(args.NewValue))==null)
      {
        var knownBrushs = KnownBrushes.Instance;
        knownBrushs.CustomBrushes.Add(new CustomBrush { Name=args.NewValue.ToString(), Brush=SelectedBrush, IsSelected=true });
      }
      SelectedBrushChanged?.Invoke(this, new ValueChangedEventArgs<Brush>(SelectedBrush));
    }

    private void SolidBrushForm_CloseFormRequest(object sender, EventArgs e)
    {
      CloseFormRequest?.Invoke(this, new EventArgs());
    }

    private void DefinedBrushsPicker_CloseFormRequest(object sender, EventArgs e)
    {
      CloseFormRequest?.Invoke(this, new EventArgs());
    }


  }
}

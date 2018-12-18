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
      SolidBrushForm.ColorSelected+=SolidBrushForm_ColorSelected;
      LinearGradientBrushForm.BrushSelected+=InternalForm_BrushSelected;
      RadialGradientBrushForm.BrushSelected+=InternalForm_BrushSelected;

      DefinedBrushesPicker.CloseFormRequest+=InternalForm_CloseFormRequest;
      SolidBrushForm.CloseFormRequest+=InternalForm_CloseFormRequest;
      LinearGradientBrushForm.CloseFormRequest+=InternalForm_CloseFormRequest;
      RadialGradientBrushForm.CloseFormRequest+=InternalForm_CloseFormRequest;
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

    public event ValueChangedEventHandler<Brush> BrushSelected;
    public event EventHandler CloseFormRequest;

    private void DefinedBrushsPicker_SelectionChanged(object sender, ValueChangedEventArgs<KnownBrush> args)
    {
      SelectedBrush = args.NewValue.Brush;
      BrushSelected?.Invoke(this, new ValueChangedEventArgs<Brush>(SelectedBrush));
    }

    private void SolidBrushForm_ColorSelected(object sender, ValueChangedEventArgs<Color> args)
    {
      SelectedBrush = new SolidColorBrush(args.NewValue);
      if (KnownBrushes.Instance.FirstOrDefault(item => item.Brush.Equals(args.NewValue))==null)
      {
        var knownBrushs = KnownBrushes.Instance;
        knownBrushs.CustomBrushes.Add(new CustomBrush { Name=args.NewValue.ToString(), Brush=SelectedBrush, IsSelected=true });
      }
      BrushSelected?.Invoke(this, new ValueChangedEventArgs<Brush>(SelectedBrush));
    }

    private void InternalForm_BrushSelected(object sender, ValueChangedEventArgs<Brush> args)
    {
      SelectedBrush = args.NewValue;
      if (KnownBrushes.Instance.FirstOrDefault(item => item.Brush.Equals(args.NewValue))==null)
      {
        var knownBrushes = KnownBrushes.Instance;
        int n = knownBrushes.Count+1;
        var name = $"Brush_{n++}";
        while (knownBrushes.FirstOrDefault(item=>item.Name==name)!=null)
          name = $"Brush_{n++}";
        knownBrushes.CustomBrushes.Add(new CustomBrush { Name=name, Brush=SelectedBrush, IsSelected=true });
      }
      BrushSelected?.Invoke(this, new ValueChangedEventArgs<Brush>(SelectedBrush));
    }

    private void InternalForm_CloseFormRequest(object sender, EventArgs e)
    {
      CloseFormRequest?.Invoke(this, new EventArgs());
    }



  }
}

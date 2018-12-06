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
  public partial class ColorSelectionForm : UserControl
  {
    public ColorSelectionForm()
    {
      InitializeComponent();
      DefinedColorsPicker.SelectionChanged+=DefinedColorsPicker_SelectionChanged;
      CustomColorForm.SelectedColorChanged+=CustomColorForm_SelectedColorChanged;
      DefinedColorsPicker.CloseFormRequest+=DefinedColorsPicker_CloseFormRequest; ;
      CustomColorForm.CloseFormRequest+=CustomColorForm_CloseFormRequest;
    }

    #region SelectedColor property
    public Color SelectedColor
    {
      get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
      ("SelectedColor", typeof(Color), typeof(ColorSelectionForm),
       new FrameworkPropertyMetadata(Colors.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    #endregion

    public event ValueChangedEventHandler<Color> SelectedColorChanged;
    public event EventHandler CloseFormRequest;

    private void DefinedColorsPicker_SelectionChanged(object sender, ValueChangedEventArgs<KnownColor> args)
    {
      SelectedColor = args.NewValue.Color;
      SelectedColorChanged?.Invoke(this, new ValueChangedEventArgs<Color>(SelectedColor));
    }

    private void CustomColorForm_SelectedColorChanged(object sender, ValueChangedEventArgs<Color> args)
    {
      SelectedColor = args.NewValue;
      if (KnownColors.Instance.FirstOrDefault(item => item.Color.Equals(args.NewValue))==null)
      {
        var knownColors = KnownColors.Instance;
        knownColors.CustomColors.Add(new CustomColor { Name=args.NewValue.ToString(), Color=args.NewValue, IsSelected=true });
      }
      SelectedColorChanged?.Invoke(this, args);
    }

    private void CustomColorForm_CloseFormRequest(object sender, EventArgs e)
    {
      CloseFormRequest?.Invoke(this, new EventArgs());
    }

    private void DefinedColorsPicker_CloseFormRequest(object sender, EventArgs e)
    {
      CloseFormRequest?.Invoke(this, new EventArgs());
    }


  }
}

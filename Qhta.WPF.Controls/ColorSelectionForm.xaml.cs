using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Qhta.WPF.Controls
{
  /// <summary>
  /// Interaction logic for ColorSelectionForm.xaml
  /// </summary>
  public partial class ColorSelectionForm : UserControl
  {
    public ColorSelectionForm()
    {
      InitializeComponent();
      DefinedColorsPicker.SelectionChanged+=DefinedColorsPicker_SelectionChanged;
      CustomColorForm.SelectedColorChanged+=CustomColorForm_SelectedColorChanged;
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
      CloseFormRequest?.Invoke(this, new EventArgs());
    }

    private void CustomColorForm_SelectedColorChanged(object sender, ValueChangedEventArgs<Color> args)
    {
      SelectedColor = args.NewValue;
      SelectedColorChanged?.Invoke(this, args);
    }

    private void CustomColorForm_CloseFormRequest(object sender, EventArgs e)
    {
      CloseFormRequest?.Invoke(this, new EventArgs());
    }


  }
}

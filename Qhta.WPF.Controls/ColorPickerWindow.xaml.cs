using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Qhta.WPF.Utils;

namespace Qhta.WPF.Controls
{

  public partial class ColorPickerWindow : Window
  {
    public ColorPickerWindow()
    {
      InitializeComponent();
      InitialWork();
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


    private void InitialWork()
    {
      DefinedColorsPicker.Items.Clear();
      KnownColors customColors = new KnownColors();
      foreach (var item in customColors.SelectableColors)
      {
        DefinedColorsPicker.Items.Add(item);
      }
      DefinedColorsPicker.SelectionChanged += new SelectionChangedEventHandler(DefinedColorsPicker_SelectionChanged);
    }

    void DefinedColorsPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (DefinedColorsPicker.SelectedValue != null)
      {
        SelectedColor = (DefinedColorsPicker.SelectedValue as KnownColor).Color;
      }
      Close();
    }

  }

}


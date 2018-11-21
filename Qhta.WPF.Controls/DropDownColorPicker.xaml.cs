using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
  public partial class DropDownColorPicker : UserControl
  {
    public DropDownColorPicker()
    {
      InitializeComponent();
    }

    public event Action<Color> SelectedColorChanged;

    #region SelectedColor property
    public Color SelectedColor
    {
      get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
      ("SelectedColor", typeof(Color), typeof(DropDownColorPicker),
       new FrameworkPropertyMetadata(Colors.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    private static void SelectedColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
    }
    #endregion

//    bool _isContexMenuOpened = false;

    //void ContextMenu_Closed(object sender, RoutedEventArgs e)
    //{
    //  if (!b.ContextMenu.IsOpen)
    //  {
    //    SelectedColorChanged?.Invoke(cp.SelectedColor);
    //    SelectedColor = cp.SelectedColor;
    //  }
    //  _isContexMenuOpened = false;
    //}

    private void button_Click(object sender, RoutedEventArgs e)
    {
      var window = new ColorPickerWindow { SelectedColor = this.SelectedColor };
      window.ShowDialog();
      if (window.SelectedColor!=this.SelectedColor)
      {
        this.SelectedColor = window.SelectedColor;
        SelectedColorChanged?.Invoke(SelectedColor);
      }
    }
  }
}

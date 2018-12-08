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
  public partial class KnownColorSelectView : UserControl
  {
    public KnownColorSelectView()
    {
      InitializeComponent();
      Init();
    }


    #region SelectedColor property
    public Color SelectedColor
    {
      get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
      ("SelectedColor", typeof(Color), typeof(KnownColorSelectView),
       new FrameworkPropertyMetadata(Colors.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    #endregion

    public event ValueChangedEventHandler<KnownColor> SelectionChanged;
    public event EventHandler CloseFormRequest;

    private void Init()
    {
      KnownColorsListBox.ItemsSource = KnownColors.Instance;
      KnownColorsListBox.SelectionChanged += new SelectionChangedEventHandler(KnownColorsListBox_SelectionChanged);
    }

    void KnownColorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
    {
      if (args.AddedItems!=null)
      {
        var KnownColor = args.AddedItems.Cast<KnownColor>().FirstOrDefault() as KnownColor;
        if (KnownColor != null)
        {
          if (KnownColor.Color!=SelectedColor)
          {
            SelectedColor = KnownColor.Color;
            SelectionChanged?.Invoke(this,
              new ValueChangedEventArgs<KnownColor>(KnownColorsListBox.SelectedValue as KnownColor));
          }
        }
      }
    }

    private void ColorNameTextBox_LostFocus(object sender, RoutedEventArgs args)
    {
      var customColor = KnownColorsListBox.SelectedItem as CustomColor;
      var name = (sender as TextBox).Text.Trim();
      if (String.IsNullOrEmpty(name))
        name = customColor.Color.ToString();
      customColor.Name = name;
    }

    private void DeleteMenuItem_Click(object sender, RoutedEventArgs args)
    {
      var customColor = KnownColorsListBox.SelectedItem as CustomColor;
      KnownColors.Instance.CustomColors.Remove(customColor);
    }

    private void KnownColorsListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs args)
    {
      CloseFormRequest?.Invoke(this, args);
    }
  }
}

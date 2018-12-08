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
  public partial class KnownBrushSelectView : UserControl
  {
    public KnownBrushSelectView()
    {
      InitializeComponent();
      Init();
    }


    #region SelectedBrush property
    public Brush SelectedBrush
    {
      get => (Brush)GetValue(SelectedBrushProperty);
      set => SetValue(SelectedBrushProperty, value);
    }

    public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register
      ("SelectedBrush", typeof(Brush), typeof(KnownBrushSelectView),
       new FrameworkPropertyMetadata(Brushes.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    #endregion

    public event ValueChangedEventHandler<KnownBrush> SelectionChanged;
    public event EventHandler CloseFormRequest;

    private void Init()
    {
      KnownBrushesListBox.ItemsSource = KnownBrushes.Instance;
      KnownBrushesListBox.SelectionChanged += new SelectionChangedEventHandler(KnownBrushesListBox_SelectionChanged);
    }

    void KnownBrushesListBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
    {
      if (args.AddedItems!=null)
      {
        var KnownBrush = args.AddedItems.Cast<KnownBrush>().FirstOrDefault() as KnownBrush;
        if (KnownBrush != null)
        {
          if (KnownBrush.Brush!=SelectedBrush)
          {
            SelectedBrush = KnownBrush.Brush;
            SelectionChanged?.Invoke(this,
              new ValueChangedEventArgs<KnownBrush>(KnownBrushesListBox.SelectedValue as KnownBrush));
          }
        }
      }
    }

    private void BrushNameTextBox_LostFocus(object sender, RoutedEventArgs args)
    {
      var customBrush = KnownBrushesListBox.SelectedItem as CustomBrush;
      var name = (sender as TextBox).Text.Trim();
      if (String.IsNullOrEmpty(name))
        name = customBrush.Brush.ToString();
      customBrush.Name = name;
    }

    private void DeleteMenuItem_Click(object sender, RoutedEventArgs args)
    {
      var customBrush = KnownBrushesListBox.SelectedItem as CustomBrush;
      KnownBrushes.Instance.CustomBrushes.Remove(customBrush);
    }

    private void KnownBrushesListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs args)
    {
      CloseFormRequest?.Invoke(this, args);
    }
  }
}

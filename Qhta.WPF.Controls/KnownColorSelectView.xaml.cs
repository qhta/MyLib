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

    KnownColors customColors = new KnownColors();
    private void Init()
    {
      KnownColorsListBox.Items.Clear();
      foreach (var item in customColors)
      {
        KnownColorsListBox.Items.Add(item);
      }
      KnownColorsListBox.SelectionChanged += new SelectionChangedEventHandler(KnownColorsListBox_SelectionChanged);
    }

    void KnownColorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
    {
      if (KnownColorsListBox.SelectedValue != null)
      {
        SelectedColor = (KnownColorsListBox.SelectedValue as KnownColor).Color;
        SelectionChanged?.Invoke(this, 
          new ValueChangedEventArgs<KnownColor>(KnownColorsListBox.SelectedValue as KnownColor));
      }
    }

  }
}

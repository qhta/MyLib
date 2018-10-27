using System.Windows;
using System.Windows.Controls;

namespace Qhta.WPF.Controls
{
  public partial class PanelHeader : UserControl
  {
    public PanelHeader()
    {
      InitializeComponent();
    }

    public static DependencyProperty TextProperty = DependencyProperty.Register
      ("Text", typeof(string), typeof(PanelHeader), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsArrange));

    public string Text
    {
      get => (string)GetValue(TextProperty);
      set => SetValue(TextProperty, value);
    }

  }
}
